using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Propolis;

public class mouseUiController : MonoBehaviour {

    public string mouseState = "default";
    public Texture2D cursorTextureCreate, cursorTextureDelete;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = new Vector2(80, 80);
    public PropolisManager PropolisManager;
    public Transform uiCanvas;

    private bool inCongif = false;

    void OnGUI() {

        if (mouseState != "default")
        {
            if (mouseState == "create")
            {
                Cursor.SetCursor(cursorTextureCreate, Vector2.zero, cursorMode);

            }

            if (mouseState == "delete")
            {
                Cursor.SetCursor(cursorTextureDelete, hotSpot, cursorMode);
            }

            if (mouseState == "edit")
            {
                Cursor.SetCursor(cursorTextureCreate, hotSpot, cursorMode);
            }

        } else {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }


        if (Input.GetMouseButtonDown(0))
        {

            if (mouseState != "default") {

                inCongif = true;

                GameObject configUI = Instantiate(Resources.Load("UI/InfoPanelConfig"), Input.mousePosition, Quaternion.identity) as GameObject;
                configUI.transform.SetParent(uiCanvas);

                if (mouseState == "create")
                {
                    StartCoroutine(WaitTosend(configUI,mouseState, Input.mousePosition));
                }

                if (mouseState == "delete")
                {

                }
                mouseState = "default";
            }
        }
    }

    IEnumerator WaitTosend(GameObject uiConfig,string lastState,Vector3 mouseposition)
    {
        while (true)
        {
            if (Input.GetKeyDown("e"))
            {
                if (lastState == "create")
                {
                    var ID = GameObject.Find("InputFieldID").GetComponent<InputField>().text;
                    var IP = GameObject.Find("InputFieldIP").GetComponent<InputField>().text;
                    var portIn = GameObject.Find("InputFieldPortIN").GetComponent<InputField>().text;
                    var portOut = GameObject.Find("InputFieldPartOut").GetComponent<InputField>().text;

                    var worldPos = Camera.main.ScreenToWorldPoint(mouseposition);

                    var command = "CREATE HEXGROUP " + ID + " " + worldPos.y + " " + worldPos.x + " " + IP + " " + portIn + " " + portOut;


                    PropolisManager.SendCommand(command);

                    Destroy(uiConfig);
                }

                yield break;
           
            }
         yield return null;
        }
    }

public void setState(string fromUi)
    {
        mouseState = fromUi;
    }
  
}
