using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tabsController : MonoBehaviour {
	
	public GameObject tabsContainer;
	public GameObject[] tabs;
	public Button initalPanel;
	public Button[] buttonsPanel;

	private int counterMenu;

	void Start () {
		counterMenu = 0;
		buttonsPanel = gameObject.GetComponentsInChildren<Button>();
		tabs = GameObject.FindGameObjectsWithTag("Tabs");
		UpdateSelectedTab(buttonsPanel[counterMenu]);
	}

	public void Update() {
		if (Input.GetKeyDown ("tab")) {
			switchMenuKeyboard();
		}
	}

	public void switchMenuKeyboard() {
		counterMenu ++;
		if (counterMenu >= buttonsPanel.Length) { counterMenu = 0; } 
		UpdateSelectedTab(buttonsPanel[counterMenu]);
	}

	public void UpdateSelectedTab(Button button) {
		foreach (var aTab in tabs) {
			if(aTab.name != button.transform.Find("Text").GetComponent<Text>().text) { aTab.SetActive (false); } else { aTab.SetActive(true); }
		}	
	}
}
