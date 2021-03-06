﻿using System.Collections;
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
    public GameController GameController;
    public Transform uiCanvas;
    public float keyMoveSpeed = 0.01f;
    public Camera currentCam;
    public tabsController tabCtl;
    public LayerMask layer_mask_Game,layer_mask_Ui;
    public string GroupType;
    public void SetGroupType(string type)
    {
        GroupType = type;
    }

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

            if (mouseState == "edit" || mouseState == "refresh" || mouseState =="unlock" || mouseState == "superClean")
            {
                Cursor.SetCursor(cursorTextureCreate, hotSpot, cursorMode);
            }

        } else {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }

    


        if (Input.GetMouseButtonDown(0))
        {

            if (mouseState != "default") {

                tabCtl.sensibleToKeypress = false;

                if (mouseState == "create")
                {
                    GameObject configUI = Instantiate(Resources.Load("UI/InfoPanelConfig"),currentCam.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity) as GameObject;
                    configUI.transform.SetParent(uiCanvas,false);
                    StartCoroutine(WaitTosend(configUI,null,mouseState, Input.mousePosition));
                }
                 
                if (mouseState == "delete")
                {
                    var deletecommand = "";
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero,Mathf.Infinity,layer_mask_Ui.value);
                    if (hit.collider != null)
                    {

                        //Debug.Log(hit.collider.name);

                        if(gameObject.GetComponent<AbstractGroup>().ID != null) { 
                            deletecommand = "DELETE " + GroupType + " " + hit.collider.gameObject.GetComponent<AbstractGroup>().ID;
                        }
                        PropolisManager.SendCommand(deletecommand);
                    }
                }


                if (mouseState == "refresh")
                {
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layer_mask_Ui.value);
                    if (hit.collider != null)
                    {

                        AbstractGroup group = hit.collider.gameObject.GetComponent<AbstractGroup>();
                        if (group != null)
                        {
                            group.Refresh();
                            Cursor.SetCursor(null, Vector2.zero, cursorMode);
                        }
                    }
                }

                if (mouseState == "unlock")
                {
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layer_mask_Ui.value);
                    if (hit.collider != null)
                    {

                        AbstractGroup group = hit.collider.gameObject.GetComponent<AbstractGroup>();
                        if (group != null)
                        {
                            group.IsPlayingAnimation = false;
                            group.ChildItemsList.ForEach(x=>x.RestoreDectection());
                            Cursor.SetCursor(null, Vector2.zero, cursorMode);
                        }
                    }
                }

                if (mouseState == "edit")
                {
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layer_mask_Ui.value);
                    if (hit.collider != null)
                    {

                        var StolenInPort = 123456; // dummy in port to prevent a code's refactor
                        var StolenOutPort = hit.collider.gameObject.GetComponent<AbstractGroup>().OSC.Port;
                        var StolenId = hit.collider.gameObject.GetComponent<AbstractGroup>().ID;
                        var StolenIp = hit.collider.gameObject.GetComponent<AbstractGroup>().OSC.ClientIPAddress.ToString();

                        GameObject configUI = Instantiate(Resources.Load("UI/InfoPanelConfig"), currentCam.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity) as GameObject;
                        configUI.transform.SetParent(uiCanvas,false);

                        GameObject.Find("InputFieldID").gameObject.GetComponent<InputField>().text = StolenId.ToString();
                        GameObject.Find("InputFieldIP").gameObject.GetComponent<InputField>().text = StolenIp.ToString();
                        GameObject.Find("InputFieldPartOut").gameObject.GetComponent<InputField>().text = StolenOutPort.ToString();
                        GameObject.Find("InputFieldPortIN").gameObject.GetComponent<InputField>().text = StolenInPort.ToString();

                        StartCoroutine(WaitTosend(configUI, hit.collider.gameObject, mouseState, Input.mousePosition));
                    }
                }

                if (mouseState == "superClean")
                {
                    Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layer_mask_Game.value);
                    if (hit.collider != null)
                    {
                        var abstractGroup = hit.collider.transform.parent.gameObject.GetComponent<AbstractGroup>();
                        if (abstractGroup == null)
                        {
                            abstractGroup = hit.collider.transform.parent.parent.gameObject.GetComponent<AbstractGroup>();
                        }
                        var abstractItem = hit.collider.gameObject.GetComponent<AbstractItem>();
                        var GroupType = abstractGroup.DataType;

                        if (abstractItem != null && PropolisData.Instance.IsGamePlaying)
                        {
                            GameController.SpawnSuperCleanser(abstractItem);
                        }

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

                if (lastState == "edit")
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        Vector3 positionTemp = toedit.transform.position;
                        positionTemp.y += keyMoveSpeed;
                        toedit.transform.position = positionTemp;
                    }

                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        Vector3 positionTemp = toedit.transform.position;
                        positionTemp.y -= keyMoveSpeed;
                        toedit.transform.position = positionTemp;
                }

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        Vector3 positionTemp = toedit.transform.position;
                        positionTemp.x -= keyMoveSpeed;
                        toedit.transform.position = positionTemp;
                }

                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        Vector3 positionTemp = toedit.transform.position;
                        positionTemp.x += keyMoveSpeed;
                        toedit.transform.position = positionTemp;
                }
           }

                if (Input.GetKeyDown(KeyCode.Return))
                {

                    if (lastState == "create")
                    {
                        var ID = GameObject.Find("InputFieldID").GetComponent<InputField>().text;
                        var IP = GameObject.Find("InputFieldIP").GetComponent<InputField>().text;
                        var portIn = GameObject.Find("InputFieldPortIN").GetComponent<InputField>().text;
                        var portOut = GameObject.Find("InputFieldPartOut").GetComponent<InputField>().text;

                        var worldPos = currentCam.ScreenToWorldPoint(mouseposition);

                        var command = "CREATE " + GroupType + " " + ID + " " + worldPos.x + " " + worldPos.y + " " + IP + " " + portIn + " " + portOut;


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

                        var worldPos = toedit.transform.position;

                        var command = "UPDATE " + GroupType + " " + ID + " " + worldPos.x + " " + worldPos.y + " " + IP + " " + portIn + " " + portOut;

                        print(command);

                        PropolisManager.SendCommand(command);

                        Destroy(uiConfig);

                        tabCtl.sensibleToKeypress = true;
                    }


                yield break;
           
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(uiConfig);
                yield break;
            }
         yield return null;
        }
    }

    public void setState(string fromUi)

    {
        mouseState = fromUi;
    }

    public void PullOffActionGame()
    {
        Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layer_mask_Game.value);
        if (hit.collider != null)
        {
            var abstractGroup = hit.collider.transform.parent.gameObject.GetComponent<AbstractGroup>();
            if (abstractGroup == null)
            {
                abstractGroup = hit.collider.transform.parent.parent.gameObject.GetComponent<AbstractGroup>();
            }
            var abstractItem = hit.collider.gameObject.GetComponent<AbstractItem>();
            var GroupType = abstractGroup.DataType;

            if (abstractItem != null)
            {
                GameController.ProcessUserInteraction(GroupType, abstractItem,PropolisUserInteractions.PULL_OFF);
            }

        }
    }

    public void ClickInGame()
    {
        Vector2 worldPoint = currentCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity,layer_mask_Game.value);
        if (hit.collider != null)
        {
            var abstractGroup = hit.collider.transform.parent.gameObject.GetComponent<AbstractGroup>();
            if(abstractGroup == null)
            {
                abstractGroup = hit.collider.transform.parent.parent.gameObject.GetComponent<AbstractGroup>();
            }
            var abstractItem = hit.collider.gameObject.GetComponent<AbstractItem>();
            var GroupType = abstractGroup.DataType;

            if(abstractItem != null)
            {
                GameController.ProcessUserInteraction(GroupType, abstractItem, PropolisUserInteractions.PRESS);
            }

        }


    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            PullOffActionGame();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClickInGame();
        }
    }
}