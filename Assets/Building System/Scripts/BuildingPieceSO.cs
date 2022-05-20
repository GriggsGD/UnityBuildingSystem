using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Building Piece", menuName = "Building System/Building Piece")]
public class BuildingPieceSO : ScriptableObject
{
    public enum PieceTypes
    {
        Foundation,
        Wall,
        CeilingFloor,
        SingleDoor,
        DoubleDoor,
        InteriorUnit
    }
    public PieceTypes pieceType;
    public string buildingPieceName;
    public GameObject previewPrefab;
    public GameObject buildingPrefab;

}
