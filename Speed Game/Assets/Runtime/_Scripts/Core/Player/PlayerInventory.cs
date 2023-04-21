using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedGame.Core.Player
{
    

    public class PlayerInventory : MonoBehaviour
    {
        // Definition:
        // This class will handle the inventory code for the player.

        // Variables:
        public enum Items 
        {
            empty,
            Powerups_PunchingGlove,
        }
        [SerializeField] private Items[] _inventory;
        private Items _currentItem;
        private PlayerController _controller;

        // Private functions:
        private void Start() 
        {
            _controller = gameObject.GetComponent<PlayerController>();
            _inventory = new Items[] {0};
            _currentItem = _inventory[0];
        }
        private void Update() 
        {
            if(_inventory[0] != _currentItem)
            {
                UpdateInventory();
                _currentItem = _inventory[0];
            }    
        }
        // Updates the Inventory current item:
        private void UpdateInventory()
        {

            switch(_currentItem)
            {
                case(Items.Powerups_PunchingGlove):
                    _controller.DisableDash();
                    //TODO Add Unit Manager so that it drops the item that got switched out in this location.
                    break;
            }

            switch(_inventory[0])
            {
                case(Items.Powerups_PunchingGlove):
                    _controller.EnableDash();
                    break;
            }
        }
        // Public functions:
        // Adds new item to inventory:
        public void AddToInventory(Items item)
        {
            _inventory[0] = item;
        }
    }
}
