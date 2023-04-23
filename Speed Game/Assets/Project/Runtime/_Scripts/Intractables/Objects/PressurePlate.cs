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
        [SerializeField] private List<GameObject> objectsOnButton;

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
            if(!objectsOnButton.Contains(other.gameObject) && !other.isTrigger)
            {
                objectsOnButton.Add(other.gameObject);
            }
            if(!isPressed && !other.isTrigger)
            {   
                _animation.CrossFade(Pressed, 0, 0);
                isPressed = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if(objectsOnButton.Contains(other.gameObject))
            {
                objectsOnButton.Remove(other.gameObject);
            }
            
            if(isButton && objectsOnButton.Count == 0)
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
