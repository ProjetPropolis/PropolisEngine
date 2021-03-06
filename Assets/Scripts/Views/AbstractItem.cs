﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Linq;
using UnityOSC;


public class AbstractItem : MonoBehaviour {

    public PropolisStatus status;
    public PropolisStatus PrevState { get; set; }
    public int i = 0;

    private Coroutine _delayedStatusCoroutine;
    private Coroutine _autoUnlockCoroutine;


    public PropolisStatus Status {
        get
        {
            return status;
        }

        set
        {
            if (_delayedStatusCoroutine != null)
                StopCoroutine(_delayedStatusCoroutine);
            PrevState = status;
            status = value;
            ChangeColor();

            if(ParentGroup.OSC != null) {

                SendOscMessage("/status", ID, (int)status);
                //if(ParentGroup.DataType == PropolisDataTypes.AtomGroup)
                //{
                //    //Debug.Log(string.Format("sending to {0} {1} status {2}",ParentGroup.ID, ID,(int)status));
                //}
            }      
            

        }
    }

    public void Refresh() {
        SendOscMessage("/status", ID, (int)status);
    }

    public void TriggerDelayedStatus(PropolisStatus status, float delay)
    {
        if (_delayedStatusCoroutine != null)
            StopCoroutine(_delayedStatusCoroutine);

        _delayedStatusCoroutine = StartCoroutine(StartDelayedStatus(status, delay));

    }

    private IEnumerator StartDelayedStatus(PropolisStatus status, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ParentGroup.parentGameController.SendItemData(ParentGroup.ID, ID, status);
    }

    public List<AbstractItem> Neighbors;
    public LayerMask ItemLayerMask;
    public PropolisStatus StatusBackup;
     

    public void CalculateNeighborsList()
    {
        Neighbors = Physics2D.OverlapCircleAll((Vector2)transform.position, NeighborsDist)
            .Where(x=>x.GetComponent<AbstractItem>() != null)
            .Select(x => x.GetComponent<AbstractItem>())
            .Where(x=>(x.ID != ID || x.ParentGroup.ID != ParentGroup.ID) && x.ParentGroup.DataType == PropolisDataTypes.HexGroup)
            .ToList<AbstractItem>();
    }

    public int ID;
    public float NeighborsDist;
    public AbstractGroup ParentGroup;
    public PropolisData propolisData;
    private Material material;
    public bool StatusLocked {

        get
        {
            return _statusLocked;
        }

        set
        {
            _statusLocked = value;

            if (_autoUnlockCoroutine !=null)
            {
                StopCoroutine(_autoUnlockCoroutine);
            }
        }
    }
    public bool IsShield = false;
    private bool _statusLocked = false;
    private void Start()
    {   
        ParentGroup = transform.parent.GetComponent<AbstractGroup>();
        if(ParentGroup == null)
        {
            ParentGroup = transform.parent.parent.GetComponent<AbstractGroup>();
        }
        propolisData = PropolisData.Instance;
        material = GetComponent<Renderer>().material;
        StatusLocked = false;
        if (ParentGroup.DataType == PropolisDataTypes.HexGroup)
        {
            Status = (PropolisStatus)propolisData.HexGroupList.First(x => x.ID == ParentGroup.ID).Childrens.First(x => x.ID == ID).Status;
            PrevState = Status;
        }
        else if (ParentGroup.DataType == PropolisDataTypes.AtomGroup)
        {
            Status = (PropolisStatus)propolisData.AtomGroupList.First(x => x.ID == ParentGroup.ID).Childrens.First(x => x.ID == ID).Status;
            PrevState = Status;
        }
    

    }

    public void AutoUnlockIn(float seconds) {
        if (_autoUnlockCoroutine != null)
            StopCoroutine(_autoUnlockCoroutine);
        StartCoroutine(StartDelayedUnlock(seconds));
            
    }

