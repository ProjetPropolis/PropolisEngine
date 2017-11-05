using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuController : MonoBehaviour {

	public Animator menuAnimator;
	public Text btnArrowText;

	void Start () {
		menuAnimator.SetBool("showBool", true);	
		updateArrow ();
	}

	public void updateBool() {
		var newBool = !menuAnimator.GetBool("showBool");
		menuAnimator.SetBool ("showBool", newBool);
		updateArrow();
	}

	private void updateArrow() {
		if (menuAnimator.GetBool("showBool") == true) {
			btnArrowText.text = ">";
		} else {
			btnArrowText.text = "<";
		}
	}
}
