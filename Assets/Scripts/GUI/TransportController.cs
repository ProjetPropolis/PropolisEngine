using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TransportController : MonoBehaviour {

    public Text timUi;
	// Update is called once per frame
	void Update () {
        timUi.text = System.DateTime.Now.ToString();
    }
}
