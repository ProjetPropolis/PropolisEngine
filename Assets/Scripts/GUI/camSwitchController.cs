using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camSwitchController : MonoBehaviour {

	public void Start() {
		switchCam("Ruche");
	}

	//ALL OFF
	public void switchCam(string toActivate) {
		var views = GameObject.FindGameObjectsWithTag("aView");
		foreach (var view in views) {
			view.SetActive(false);
		}
	//SELECTED ON
		GameObject.Find (toActivate).SetActive (true);
	}

}
