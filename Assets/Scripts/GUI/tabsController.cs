using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tabsController : MonoBehaviour {
	
	public GameObject tabsContainer;
	public GameObject[] tabs;
	public Button initalPanel;
	public Button[] buttonsPanel;
	public genericUiController uiController;
    public bool sensibleToKeypress;

    private int counterMenu;

	void Start () {
		counterMenu = 0;
		buttonsPanel = gameObject.GetComponentsInChildren<Button>();
		tabs = GameObject.FindGameObjectsWithTag("Tabs");
        sensibleToKeypress = true;
    }

	public void Update() {
        if (sensibleToKeypress != false)
        {
            if (Input.GetKeyDown ("tab")) {
			    switchMenuKeyboard();
		    }
        }
    }

	public void switchMenuKeyboard() {
		counterMenu ++;
		if (counterMenu >= buttonsPanel.Length) { counterMenu = 0; } 
		UpdateSelectedTab(buttonsPanel[counterMenu]);
	
	}

    public void UpdateSelectedTab(Button button) {
        foreach (var aTab in tabs) {
            if (aTab.name.ToLower() != button.transform.Find("Text").GetComponent<Text>().text.ToLower()) { aTab.SetActive(false); } else { aTab.SetActive(true); }
        }

		updateCheck();
	}

	public void updateCheck() {
		if (uiController.currentState != "null") {
			uiController.updateInteractibles();
		}
	}
}
