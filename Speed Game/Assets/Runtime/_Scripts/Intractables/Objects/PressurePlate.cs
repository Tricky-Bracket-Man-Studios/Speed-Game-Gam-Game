using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedGame.Intractables.Objects
{
    public class PressurePlate : MonoBehaviour
    {
        // Definition:
        // This class manages the  PressurePlate logic:

        // Variables:
        private Animator _animation;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool isPressed = false;
        [SerializeField] private bool isButton = false;

        // Graphic states:
        private static readonly int Unpressed = Animator.StringToHash("Unpressed");
        private static readonly int Pressed = Animator.StringToHash("Pressed");

        // Private functions:
        private void Start() 
        {
            _animation = gameObject.GetComponent<Animator>();
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();    
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!isPressed)
            {
                _animation.CrossFade(Pressed, 0, 0);
                isPressed = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if(isButton)
            {
                _animation.CrossFade(Unpressed, 0, 0);
                isPressed = false;
            }
        }
        
        // public functions:

        public bool GetPlateState()
        {
            return isPressed;
        }
        
    }
}
