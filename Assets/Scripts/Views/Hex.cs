using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Propolis;


public class Hex : MonoBehaviour {

    private PropolisStatus status;

    public PropolisStatus Status {
        get
        {
            return status;
        }

        set
        {
            status = value;
            ChangeColor();
            SendOscMessage("/hex", ID, (int)status);
        }
    }
   
    public int ID;
    public float lifeTime;
    public bool isCircled;
    public Collider2D otherHex;
    public OSC osc;

    private Material material;

    public float TimeAlive = 3.0f;
    public float TimeToLive = 0.0f;
    private void Start()
    {
       

        

    }

    private void OnEnable()
    {
        material = GetComponent<Renderer>().material;
        osc = transform.parent.gameObject.GetComponent<OSC>();
        Status = PropolisStatus.OFF;
    }

    void ChangeColor()
    {
        switch (Status)
        {

            case PropolisStatus.OFF: material.color = GetColorFromHTML("#3A3459"); break;
            case PropolisStatus.ON: material.color = GetColorFromHTML("#FDE981"); break;
            case PropolisStatus.CORRUPTED: material.color = GetColorFromHTML("#EF5572"); break;
            case PropolisStatus.CLEANSER: material.color = GetColorFromHTML("#0BFFE2"); break;
        }

    }

    Color GetColorFromHTML(string hex)
    {
        Color color;

        if(ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return color;
        
    }

    public void SendDataToTouchDesigner()
    {
        
        OscMessage message = new OscMessage();

        SendOscMessage("/hex", ID, (int)Status);

    }


    private void SendOscMessage(string address, int value, int value2)
    {

        OscMessage message = new OscMessage();

        message.address = address;
        message.values.Add(value);
        message.values.Add(value2);
        osc.Send(message);
    }

 

    private void OnMouseOver()
    {

            
    }

    private void Update()
    {
   
     
    }


}
