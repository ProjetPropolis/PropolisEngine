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

    // Use this for initialization
    void Start () {
        Osc.SetAddressHandler("/hex", OnReceiveHexStatus);
        ChildHexsList = transform.GetComponentsInChildren<Hex>().ToList<Hex>();
    }
	
	// Update is called once per frame
	void Update () {
        

    }

    void OnReceiveHexStatus(OscMessage message)
    {
        Debug.Log("received");
        var value = message.values[0];
        ChildHexsList[Convert.ToInt32(value)].Status = (PropolisStatus)Convert.ToInt32(message.values[1]);
    }
}
