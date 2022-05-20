using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingInput : MonoBehaviour
{
    [SerializeField] BuildingSystem bSys;
    [SerializeField] InputAction placeInput, rotateInput, nextInput, previousInput, deleteInput;

    void Awake()
    {
        placeInput.performed += context => { if (context.performed) bSys.PlacePiece(); };
        rotateInput.performed += context => { if (context.performed) bSys.RotatePiece(); };
        nextInput.performed += context => { if (context.performed) bSys.NextPart(); };
        previousInput.performed += context => { if (context.performed) bSys.PreviousPart(); };
        deleteInput.performed += context => { if (context.performed) bSys.DeletePiece(); };
    }
    private void OnEnable()
    {
        placeInput.Enable();
        rotateInput.Enable();
        nextInput.Enable();
        previousInput.Enable();
        deleteInput.Enable();
    }
    private void OnDisable()
    {
        placeInput.Disable();
        rotateInput.Disable();
        nextInput.Disable();
        previousInput.Disable();
        deleteInput.Disable();
    }
}
