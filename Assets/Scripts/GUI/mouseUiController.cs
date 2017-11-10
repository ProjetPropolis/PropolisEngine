using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                Cursor.SetCursor(cursorTextureCreate, hotSpot, cursorMode);
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
                    StartCoroutine(WaitTosend(configUI,mouseState, Input.mousePosition, "lol", "lol", "lol", "lol"));
                }

                if (mouseState == "delete")
                {

                }
                mouseState = "default";
            }
        }
    }

    IEnumerator WaitTosend(GameObject uiConfig,string lastState,Vector3 mouseposition,string id,string ip,string portIn,string portOut)
    {
        while (true)
        {
            if (Input.GetKeyDown("e"))
            {
                if (lastState == "create")
                {
                    var createString = "CREATE HEXGROUP " + id + " " + mouseposition.x + " " + mouseposition.y + " " + ip + " " + portIn + " " + portOut;
                    Destroy(uiConfig);
                    print(createString);
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
