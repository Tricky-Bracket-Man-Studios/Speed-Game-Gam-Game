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

        private void Start()
        {
            
        }

        private void Update() 
        {
            if(testPlate.GetPlateState())
            {
                meshRenderer.enabled = true;
                if(startTimer)
                {
                    timer.StartWatch(TimerLogic.Types.Timer, 120f);
                    timer.AddTimeInSecs(60f);
                    startTimer = false;
                }
                
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }
        
    }
}
