using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Propolis;

public class HexGroup : MonoBehaviour {

    public OSC Osc;
    public int ID;

    [SerializeField]
    public List<Hex> ChildHexsList;
    public HiveGameController hiveGameController;

    // Use this for initialization
    void Start () {
        Osc.SetAddressHandler("/hex", OnReceiveHexStatus);
        ChildHexsList = transform.GetComponentsInChildren<Hex>().ToList<Hex>();
        hiveGameController = GameObject.Find("Controllers").GetComponent<HiveGameController>();
    }
	
	// Update is called once per frame
	void Update () {
        

    }

    void OnReceiveHexStatus(OscMessage message)
    {
        Debug.Log("received");
        var value = message.values[0];
        ChildHexsList[Convert.ToInt32(value)].Status = (PropolisStatus)Convert.ToInt32(message.values[1]);
        //hiveGameController.SendCommand(String.Format("uis {0} {1} {2} {3}", PropolisDataTypes.HexGroup, ID, message.values[0], message.values[1]));
    }
}
