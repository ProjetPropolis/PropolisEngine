using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Propolis;

public class DiffucultyToggler : MonoBehaviour {
    public GameController gameController;
    public Image material;
    public Text text;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ProcessDifficultyToggle()
    {   
        gameController.ToggleDifficulty();
        if(gameController.DifficultyMode == GameController.PropolisDifficultyMode.Auto)
        {
            material.color = PropolisColors.Green;
            text.text = "AUTO";

        }
        else
        {
            material.color = PropolisColors.Red;
            text.text = "MANUAL";
        }
        
    }


}
