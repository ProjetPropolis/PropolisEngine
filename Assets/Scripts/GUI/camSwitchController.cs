using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camSwitchController : MonoBehaviour {

	private GameObject[] cams;
	public string initialState = "Ruche";
    public mouseUiController MouseController;
    public genericUiController genericUi;

	public void Start() {
		cams = GameObject.FindGameObjectsWithTag("aView");
		switchCam(initialState);
	}

    public void Update()
    {
     if(Input.GetKeyDown(KeyCode.R))
        {
            switchCam("Ruche");
            genericUi.UpdateInteractables("interactableZoneRuche");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            switchCam("ChampsMoléculaires");
            genericUi.UpdateInteractables("interactableZoneChamps");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            genericUi.UpdateInteractables("");
        }
    }

    //ALL OFF
    public void switchCam(string toActivateString) {



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
