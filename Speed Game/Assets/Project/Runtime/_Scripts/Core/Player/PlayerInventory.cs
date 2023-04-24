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
            Powerups_LeadBoots,
            Powerups_JumpBoots,
            Items_Gem,
        }
        [SerializeField] private Items[] _inventory;
        [SerializeField] private GameObject _storedPowerup;
        [SerializeField] private Items _currentItem;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _defaultClip;
        [SerializeField] private AudioClip GemAudioClip;
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
                //UpdateInventory();
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
                    DropPowerup();
                    break;
                case(Items.Items_Gem):
                    _audioSource.clip = _defaultClip;
                    _audioSource.Play(0);
                    DropPowerup();
                    break;
                case(Items.Powerups_JumpBoots):
                    _controller.DisableBoostJump();
                    DropPowerup();
                    break;
                case(Items.Powerups_LeadBoots):
                    _controller.DisableLeadJump();
                    DropPowerup();
                    break;
            }

            switch(_inventory[0])
            {
                case(Items.Powerups_PunchingGlove):
                    _controller.EnableDash();
                    break;
                case(Items.Powerups_JumpBoots):
                    _controller.EnableBoostJump();
                    break;
                case(Items.Powerups_LeadBoots):
                    _controller.EnableLeadJump();
                    break;
                case(Items.Items_Gem):
                    _audioSource.clip = GemAudioClip;
                    _audioSource.Play(0);
                    break;
            }
        }
        private void DropPowerup()
        {
            var newPowerup = Instantiate(_storedPowerup);
            newPowerup.transform.position = transform.position;
            newPowerup.SetActive(true);
            Destroy(_storedPowerup);
        }
        // Public functions:
        // Adds new item to inventory:
        public void AddToInventory(Items item)
        {
            _inventory[0] = item;
            UpdateInventory();
        }
        public void AddStoredPowerup(GameObject powerup)
        {
            _storedPowerup = powerup;
        }
        public Items GetCurrentItem()
        {
            return _currentItem;
        }
        
    }
}