    private IEnumerator StartDelayedUnlock(float seconds) {
        yield return new WaitForSecondsRealtime(seconds);
        _statusLocked = false;
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
            case PropolisStatus.RECIPE1: material.color = PropolisColors.Orange; break;
            case PropolisStatus.RECIPE2: material.color = PropolisColors.Fushia; break;
            case PropolisStatus.RECIPE3: material.color = PropolisColors.DarkBlue; break;
            case PropolisStatus.WAVECORRUPTED: material.color = PropolisColors.Red; break;
            case PropolisStatus.CLEANSING: material.color = PropolisColors.White; break;
            case PropolisStatus.SHIELD_ON: material.color = PropolisColors.Blue; break;
            case PropolisStatus.SHIELD_OFF: material.color = PropolisColors.DarkBlue; break;
            case PropolisStatus.ULTRACORRUPTED_CLEAR_HINT: material.color = PropolisColors.White; break;
            case PropolisStatus.SLEEP_MODE: material.color = PropolisColors.White; break;
            case PropolisStatus.BLINKRECIPE3: material.color = PropolisColors.DarkBlue; break;
            case PropolisStatus.ANIM_TURQUOISE: material.color = PropolisColors.Blue; break;
            case PropolisStatus.ANIM_ORANGE: material.color = PropolisColors.Orange; break;
            case PropolisStatus.ANIM_YELLOW: material.color = PropolisColors.Yellow; break;
            case PropolisStatus.ANIM_PURPLE: material.color = PropolisColors.Purple; break;
            case PropolisStatus.ANIM_SNAKE_TURQUOISE: material.color = PropolisColors.Blue; break;
            case PropolisStatus.ANIM_BLACK: material.color = PropolisColors.Dark; break;
            case PropolisStatus.ANIM_TURQUOISE_FADE: material.color = PropolisColors.Blue; break;
            case PropolisStatus.ANIM_SNAKE_YELLOW: material.color = PropolisColors.Yellow; break;
            case PropolisStatus.ANIM_GREEN: material.color = PropolisColors.Green; break;
            case PropolisStatus.SUPER_CLEAN_GREEN: material.color =  PropolisColors.SuperCleanTrigger; break;
            case PropolisStatus.SUPER_CLEAN_TRIGGER: material.color = PropolisColors.SuperCleanTrigger; break;
            case PropolisStatus.SUPER_CLEAN_TURQUOISE: material.color = PropolisColors.SuperClean1; break;
            case PropolisStatus.SUPER_CLEAN_TURQUOISE_FADE: material.color = PropolisColors.SuperClean1; break;




        }

    }

    public int CountNeighborsWithStatus(PropolisStatus status)
    {
        try
        {
            return Neighbors.Where(x => x.status == status).Count();
        }
        catch
        {
            return 0;
        }
        
    }


    public int CountNeighborsWithStatus(PropolisStatus[] status)
    {
        try
        {
            int count = 0 ;
            foreach (var s in status)
            {
                count += CountNeighborsWithStatus(s);
            }
            return count;
        }
        catch
        {
            return 0;
        }

    }

    public List<AbstractItem> GetNeighborsWithStatus(PropolisStatus status)
    {
        try
        {
            return Neighbors.Where(x => x.status == status).ToList<AbstractItem>();
        }
        catch
        {
            return new List<AbstractItem>();
        }

    }

    public List<AbstractItem> GetNeighborsWithStatus(PropolisStatus[] status)
    {
        try
        {
            List<AbstractItem> returnList = new List<AbstractItem>();
            foreach (var s in status)
            {
                returnList = returnList.Concat(GetNeighborsWithStatus(s)).ToList<AbstractItem>();
            }

            return returnList;
                
        }
        catch
        {
            return new List<AbstractItem>();
        }

    }

	private void Update()
	{
        

	}


    public void BlockDectection()
    {
        SendOscMessage("/status", ID, (int)PropolisStatus.DETECTION_OFF);
    }

    public void RestoreDectection()
    {
        StatusLocked = false;
        SendOscMessage("/status", ID, (int)PropolisStatus.DETECTION_ON);
    }


    private void SendOscMessage(string address, int value, int value2)
    {
        OSCMessage message = new OSCMessage(address);
        message.Append<int>(value);
        message.Append<int>(value2);
        ParentGroup.OSC.Send(message);

    }



}
