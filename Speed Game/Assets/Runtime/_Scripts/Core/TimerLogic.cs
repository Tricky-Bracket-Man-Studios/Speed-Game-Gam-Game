using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedGame.Core
{
    public class TimerLogic : MonoBehaviour
    {
        // Definition: 
        // This class will handle all the code for timers and stop watches.

        //public static TimerLogic Instance;
        //void Awake() => Instance = this;

        //Varibles:
        public enum Types 
        {
            None,
            Timer,
            StopWatch,
        }

        [Header("Timer Stats:")] 
        [SerializeField] private Types _currentType = Types.None;
        [SerializeField] private float _currentTime = 120f;
        [SerializeField] private float _startingTime = 120f;

        [SerializeField] private int _currentSec;
        [SerializeField] private int _currentMin;

        void Update()
        {
            // Calculating mins and secs from secconds
            _currentMin = (int)_currentTime / 60;
            _currentSec = ((int)_currentTime - (_currentMin * 60));
            
            switch (_currentType)
            {
                case(TimerLogic.Types.Timer):
                    _currentTime -= (1 * Time.deltaTime);
                    break;
                case(TimerLogic.Types.StopWatch):
                    _currentTime += (1 * Time.deltaTime);
                    break;
            }
            // If the timer is below zero, stop counting down.
            if(_currentTime <= 0)
            {
                _currentTime = 0f;
            }
        }
        // This Starts a watch:
        public void StartWatch(Types type, float time)
        {
            _startingTime = time;
            _currentTime = _startingTime;
            _currentType = type;
        }
        // This adds Time to a watch
        public void AddTimeInSecs(float time)
        {
            _currentTime += time;
        }
        // Getters
        public int GetSecs()
        {
            return (int)_currentSec;
        }
        public int GetMins()
        {
            return (int)_currentMin;
        }
        public Types GetTimerType()
        {
            return _currentType;
        }
        public int GetcurrentTime()
        {
            return (int)_currentTime;
        }

    }
}
