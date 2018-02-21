using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class VisibilityToggle : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.SetActive(!PropolisData.Instance.IsGamePlaying);
	}
}
