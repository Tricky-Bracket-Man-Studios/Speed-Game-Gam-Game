using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeedGame.Intractables.Objects;
using SpeedGame.Core;
using SpeedGame.Core.Player;
using UnityEngine.SceneManagement;

public class IntroPuzzle : MonoBehaviour
{
    //Definition:
    // This will link our puzzle together

    [SerializeField] private PressurePlate pressurePlate1;
    [SerializeField] private DoorLogic door1;
    [SerializeField] private TimerLogic timer;
    [SerializeField] private PlayerInventory player;

    private bool TimerSet = false;

    private bool playerExited = false;

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D()
    {
        if(pressurePlate1.GetPlateState())
        {
            playerExited = true;
        }
        
    }

    private void Update()
    {
        if(pressurePlate1.GetPlateState())
        {
            if(!TimerSet)
            {
                timer.StartWatch(TimerLogic.Types.Timer, 120);
                TimerSet = true;
            }

            var badEnding = timer.GetCurrentTime() == 0f;
            if(playerExited)
            {
                var trueEnding = (player.GetCurrentItem() == PlayerInventory.Items.Items_Gem) && timer.GetCurrentTime() != 0f;
                
                var ending = timer.GetCurrentTime() != 0f && player.GetCurrentItem() != PlayerInventory.Items.Items_Gem;
                
                if(trueEnding)
                {
                    SceneManager.LoadScene("EndScreen");
                }
                else if(ending)
                {
                    SceneManager.LoadScene("End");
                }
                
            }
            else if (badEnding)
            {
                SceneManager.LoadScene("BadEnd");
            }
        }

    }

}