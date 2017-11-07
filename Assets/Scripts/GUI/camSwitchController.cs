using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camSwitchController : MonoBehaviour {

	private GameObject[] cams;

	public void Start() {
		cams = GameObject.FindGameObjectsWithTag("aView");
		switchCam("Ruche");
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
