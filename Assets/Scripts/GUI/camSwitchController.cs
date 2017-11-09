using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camSwitchController : MonoBehaviour {

	private GameObject[] cams;
	public string initialState = "Ruche";

	public void Start() {
		cams = GameObject.FindGameObjectsWithTag("aView");
		switchCam(initialState);
	}

	//ALL OFF
	public void switchCam(string toActivateString) {
		foreach (var view in cams) {
			if (view.transform.name != toActivateString) {
				view.SetActive (false);
			} else {				
				view.SetActive (true);
			}
		}
	}

}
