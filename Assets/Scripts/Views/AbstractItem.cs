using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Propolis;
using System.Linq;


public class AbstractItem : MonoBehaviour {

    private PropolisStatus status;

    public PropolisStatus Status {
        get
        {
            return status;
        }

        set
        {
        
         
            status = value;
            // this condition is to be remove
            if (status == PropolisStatus.ON && TimeToLive <= 0)
            {
                TimeToLive = TimeAlive;
            }
            ChangeColor();
            SendOscMessage("/hex", ID, (int)status);
            
            
        }
    }
   
    public int ID;
    public float lifeTime;
    public bool isCircled;
    public Collider2D otherHex;
    public OSC osc;
    public AbstractGroup ParentGroup;
    public PropolisData propolisData;

    private Material material;

    public float TimeAlive = 3.0f;
    public float TimeToLive = 0.0f;
    private void Start()
    {
        ParentGroup = transform.parent.GetComponent<AbstractGroup>();
        propolisData = PropolisData.Instance;
        material = GetComponent<Renderer>().material;
		osc = transform.parent.gameObject.GetComponent<OSC>();
        if(ParentGroup.DataType == PropolisDataTypes.HexGroup)
        {
            Status = (PropolisStatus)propolisData.HexGroupList.First(x => x.ID == ParentGroup.ID).Childrens.First(x => x.ID == ID).Status;
        }
        else
        {
            Status = (PropolisStatus)propolisData.HexGroupList.First(x => x.ID == ParentGroup.ID).Childrens.First(x => x.ID == ID).Status;
        }
    

    }

    private void OnEnable()
    {

    }

    void ChangeColor()
    {
        switch (Status)
        {

            case PropolisStatus.OFF: material.color = PropolisColors.Dark; break;
            case PropolisStatus.ON: material.color = PropolisColors.Yellow; break;
            case PropolisStatus.CORRUPTED: material.color = PropolisColors.Purple; break;
            case PropolisStatus.CLEANSER: material.color = PropolisColors.Blue; break;
            case PropolisStatus.ULTRACORRUPTED: material.color = PropolisColors.Red; break;
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

	private void Update()
	{
        
		//this code is to be remove
		if (status == PropolisStatus.ON) {
			TimeToLive -= Time.deltaTime;

			if (TimeToLive <= 0) {
                ParentGroup.SendHexDataToHiveController(ID, PropolisStatus.CORRUPTED);

            }
		}
	}

   //DEPRACIATED NOW USING MOUSE CONTROLLER TO GET CLICKS ON HEX
   // private void OnMouseOver()
   //{
   //    if(Input.GetMouseButtonDown(1))
   //     {
   //         ParentGroup.SendHexDataToHiveController(ID, PropolisStatus.ON);
   //    }
   // }

    private void SendOscMessage(string address, int value, int value2)
    {

        OscMessage message = new OscMessage();

        message.address = address;
        message.values.Add(value);
        message.values.Add(value2);
        osc.Send(message);
    }

}
