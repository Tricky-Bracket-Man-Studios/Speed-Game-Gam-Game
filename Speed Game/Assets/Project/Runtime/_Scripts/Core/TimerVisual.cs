using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpeedGame.Core
{
    public class TimerVisual : MonoBehaviour
    {
        // Definition: 
        // This class will handle all the code for timers and stop watches visuals.

        //Variables:
        [Header("Timer Visual Stats:")]
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TimerLogic _watch;
        [SerializeField] private string _secsText;
        [SerializeField] private string _minsText;

        private void Start()
        {
            _watch = gameObject.GetComponent<TimerLogic>();
        }

        private void Update()
        {
            _secsText = _watch.GetSecs().ToString();
            _minsText = _watch.GetMins().ToString();

            if(_watch.GetSecs() < 10)
            {
                _secsText = "0" + _secsText;
            }
            if(_watch.GetMins() < 10)
            {
                _minsText = "0"+ _minsText;
            }

            _timerText.text = "Time: " + _minsText + ":" + _secsText;
        }

    }
}
