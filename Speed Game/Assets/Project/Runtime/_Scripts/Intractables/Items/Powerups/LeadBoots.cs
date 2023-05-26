using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Core.Player;

namespace SpeedGame.Intractables.Items.Powerups
{
    public class LeadBoots : MonoBehaviour
    {
        // Definition: 
        // This class will handle all the logic for the punching glove.

        // Variables:
        private bool playerInRange = false;
        [SerializeField] private PlayerMovementControls player;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private Transform _parentTransform;
        
        // Private functions:
        // if the player and only the player collides with the punching glove, enable their dash.
        private void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.tag == "Player")
            {
                player = other.gameObject.GetComponent<PlayerMovementControls>() ;
                playerInventory = other.gameObject.GetComponent<PlayerInventory>() ;

                playerInRange = true;
            }
            
        }
        private void OnTriggerExit2D(Collider2D other) 
        {
            if(other.tag == "Player")
            {
                playerInRange = false;
            }
            
        }

        private void Update()
        {
            if(playerInRange)
            {
                if(player.GetPlayerIsInteracting())
                {
                    playerInventory.AddToInventory(PlayerInventory.Items.Powerups_LeadBoots);
                    playerInventory.AddStoredPowerup(gameObject);
                    gameObject.SetActive(false);
                }
            }
            
        }

    }
}
