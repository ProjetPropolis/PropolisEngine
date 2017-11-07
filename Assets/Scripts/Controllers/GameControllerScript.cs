using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Propolis;

public class GameControllerScript : MonoBehaviour {

    List<GameObject> tuilesActives = new List<GameObject>();
    int sizeOfList;
    public float countDownTest;
    string newColor;

    public float BatteryLevel = 0.0f;
    public float BatteryIncrement = .1f;
    public OSC osc;
    public PropolisManager propolisManager;


	void Start () {
        countDownTest = 6;
        newColor = "orange";


    }


    public void IncrementBattery()
    {
        OscMessage message = new OscMessage();
        BatteryLevel += BatteryIncrement;


        message.address = "/battery";
        message.values.Add(BatteryLevel);
        osc.Send(message);
        if (BatteryLevel >= 1f)
        {
            BatteryLevel = 0.0f;
        }


                
    }
	
	void Update () {
        countDownTest = countDownTest - Time.deltaTime;
        if (countDownTest < 0)
        {
            countDownTest = 0;
            //SendToHex();
            countDownTest = 1000;
        }
    }



    public void AjoutAListe(GameObject nouvelleTuile)
    {
        tuilesActives.Add(nouvelleTuile);
        sizeOfList = tuilesActives.Count;
        Debug.Log("nombre de tuiles " + sizeOfList );

    }


 
}
