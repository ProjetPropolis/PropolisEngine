using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Linq;
public class RecipeUIController : MonoBehaviour {

    public int recipeID = 0;
    private CanvasRenderer material;

    // Use this for initialization
    void Start () {
        material = GetComponent<CanvasRenderer>();
        StartCoroutine(UpdateRecipeUI());
    }


    IEnumerator UpdateRecipeUI()
    {
        while(true)
        {
            if (PropolisData.Instance.RecipeStack != null)
            {
                if (PropolisData.Instance.RecipeStack.Count == 3&& recipeID >=0 && recipeID <=2)
                {
                    int recipeElement = PropolisData.Instance.RecipeStack.ToArray()[1].GetItem(recipeID);

                    switch ((PropolisStatus)recipeElement)
                    {
                        case PropolisStatus.RECIPE1: material.SetColor(PropolisColors.Orange); break;
                        case PropolisStatus.RECIPE2: material.SetColor(PropolisColors.Fushia); break;
                        case PropolisStatus.RECIPE3: material.SetColor(PropolisColors.DarkBlue); break;
                    }
                }
            }

            yield return new WaitForSeconds(0.7f);
        }
    }
    

    private void OnGUI()
    {
       
    }

    private void OnDestroy()
    {
        StopCoroutine(UpdateRecipeUI());
    }

    // Update is called once per frame
    void Update () {
		
	}
}
