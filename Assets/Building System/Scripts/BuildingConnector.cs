using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConnector: MonoBehaviour
{
    [SerializeField] BuildingPieceSO.PieceTypes[] allowedPieces;
    public BuildingPieceSO.PieceTypes[] GetAllowedPieces => allowedPieces;
}
