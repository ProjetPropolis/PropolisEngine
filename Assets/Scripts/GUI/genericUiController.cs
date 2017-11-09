using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class genericUiController : MonoBehaviour {

	public string currentState;

	public void Start() {
		UpdateInteractables ("interactableZoneRuche");
	}

	public void UpdateInteractables (string interactableTab) {

		currentState = interactableTab;

		//CLOSE ALL
		var rucheArray = GameObject.FindGameObjectsWithTag ("interactableZoneRuche"); 
		var champsArray = GameObject.FindGameObjectsWithTag ("interactableZoneChamps"); 
		var interactablesItems = rucheArray.Concat (champsArray).ToArray ();

		foreach (var Items in interactablesItems) {
			var cRItems = Items.GetComponent<CanvasGroup> ();
			cRItems.alpha = 0.5f;
			var buttonsInItems = Items.GetComponentsInChildren<Button> ();
			foreach (var buttons in buttonsInItems) {
				buttons.interactable = false;
			}
		}

		if (interactableTab != "") {
			if (GameObject.FindGameObjectsWithTag(interactableTab) != null) {
				var interactableArray = GameObject.FindGameObjectsWithTag (interactableTab); 
				foreach (var toSelect in interactableArray) {
					toSelect.GetComponent<CanvasGroup> ().alpha = 1;
					var buttonsInselectedItems = toSelect.GetComponentsInChildren<Button> ();
					foreach (var buttonsInSelected in buttonsInselectedItems) {
						buttonsInSelected.interactable = true;
					}
				}
			}
	
		}
	}


	public void updateInteractibles() {
		UpdateInteractables(currentState);
	}
	}

