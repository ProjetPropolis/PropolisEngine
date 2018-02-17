using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Propolis;
using UnityOSC;
using System.Net;
using UnityEngine.UI;

public class AbstractGroup : MonoBehaviour {

    public int ID;

    [SerializeField]
    public List<AbstractItem> ChildItemsList;
    public AbstractGameController parentGameController;
    public string DataType;
    [SerializeField]
    public OSCClient OSC;
    private bool _IsLocked;
    public GameObject EnvironnementalRepresentation;
    public bool IsPlayingAnimation {

        get
        {

           return  _isPlayingAniamation;
        }

        set
        {
            if (value)
                ChildItemsList.ForEach(x => x.BlockDectection());
            else
                ChildItemsList.ForEach(x => x.RestoreDectection());
            _isPlayingAniamation = value;
        }



    }
    public Text IDDisplay;
    private bool _isPlayingAniamation = false;
    public bool IsLocked
    {
        get { return _IsLocked; }
        set {
            _IsLocked = value;
          

        }
    }
    private GameObject Shield;


    // Use this for initialization
    void Start () {
        //IsPlayingAnimation = false;
        Shield = GameObject.Find("Shield");
        IsLocked = false;
        //Osc.SetAddressHandler("/status", OnReceiveHexStatus);

        ChildItemsList = transform.GetComponentsInChildren<AbstractItem>().ToList<AbstractItem>();
        switch (DataType)
        {
            case PropolisDataTypes.HexGroup: parentGameController = GameObject.Find("Controllers").GetComponent<HiveGameController>(); break;
            case PropolisDataTypes.AtomGroup: parentGameController = GameObject.Find("Controllers").GetComponent<MolecularGameController>(); break;
        }

        
        if(DataType == PropolisDataTypes.AtomGroup)
        {
           /* transform.Rotate(0, 0, 180);
            //Patch for emery street
            if(ID == 102)
            {
                transform.Rotate(0, 0, -20);
            }*/
        }
    }

	
	// Update is called once per frame
	void Update () {
        if (EnvironnementalRepresentation != null)
        {
            if(ChildItemsList.Count == 10)
            {
                EnvironnementalRepresentation.SetActive(ChildItemsList[9].status == PropolisStatus.SHIELD_ON);
            }
            
        }

    }


    public void SetOSCSettings(string ip, int port)
    {
        try
        {
            IPAddress address;
            IPAddress.TryParse(ip,out address);
            OSC = new OSCClient(address, port);
        }catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }     
       
    }

    public void SendHexDataToHiveController(int hexID, PropolisStatus status)
    {
        parentGameController.SendCommand(String.Format("uis {0} {1} {2} {3}", DataType, ID, hexID, (int)status));
    }


    //@TODO replace by new osc
    //void OnReceiveHexStatus(OscMessage message)
    //{

    //    try
    //    {
    //        AbstractItem item = ChildItemsList.First(x => x.ID == Convert.ToInt32(message.values[0]));
    //        parentGameController.ProcessUserInteraction(item, (PropolisUserInteractions)Convert.ToInt32(message.values[1]));


    //        Debug.Log(String.Format("received  from {0}:  {1} {2}", ID, message.values[0], message.values[1 ]));
    //    }
    //    catch (Exception)
    //    {
    //        Debug.LogError("Error received ID :" + message.values[0].ToString());
    //    }


    //}

    public int CountChildrenWithStatus(PropolisStatus status)
    {
        try
        {
            return ChildItemsList.Where(x => x.Status == status).Count();
        }
        catch
        {
            return 0;
        }

    }

    public int CountChildrenWithStatus(PropolisStatus[] status)
    {
        try
        {
            int count = 0;
            foreach (var s in status)
            {
                count += CountChildrenWithStatus(s);
            }
            return count;
        }
        catch
        {
            return 0;
        }

    }

    public void Refresh()
    {
        ChildItemsList.ForEach(x=>x.Refresh());
    }
}
