using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Intractables.Objects;
using SpeedGame.Core;

namespace SpeedGame
{
    public class Test : MonoBehaviour
    {
        public PressurePlate testPlate;
        public MeshRenderer meshRenderer;

        public TimerLogic timer;
        public bool startTimer = true;
        public DoorLogic door;

        private void Start()
        {
            
        }

        private void Update() 
        {

            if(testPlate.GetPlateState() == true)
            {
                meshRenderer.enabled = true;
                
                if(startTimer)
                {
                    timer.StartWatch(TimerLogic.Types.Timer, 10f);
                    startTimer = false;
                }
                if(timer.GetCurrentTime() > 0f)
                {
                    door.SetDoorState(true);
                }
                
            }
            else if(testPlate.GetPlateState() == false)
            {
                door.SetDoorState(false);
            }
        }
        
    }
}
