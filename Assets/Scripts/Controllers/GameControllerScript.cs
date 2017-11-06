using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Propolis;
using System.Linq;

public class GameControllerScript : MonoBehaviour {
    public float BatteryLevel = 0.0f;
    public float BatteryIncrement = .1f;
    public OSC osc;
    public PropolisManager propolisManager;
    [SerializeField]
    public List<HexGroup> HexGroupList;


	void Start () {

        HexGroupList = transform.GetComponentsInChildren<HexGroup>().ToList<HexGroup>();

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

    }


 
}
