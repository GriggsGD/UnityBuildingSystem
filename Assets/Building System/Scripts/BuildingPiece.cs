using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPiece : MonoBehaviour
{
    [SerializeField] BuildingPieceSO bp;
    public BuildingPieceSO GetPieceAsset => bp;
    [SerializeField] float rotationIncrement = 90;
    public BuildingPieceSO.PieceTypes GetPieceType => bp.pieceType;
    public Transform GetGFXRoot => GFXRoot;
    [SerializeField] Transform GFXRoot;
    [SerializeField] MeshRenderer[] meshes;
    Transform parentPart;
    public Transform GetParentPart => parentPart;
    public Action OnDestroy;
    public void DestroyPiece()
    {
        OnDestroy?.Invoke();
        if(parentPart != null)
        {
            parentPart.GetComponent<BuildingPiece>().OnDestroy -= DestroyPiece;
        }
        Destroy(gameObject);
    }
    public void AddParentPart(Transform _parent)
    {
        parentPart = _parent;
    }
    public void RotatePiece()
    {
        GFXRoot.rotation *= Quaternion.Euler(0, rotationIncrement, 0);
    }
}
