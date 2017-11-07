using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hex : MonoBehaviour {

    private bool isActive;
    public bool IsActive {

        get
        {
            return isActive;
        }
        set
        {
            if(TimeToLive <= 0.0f)
            {
                isActive = value;
                Verify();
                ChangeColor();
                SendDataToTouchDesigner();
            }
   
        }
    }


    public int hexID;
    public float lifeTime;
    public int otherPackID;
    public string otherHexColor;
    public bool newHex;
    public bool someoneOn;
    public string color;
    public bool isBad;
    public bool isCircled;
    public Collider2D otherHex;
    public OSC osc;

    private Vector2 pos2D = new Vector2();
    private List<Collider2D> ActiveHexPack;
    private Material material;

    public float TimeAlive = 3.0f;
    public float TimeToLive = 0.0f;





    private void Start()
    {
        osc = transform.parent.gameObject.GetComponent<OSC>();
        material = GetComponent<Renderer>().material;
        isBad = false;
        IsActive = false;

        pos2D = (Vector2)transform.position;
        
        if (IsActive)
        {
            Verify();
        }

        //SendDataToTouchDesigner();

    }

    public void SendDataToTouchDesigner()
    {
        
        OscMessage message = new OscMessage();

        SendOscMessage("/hex", hexID, StatusToInt(GetStatus()));

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


    int StatusToInt(string status)
    {
        int returnValue = 0;
        switch (status)
        {
            case "OFF": returnValue = 0; break;
            case "ON": returnValue = 1; break;
            case "BAD": returnValue = 2; break;
        }

        return returnValue;

    }

    string GetStatus()
    {
        string status = "OFF";
        if (isBad)
        {
            status = "BAD";
        }
        else if (IsActive)
        {
            status = "ON";
        }
        return status;
    }

    void ChangeColor()
    {
        switch (GetStatus())
        {

            case "OFF": material.color = Color.gray; break;
            case "ON": material.color = Color.white; break;
            case "BAD": material.color = Color.red; break;
        }
        
    }

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1))
        {
            IsActive = !IsActive;
        }
            
    }

    private void Update()
    {
        if (isActive)
        {
            TimeToLive -= Time.deltaTime;
            if (TimeToLive <= 0)
                IsActive = false;
        }
     
    }

    public void Verify()
    {
        Collider2D[] otherColliders;
        ActiveHexPack = new List<Collider2D>();
        otherColliders = Physics2D.OverlapCircleAll(pos2D, 2);

        for (int i = 0; i < otherColliders.Length; i++)
        {
            otherHex = otherColliders[i];

            if (hexID != otherHex.GetComponent<Hex>().hexID && otherHex.GetComponent<Hex>().IsActive == true)
            {
                ActiveHexPack.Add(otherHex);

            }

            //si une tuile dans son environnement est isBad, on veut que cette mauvaise tuile vérifie son état pour voir si elle est maintenant encerclée
            if (otherHex.GetComponent<Hex>().isBad == true && hexID != otherHex.GetComponent<Hex>().hexID)
            {
                otherHex.GetComponent<Hex>().Verify();
            }
        }

        //Savoir si une mauvaise tuile est entourée de tuiles toutes actives
        if (isBad == true)
        {
            if (ActiveHexPack.Count ==6)
            {
                isCircled = true;
                SendDataToTouchDesigner();
            }
            else
            {
                //Debug.Log("ActiveHexPack around " + hexID + "is" + ActiveHexPack.Count);
            }

        }

        Array.Clear(otherColliders, 0, otherColliders.Length);
        
    }


}
