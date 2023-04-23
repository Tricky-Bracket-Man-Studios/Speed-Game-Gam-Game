using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedGame.Intractables.UI
{
    public class Buttons : MonoBehaviour
    {
        // Definition: 
        // This class will handle all the logic for the UI buttons.

        public string DemoScene;

        public void GotoDemoScene()
        {
            SceneManager.LoadScene(DemoScene);
        }
    }
}
