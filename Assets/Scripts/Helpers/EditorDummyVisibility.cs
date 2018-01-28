using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorDummyVisibility : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.SetActive(Debug.isDebugBuild);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
