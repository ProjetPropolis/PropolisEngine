using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Propolis;

public class AbstractGroup : MonoBehaviour {

    public OSC Osc;
    public int ID;

    [SerializeField]
    public List<AbstractItem> ChildHexsList;
    public AbstractGameController parentGameController;
    public string DataType;

    // Use this for initialization
    void Start () {
        Osc.SetAddressHandler("/status", OnReceiveHexStatus);
        ChildHexsList = transform.GetComponentsInChildren<AbstractItem>().ToList<AbstractItem>();
        parentGameController = GameObject.Find("Controllers").GetComponent<AbstractGameController>();
    }
	
	// Update is called once per frame
	void Update () {
        

    }

    public void SendHexDataToHiveController(int hexID, PropolisStatus status)
    {
        parentGameController.SendCommand(String.Format("uis {0} {1} {2} {3}", DataType, ID, hexID, (int)status));
    }

    void OnReceiveHexStatus(OscMessage message)
    {
        Debug.Log("received");
        parentGameController.SendItemData(ID, Convert.ToInt32(
            message.values[0]),
            (PropolisStatus)Convert.ToInt32(message.values[1])
        );
    }
}
