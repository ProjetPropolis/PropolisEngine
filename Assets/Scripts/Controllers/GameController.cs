using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Propolis;

public class GameController : MonoBehaviour {

    List<GameObject> tuilesActives = new List<GameObject>();
    public PropolisManager propolisManager;
    public AbstractGameController hiveGameController;


	void Start () {


    }

    public void SendCommand(string command)
    {
        propolisManager.SendCommand(command);
    }

    public void UpdateFromModel()
    {
        hiveGameController.UpdateFromModel();
    }

    
	void Update () {
    
    }

 
}
