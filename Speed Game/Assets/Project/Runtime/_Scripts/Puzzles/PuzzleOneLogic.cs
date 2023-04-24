using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Intractables.Objects;

public class PuzzleOneLogic : MonoBehaviour
{
    //Definition:
    // This will link our puzzle together

    [SerializeField] private PressurePlate pressurePlate1;
    [SerializeField] private PressurePlate pressurePlate2;
    [SerializeField] private DoorLogic door1;
    [SerializeField] private DoorLogic door2;

    private void Start()
    {
        door1.SetDoorOpenPos(door1.GetDoorClosePos().x - 2f, door1.GetDoorClosePos().y);
        
    }

    private void Update()
    {
        door1.SetDoorState(pressurePlate1.GetPlateState());
        door2.SetDoorState(pressurePlate2.GetPlateState());
    }

}
