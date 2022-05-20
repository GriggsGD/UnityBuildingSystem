using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("Building Assets")]
    [SerializeField] BuildingPieceSO[] buildingParts;
    public BuildingPieceSO[] GetBuildingPartsData => buildingParts;
    int buildingIndex; //Current selected building part/piece
    public int GetSelectedBuildingPartIndex => buildingIndex;

    [Header("Building Mechanics Properties")]
    [SerializeField] float maxBuildDistance = 10;
    [Tooltip("Layers that the build aim raycast will collide with and detect")][SerializeField] LayerMask buildAimPosHitLayers;
    [Tooltip("Enable/Disable building ability")] public bool canBuild = true;

    GameObject currPreview;
    BuildingPiece bpCtrl;
    BuildingConnector bpConnector, currSnappedConnector;
    PreviewPiece ppCtrl;

    private void Update()
    {
        GetAimPos(); //Bodge job I KNOW! xD
        PreviewBuild();
    }
    public void NextPart()
    {
        if (buildingIndex >= buildingParts.Length - 1) buildingIndex = 0;
        else buildingIndex++;
        Destroy(currPreview);
    }
    public void PreviousPart()
    {
        if (buildingIndex <= 0) buildingIndex = buildingParts.Length-1;
        else buildingIndex--;
        Destroy(currPreview);
    }
    public void SelectPart(int _partIndex)
    {
        if(_partIndex > buildingParts.Length - 1)
        {
            Debug.LogError("Provided building part index is out of array range!", this);
            return;
        }
        else
        {
            buildingIndex = _partIndex;
            Destroy(currPreview);
        }
    }
    public void PlacePiece()
    {
        if (!CanPlace())
        {
            return;
        }
        BuildingPiece parentBPCtrl = currSnappedConnector != null ? currSnappedConnector.transform.root.GetComponent<BuildingPiece>() : null;
        Vector3 placementPos = currPreview.transform.position;
        Quaternion placementRotation = currPreview.transform.rotation;
        Quaternion gfxRotation = currPreview.GetComponent<PreviewPiece>().GetGFXRoot.rotation;
        GameObject placedPiece = Instantiate(buildingParts[buildingIndex].buildingPrefab, placementPos, placementRotation);
        BuildingPiece placedBPCtrl = placedPiece.GetComponent<BuildingPiece>();
        placedBPCtrl.GetGFXRoot.rotation = gfxRotation;
        if(buildingParts[buildingIndex].pieceType != BuildingPieceSO.PieceTypes.Foundation)
        {
            placedBPCtrl.AddParentPart(parentBPCtrl.transform);
            parentBPCtrl.OnDestroy += placedBPCtrl.DestroyPiece;
        }
    }
    public void RotatePiece()
    {
        currPreview.GetComponent<PreviewPiece>().RotatePiece();
    }
    public void DeletePiece()
    {
        if (bpCtrl == null)
        {
            return;
        }
        else
        {
            bpCtrl.DestroyPiece();
        }
    }
    
    bool CanSnap()
    {
        if (bpConnector == null)
        {
            return false;
        }

        if (bpConnector.GetAllowedPieces.Contains(buildingParts[buildingIndex].pieceType)){
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CanPlace()
    {
        if (!canBuild)
        {
            return false;
        }
        if (ppCtrl.IsOverlapValid())
        {
            return true;
        }
        else
        {
            Debug.Log("Invalid Placement!");
            return false;
        }
    }
    void PreviewBuild()
    {
        
        if(currPreview == null)
        {
            currPreview = Instantiate(buildingParts[buildingIndex].previewPrefab, GetAimPos(), Quaternion.identity);
            ppCtrl = currPreview.GetComponent<PreviewPiece>();
        }

        if (CanSnap())
        {
            if (!ppCtrl.snapped) 
            {
                currSnappedConnector = bpConnector;
                ppCtrl.SetSnap(currSnappedConnector.transform, bpCtrl); 
            }
        }
        else
        {
            currSnappedConnector = null;
            ppCtrl.Unsnap();
            var lookPos = GetAimPos() - Camera.main.transform.position;
            lookPos.y = 0;
            var previewRotation = Quaternion.LookRotation(lookPos);

            currPreview.transform.rotation = previewRotation;

            currPreview.transform.position = GetAimPos();
        }
    }

    Vector3 GetAimPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit rcHit, maxBuildDistance, buildAimPosHitLayers))
        {
            if(rcHit.transform.TryGetComponent(out BuildingConnector _bConnect))
            {
                bpConnector = _bConnect;
                bpCtrl = bpConnector.transform.root.GetComponent<BuildingPiece>();
            }
            else if (rcHit.transform.root.TryGetComponent(out BuildingPiece _bp))
            {
                bpCtrl = _bp;
                bpConnector = null;
            }
            else
            {
                bpCtrl = null;
                bpConnector = null;
            }
            return rcHit.point;
        }
        else
        {
            bpConnector = null;
            bpCtrl = null;
        }
        return ray.GetPoint(maxBuildDistance);
    }
}
