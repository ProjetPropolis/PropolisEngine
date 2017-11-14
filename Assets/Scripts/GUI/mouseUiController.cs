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

    public GameObject dummyHexGroup;
    public Camera currentCam;
    public tabsController tabCtl;

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
                tabCtl.sensibleToKeypress = false;

                if (mouseState == "create")
                {
                    GameObject configUI = Instantiate(Resources.Load("UI/InfoPanelConfig"), Input.mousePosition, Quaternion.identity) as GameObject;
                    configUI.transform.SetParent(uiCanvas);
                    StartCoroutine(WaitTosend(configUI,null,mouseState, Input.mousePosition));
                }
                 
                if (mouseState == "delete")
                {
                    var deletecommand = "";
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                    if (hit.collider != null)
                    {
                        Debug.Log(hit.collider.name);

                        if(gameObject.GetComponent<HexGroup>().ID != null) { 
                            deletecommand = "DELETE HEXGROUP " + hit.collider.gameObject.GetComponent<HexGroup>().ID;
                        }
                        PropolisManager.SendCommand(deletecommand);
                    }
                }

                if(mouseState == "edit")
                {
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                    if (hit.collider != null)
                    {

                        var StolenInPort = hit.collider.gameObject.GetComponent<OSC>().inPort;
                        var StolenOutPort = hit.collider.gameObject.GetComponent<OSC>().outPort;
                        var StolenId = hit.collider.gameObject.GetComponent<HexGroup>().ID;
                        var stolenIp = hit.collider.gameObject.GetComponent<OSC>().outIP;

                        GameObject configUI = Instantiate(Resources.Load("UI/InfoPanelConfig"), Input.mousePosition, Quaternion.identity) as GameObject;
                        configUI.transform.SetParent(uiCanvas);

                        GameObject.Find("InputFieldID").gameObject.GetComponent<InputField>().text = StolenId.ToString();
                        GameObject.Find("InputFieldIP").gameObject.GetComponent<InputField>().text = stolenIp.ToString();
                        GameObject.Find("InputFieldPartOut").gameObject.GetComponent<InputField>().text = StolenOutPort.ToString();
                        GameObject.Find("InputFieldPortIN").gameObject.GetComponent<InputField>().text = StolenInPort.ToString();

                        StartCoroutine(WaitTosend(configUI, hit.collider.gameObject, mouseState, Input.mousePosition));
                    }
                }

                mouseState = "default";
            }
        }
    }
     
    IEnumerator WaitTosend(GameObject uiConfig ,GameObject toedit,string lastState,Vector3 mouseposition)
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

                        tabCtl.sensibleToKeypress = true;
                    }

                    if (lastState == "edit")
                    {
                        var ID = GameObject.Find("InputFieldID").GetComponent<InputField>().text;
                        var IP = GameObject.Find("InputFieldIP").GetComponent<InputField>().text;
                        var portIn = GameObject.Find("InputFieldPortIN").GetComponent<InputField>().text;
                        var portOut = GameObject.Find("InputFieldPartOut").GetComponent<InputField>().text;

                        var worldPos = Camera.main.ScreenToWorldPoint(mouseposition);

                        var command = "UPDATE HEXGROUP " + ID + " " + worldPos.y + " " + worldPos.x + " " + IP + " " + portIn + " " + portOut;

                        PropolisManager.SendCommand(command);

                        Destroy(uiConfig);

                        tabCtl.sensibleToKeypress = true;
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
