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
        osc = transform.parent.gameObject.GetComponent<OSC>();
        material = GetComponent<Renderer>().material;
        status = PropolisStatus.ON;

        SendDataToTouchDesigner();

    }

    public void SendDataToTouchDesigner()
    {
        
        OscMessage message = new OscMessage();

        SendOscMessage("/hex", ID, (int)Status);

    }


    private void SendOscMessage(string address, int value, int value2)
    {
       // Debug.Log("sending osc: " + address + value.ToString() + " " + value2.ToString());

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
