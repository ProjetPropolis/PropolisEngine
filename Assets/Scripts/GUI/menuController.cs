using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuController : MonoBehaviour {

	public Animator menuAnimator;
	public Text btnArrowText;

	void Start () {
		menuAnimator.SetBool("showBool", false);	
		updateArrow ();
    }

	void Update(){
		if (Input.GetKeyDown("escape")) {
			updateBool ();
		}
	}


	public void updateBool() {
		var newBool = !menuAnimator.GetBool("showBool");
		menuAnimator.SetBool ("showBool", newBool);
		updateArrow();
	}

	private void updateArrow() {
		if (menuAnimator.GetBool("showBool") == true) {
			btnArrowText.text = "<";
		} else {
			btnArrowText.text = ">";
		}
	}
}
