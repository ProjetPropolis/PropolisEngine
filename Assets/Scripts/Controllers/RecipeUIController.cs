using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class RecipeUIController : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}

    private void OnGUI()
    {
        if(PropolisData.Instance.LastEvent.Action == PropolisActions.PushRecipe && PropolisData.Instance.RecipeStack !=null)
        {
            if(PropolisData.Instance.RecipeStack.Count == 3)
            {

            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
