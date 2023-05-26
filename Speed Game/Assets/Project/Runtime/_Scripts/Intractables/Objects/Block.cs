using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Core.Player;

namespace SpeedGame
{
    public class Block : MonoBehaviour
    {
        // Definition:
        // This Class will handle what the block does:
        private Rigidbody2D _rigidbody;
        private Vector3 forceDirection;
        [SerializeField] private float forceMagnitude;
        [SerializeField] private bool isMoveable = true;
        [SerializeField] private bool isBreakable = false;
        [SerializeField] private bool playerInRange;

        private Rigidbody2D playerRigidbody;
        private PlayerMovementControls playerController;
        private PlayerInventory playerInventory;

        public List<Collider2D> colliders;


        private void Awake() 
        {
            playerController = PlayerMovementControls.Instance;
            _rigidbody = gameObject.GetComponentInParent<Rigidbody2D>();
            if(playerController != null)
            {
                playerRigidbody = playerController.GetComponent<Rigidbody2D>();
                playerInventory = playerController.GetComponent<PlayerInventory>();
            }
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(!colliders.Contains(other))
            {
                colliders.Add(other);
            }
            
            if(other.tag == "Player")
            {
                playerController = other.gameObject.GetComponent<PlayerMovementControls>();
                playerRigidbody = playerController.GetComponent<Rigidbody2D>();
                playerInventory = playerController.GetComponent<PlayerInventory>();

                playerInRange = true;
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                playerInRange = false;
            }
            if(colliders.Contains(other))
            {
                colliders.Remove(other);
            }
        }

        private void FixedUpdate() 
        {
            if(isMoveable)
            {
                isBreakable = false;
                if(playerController != null)
                {
                    MoveBlock();
                }
                
            }

            else if(isBreakable)
            {
                isMoveable = false;
                BreakBlock();

            }
        }

        private void MoveBlock()
        {
            var playerHasGloves = playerInventory.GetCurrentItem() == PlayerInventory.Items.Powerups_PunchingGlove;
            var playerCanDash = playerController.GetCanDash() == true;
            var playerIsAbove = playerController.gameObject.transform.position.y > transform.position.y + 0.4f;
            
            if (playerInRange && playerHasGloves && !playerCanDash && !playerIsAbove)
            {
                playerRigidbody.velocity = Vector2.zero;
                _rigidbody.velocity = Vector2.zero;
            }

            if(playerInRange && !playerIsAbove)
            {
                forceDirection = transform.position - playerController.gameObject.transform.position;
                forceDirection.y = 0;
                forceDirection.Normalize();

                if(playerHasGloves && playerCanDash)
                {
                    _rigidbody.velocity = new Vector2(playerController.GetPlayerInputX() * forceMagnitude, _rigidbody.velocity.y);
                }
                else if(!playerHasGloves && playerController.GetPlayerInputX() > 0)
                {
                    //Debug.Log("Moving");
                    _rigidbody.velocity = new Vector2(forceMagnitude, _rigidbody.velocity.y);

                    if(_rigidbody.velocity.y < forceMagnitude)
                    {
                       _rigidbody.velocity = new Vector2(forceMagnitude, _rigidbody.velocity.y); 
                    }
                }
                else if(!playerHasGloves && playerController.GetPlayerInputX() < 0)
                {
                    //Debug.Log("Moving");
                    _rigidbody.velocity = new Vector2(-forceMagnitude, _rigidbody.velocity.y);
                    
                    if(_rigidbody.velocity.y < forceMagnitude)
                    {
                       _rigidbody.velocity = new Vector2(-forceMagnitude, _rigidbody.velocity.y); 
                    }
                }
                
                
            }
        }

        private void BreakBlock()
        {
            if(playerInRange && playerController.GetIsDashing() && !playerController.GetPlayerIsGrounded())
            {
                var playerIsAbove = playerController.gameObject.transform.position.y > transform.position.y + 0.4f;
                
                if(!playerIsAbove)
                {
                    Destroy(_rigidbody.gameObject);
                    Destroy(gameObject);
                }
            }
            else if(playerInRange && playerInventory.GetCurrentItem() == PlayerInventory.Items.Powerups_LeadBoots)
            {
                var playerIsAbove = playerController.gameObject.transform.position.y > transform.position.y + 0.4f;
                if(playerIsAbove)
                {
                    Destroy(_rigidbody.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
