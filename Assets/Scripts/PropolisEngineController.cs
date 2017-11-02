using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropolisEngineController : MonoBehaviour {

    GameObject hex;
    HexPackManager packScript;
    List<GameObject> tuilesActives = new List<GameObject>();
    int sizeOfList;
    public float countDownTest;
    private string hexPackID;


	void Start () {


    }
	
	void Update () {

    }



    public void AjoutAListe(GameObject nouvelleTuile)
    {
        tuilesActives.Add(nouvelleTuile);
        sizeOfList = tuilesActives.Count;
        Debug.Log("nombre de tuiles " + sizeOfList );

    }

	public void ResetHex() {
		
//		foreach(var Tuiles in 
	//

    void SendToHex()
    {
        GameObject hexObject = GameObject.FindWithTag("Hex7");

        if (hexObject != null)
        {

            if (hexObject.GetComponent<HexControllerScript>().IsActive == true)
            {   
                hexPackID = hexObject.GetComponent<HexControllerScript>().packID;
                GameObject HexPackManagerObject = GameObject.FindWithTag("HexPackManager");
                if (HexPackManagerObject != null)
                {
                    packScript = HexPackManagerObject.GetComponent<HexPackManager>();
                }

                packScript.RemoveFromPack(hexObject, hexPackID);

            }

            
            if (hexObject.GetComponent<HexControllerScript>().lifeTime == 0)
            {
                hexObject.GetComponent<HexControllerScript>().lifeTime = 300;
            }


            hexObject.GetComponent<HexControllerScript>().IsActive = true;
            hexObject.GetComponent<HexControllerScript>().color = newColor;
            //packScript.AddToPack(hexObject, newColor);
            hexObject.GetComponent<HexControllerScript>().Verify();
        }

        if (hexObject == null)
        {
            Debug.Log("Cannot find Hexagone");
        }

    }
}
