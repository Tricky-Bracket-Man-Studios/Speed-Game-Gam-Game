using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedGame.Intractables.Objects

{
    public class DoorLogic : MonoBehaviour
    {
        // Definition:
        // This class manages the Door logic:

        // Variables: 
        [SerializeField] private bool _isDoorOpen = false;
        [SerializeField] private Vector3 _doorClosedPos;
        [SerializeField] private Vector3 _doorOpenPos;
        [SerializeField] private float _doorSpeed = 0.3f;

        // Private Functions:
        private void Awake()
        {
            _doorClosedPos = transform.position;
            _doorOpenPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        }

        private void Update()
        {
            if(_isDoorOpen)
            {
                OpenDoor();
            }
            else if(!_isDoorOpen)
            {
                CloseDoor();
            }
        }

        private void OpenDoor()
        {
            if(transform.position != _doorOpenPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, _doorOpenPos, _doorSpeed * Time.deltaTime);
            }
        }

        private void CloseDoor()
        {
            if(transform.position != _doorClosedPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, _doorClosedPos, _doorSpeed * Time.deltaTime);
            }
        }

        // Public functions
        public void SetDoorOpenPos(float x, float y)
        {
            _doorOpenPos.x = x;
            _doorOpenPos.y = y;
        }
        public void SetDoorClosePos(float x, float y)
        {
            _doorClosedPos.x = x;
            _doorClosedPos.y = y;
        }
        public void SetDoorState(bool state)
        {
            _isDoorOpen = state;
        }

        //Getters
        public Vector3 GetDoorOpenPos()
        {
            return _doorOpenPos;
        }
        public Vector3 GetDoorClosePos()
        {
            return _doorClosedPos;
        }
        public bool GetDoorState()
        {
            return _isDoorOpen;
        }
        

    }
}
