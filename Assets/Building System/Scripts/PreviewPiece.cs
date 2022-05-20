using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPiece : MonoBehaviour
{
    [SerializeField] BuildingPieceSO bp;
    [SerializeField] Transform snapPoint;
    [SerializeField] Transform gfxRoot;
    public Transform GetGFXRoot => gfxRoot;
    [SerializeField] MeshRenderer[] meshes;
    public MeshRenderer[] GetPreviewMeshes => meshes;
    [SerializeField] float rotationIncrement = 90;
    [SerializeField] Color validColour = new Color(0,1,0,.5f), invalidColour = new Color(1, 0, 0, .5f);
    [SerializeField] LayerMask prohibitedOverlap, requiredOverlap;
    [SerializeField] bool requireOverlap = false, requireSnap = true;
    [SerializeField] BuildingPieceSO.PieceTypes[] allowedOverlapPieces;
    [SerializeField] Vector3 overlapCheckBoxHalfSize = new Vector3(1.28f, .64f, 1.28f), overlapCheckboxCenter = Vector3.zero;
    [SerializeField] [Range(0 , 2)] float overlapCheckBoxScale = .75f;

    Transform player;
    BuildingSystem bSys;
    public bool snapped { get; private set; } = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bSys = player.GetComponent<BuildingSystem>();
    }
    public void SetSnap(Transform _snapPos, BuildingPiece _targetBP)
    {
        if (snapped) return;
        transform.eulerAngles = Vector3.zero;
        transform.position = CalcOffset(_snapPos.position, snapPoint.localPosition);

        Vector3 targetBPCenter = _snapPos.parent.position;
        targetBPCenter.y = snapPoint.position.y;
        Vector3 dirToSide = (targetBPCenter - snapPoint.position).normalized;
        float dirAngle = Vector3.SignedAngle(snapPoint.forward, dirToSide, Vector3.up);
        transform.RotateAround(snapPoint.position, Vector3.up, dirAngle);

        switch (bp.pieceType)
        {
            case BuildingPieceSO.PieceTypes.Foundation:
                
                break;

            case BuildingPieceSO.PieceTypes.Wall:
                if (_targetBP.GetPieceType == BuildingPieceSO.PieceTypes.Wall)
                {
                    transform.forward = _snapPos.parent.forward;
                }
                else
                {
                    transform.forward = _snapPos.forward;
                }
                break;

            case BuildingPieceSO.PieceTypes.CeilingFloor:
                if (_targetBP.GetPieceType == BuildingPieceSO.PieceTypes.Wall)
                {
                    transform.forward = _snapPos.parent.forward * (IsPlayerInFront(_snapPos.parent) ? -1 : 1);

                    transform.position = CalcOffset(_snapPos.position, snapPoint.localPosition);
                }
                break;

            case BuildingPieceSO.PieceTypes.SingleDoor:
            case BuildingPieceSO.PieceTypes.DoubleDoor:
                transform.forward = _snapPos.forward;
                break;
            case BuildingPieceSO.PieceTypes.InteriorUnit:
                transform.forward = _snapPos.forward;
                transform.position = CalcOffset(_snapPos.position, snapPoint.localPosition);
                break;
        }

        snapped = true;
        Physics.SyncTransforms();
    }
    private void Update()
    {
        if(meshes.Length > 0)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].material.color = IsOverlapValid() ? validColour : invalidColour;
            }
        }
    }
    bool IsPlayerInFront(Transform _target)
    {
        Vector3 dirToPlayer = (player.position - _target.position).normalized;
        float dot = Vector3.Dot(_target.forward, dirToPlayer);

        if (dot > 0) { return true; }
        else { return false; }
    }
    public void Unsnap()
    {
        snapped = false;
    }
    Vector3 CalcOffset(Vector3 startPos, Vector3 offset)
    {
        if(transform.eulerAngles != Vector3.zero)
        {
            Quaternion rot = transform.rotation;
            Vector3 newOffset = rot * offset;
            offset = newOffset;
        }

        return startPos - offset;
    }
    public bool IsOverlapValid()
    {
        Vector3 boxPos = new Vector3(transform.position.x + overlapCheckboxCenter.x, transform.position.y + overlapCheckboxCenter.y, transform.position.z + overlapCheckboxCenter.z);

        Collider[] overlapCols = new Collider[0];
        overlapCols = Physics.OverlapBox(boxPos, overlapCheckBoxHalfSize * overlapCheckBoxScale, transform.rotation, prohibitedOverlap);

        if (overlapCols.Length > 0)
        {
            for (int i = 0; i < overlapCols.Length; i++)
            {
                if (overlapCols[i].transform.root.TryGetComponent(out BuildingPiece overlapBP))
                {
                    if (allowedOverlapPieces.Contains(overlapBP.GetPieceType))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        if(requireSnap && !snapped)
        {
            return false;
        }
        if (requireOverlap)
        {
            if(!Physics.CheckBox(boxPos, overlapCheckBoxHalfSize * overlapCheckBoxScale, transform.rotation, requiredOverlap))
            {
                return false;
            }
        }
        return true;
    }
    public void RotatePiece()
    {
        gfxRoot.rotation *= Quaternion.Euler(0, rotationIncrement, 0);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(overlapCheckboxCenter, (overlapCheckBoxHalfSize * 2) * overlapCheckBoxScale);
    }
}
