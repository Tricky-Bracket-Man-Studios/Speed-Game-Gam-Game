using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedGame.Core.Environment
{
    public class DeathTrigger : MonoBehaviour
    {
        // Definition:
        // This Class will handle what the DeathTrigger does to the player:
        
        #region Methods:
        // When the player hits our trigger:
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                Debug.Log("Respawned");
                other.gameObject.transform.position = new Vector2(-35f, 1);
            }
            
        }

        #endregion
    }
}
