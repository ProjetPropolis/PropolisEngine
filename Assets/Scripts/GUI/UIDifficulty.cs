using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDifficulty : MonoBehaviour {

    public Text TextUI;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TextUI.text = Propolis.PropolisGameSettings.CurrentDifficultyMultiplier.ToString();

    }
}
