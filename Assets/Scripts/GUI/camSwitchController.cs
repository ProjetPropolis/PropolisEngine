using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camSwitchController : MonoBehaviour {

	private GameObject[] cams;
	public string initialState = "Ruche";
    public mouseUiController MouseController;

	public void Start() {
		cams = GameObject.FindGameObjectsWithTag("aView");
		switchCam(initialState);
	}

	//ALL OFF
	public void switchCam(string toActivateString) {

        if(toActivateString == "ChampsMoléculaires") { MouseController.GroupType = "ATOMGROUP"; }
        if (toActivateString == "Ruche") { MouseController.GroupType = "HEXGROUP"; }

        foreach (var view in cams) {
			if (view.transform.name != toActivateString) {
				view.SetActive (false);
			} else {				
				view.SetActive (true);
                MouseController.currentCam = view.GetComponentInChildren<Camera>();
			}
		}
	}

}
