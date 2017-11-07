using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Propolis;

public class ConsoleController : MonoBehaviour {

	public Text consoleText;
	public InputField consoleinputField;
	public GameObject consolefieldObj;
    public PropolisManager PropolisManager;
	public ScrollRect thisone;

	public void sendValueToConsole(string newLine) {
		consoleinputField.placeholder.GetComponent<Text>().text = "/Enter command here...";
        consoleinputField.text = PropolisManager.SendCommand(consoleinputField.text);
        consoleText.text = PropolisManager.ConsoleLog;
		ScrollToBottom();
    }

	public void addFromField() {
		sendValueToConsole (consoleinputField.text);
		consoleinputField.text = "";
	}

	public void clearConsole() {
        PropolisManager.ClearConsole();
        consoleText.text = PropolisManager.ConsoleLog;
    }

	public void Update() {
		//SEND TO CONSOLE ON ENTER KEY
		if (Input.GetKeyDown("return")) {
			addFromField();
			EventSystem.current.SetSelectedGameObject(consolefieldObj,null);
		} 
		//IF NOT ENTER KEY FOCUS ON TEXT FIELD
		if(!Input.GetKeyDown("return")){
			EventSystem.current.SetSelectedGameObject(consolefieldObj,null);
		}
	}

	public void ScrollToBottom()
	{
		thisone.normalizedPosition = new Vector2(0, 0);
	}
}
