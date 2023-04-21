using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Core.Player;

namespace SpeedGame.Intractables.Items.Powerups
{
    public class PunchingGlove : MonoBehaviour
    {
        // Definition: 
        // This class will handle all the logic for the punching glove.

        // if the player and only the player collides with the punching glove, enable their dash.
        private void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerInventory>().AddToInventory(PlayerInventory.Items.Powerups_PunchingGlove);
                Destroy(gameObject);
            }
            
        }

    }
}
