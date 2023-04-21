using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Intractables.Objects;

namespace SpeedGame
{
    public class Test : MonoBehaviour
    {
        public PressurePlate testPlate;
        public MeshRenderer meshRenderer;
        private void Update() 
        {
            if(testPlate.GetPlateState())
            {
                meshRenderer.enabled = true;
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }
        
    }
}
