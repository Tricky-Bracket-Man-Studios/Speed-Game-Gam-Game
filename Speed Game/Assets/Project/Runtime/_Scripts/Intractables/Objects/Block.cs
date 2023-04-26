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

        [SerializeField] private PlayerController2 player;
        private Rigidbody2D playerRigidbody;
        private PlayerController playerController;
        private PlayerInventory playerInventory;


        private void Awake() 
        {
            player = PlayerController2.Instance;
            _rigidbody = gameObject.GetComponentInParent<Rigidbody2D>();
            if(player != null)
            {
                playerRigidbody = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerController>();
                playerInventory = player.GetComponent<PlayerInventory>();
            }
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                //player = other.gameObject;
                playerRigidbody = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerController>();
                playerInventory = player.GetComponent<PlayerInventory>();

                playerInRange = true;
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                playerInRange = false;
            }
        }

        private void Update() 
        {
            if(isMoveable)
            {
                isBreakable = false;
                MoveBlock();
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
            var playerIsAbove = player.transform.position.y > transform.position.y + 0.4f;
            
            if (playerInRange && playerHasGloves && !playerCanDash && !playerIsAbove)
            {
                playerRigidbody.velocity = Vector2.zero;
                _rigidbody.velocity = Vector2.zero;
            }

            if(playerInRange && (playerController.GetPlayerColLeft() || playerController.GetPlayerColRight()))
            {
                forceDirection = transform.position - player.transform.position;
                forceDirection.y = 0;
                forceDirection.Normalize();

                if(playerHasGloves && playerCanDash)
                {
                    _rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, player.transform.position, ForceMode2D.Impulse);
                }
                else if(!playerHasGloves)
                {
                    _rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, player.transform.position, ForceMode2D.Impulse);
                }
                
                
            }
        }

        private void BreakBlock()
        {
            if(playerInRange && playerController.GetIsDashing() && !playerController.GetPlayerColDown())
            {
                var playerIsAbove = player.transform.position.y > transform.position.y + 0.4f;
                
                if(!playerIsAbove)
                {
                    Destroy(_rigidbody.gameObject);
                    Destroy(gameObject);
                }
            }
            else if(playerInRange && playerInventory.GetCurrentItem() == PlayerInventory.Items.Powerups_LeadBoots)
            {
                var playerIsAbove = player.transform.position.y > transform.position.y + 0.4f;
                if(playerIsAbove)
                {
                    Destroy(_rigidbody.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
