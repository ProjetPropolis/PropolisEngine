using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour {

	public Text consoleText;
	public InputField consoleinputField;

	public void sendValueToConsole(string newLine) {
		consoleinputField.placeholder.GetComponent<Text>().text = "/hex all on";
		if (verifyString (newLine) == true) {
			consoleText.text += newLine + "\n";
		} else {
			consoleinputField.placeholder.GetComponent<Text>().text = "INVALID COMMAND";
			consoleinputField.text = "";
		}
	}

	public void addFromField() {
		sendValueToConsole (consoleinputField.text);
		consoleinputField.text = "";
	}

	public void clearConsole() {
		consoleText.text = "";
	}
		
	private bool verifyString(string consoleLineToCheck) {
		if (Regex.IsMatch (consoleLineToCheck, "/hex", RegexOptions.IgnoreCase)) {
			return true;
		} else {
			return false;		
		}
	}
}
