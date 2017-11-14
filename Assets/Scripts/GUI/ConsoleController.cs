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


	private List<string> consoleHistory;
	private int historyIndex;

	public void Awake() {
		consoleHistory = new List<string>();
    }

    void OnEnable()
    {
        consoleText.text = PropolisManager.ConsoleLog;
    }

    public void sendValueToConsole(string newLine) {
		consoleinputField.placeholder.GetComponent<Text>().text = "/Enter command here...";
        consoleinputField.text = PropolisManager.SendCommand(consoleinputField.text);
        consoleText.text = PropolisManager.ConsoleLog;
		consoleHistory.Add(newLine);
		historyIndex = consoleHistory.Count;
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

	// KEYBOARD SHORTCUTS
	public void Update() {

		    //SEND TO CONSOLE ON ENTER KEY
		    if (Input.GetKeyDown("return")) {
			    addFromField();
			    EventSystem.current.SetSelectedGameObject(consolefieldObj,null);
		    } 

		    //UP AND DOWN ON HISTORY
		    if (Input.GetKeyDown("up")) {
			    moveInHistory ("up");
		    } 

		    if (Input.GetKeyDown("down")) {
			    moveInHistory("down");
		    } 

		    //IF NOT ENTER KEY FOCUS ON TEXT FIELD
		    if(!Input.GetKeyDown("return")){
			    EventSystem.current.SetSelectedGameObject(consolefieldObj,null);
		    }


    }
		
	//TERMINAL STYLE UP AND DOWN KEY TO NAVIGATE IN COMMAND HISTORY

	public void moveInHistory(string direction) {
		
		if (historyIndex != 0) {
			if (direction == "up") {
				historyIndex--;
			} 
			if (direction == "down") {
				historyIndex++;
			}
			if (historyIndex >= consoleHistory.Count) {
				historyIndex = consoleHistory.Count - 1;
			} 
			if (historyIndex <= 0) {
				historyIndex = 0;
				consoleinputField.text = "";
			}
			consoleinputField.text = consoleHistory [historyIndex];
			consoleinputField.MoveTextEnd(false);
		}
	}
	//SCROLL TO BOTTOM OF CONOSOLE

	public void ScrollToBottom()
	{
		//thisone.normalizedPosition = new Vector2(0, 0);
	}
}
