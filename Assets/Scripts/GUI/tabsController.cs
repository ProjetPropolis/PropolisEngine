using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tabsController : MonoBehaviour {
	
	public GameObject tabsContainer;
	public GameObject[] tabs;
	public Button initalPanel;

	void Start () {
		tabs = GameObject.FindGameObjectsWithTag("Tabs");
		UpdateSelectedTab (initalPanel);
	}

	public void UpdateSelectedTab(Button button) {
		foreach (var aTab in tabs) {
			if(aTab.name != button.transform.Find("Text").GetComponent<Text>().text) { aTab.SetActive (false); } else { aTab.SetActive(true); }
		}
	}
}
