using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Linq;
using System;
using UnityOSC;

public class HiveGameController : AbstractGameController
{
    List<AbstractItem> EdgeHexList;
    List<AbstractItem> PotentialUtraCorrupt;
    List<AbstractItem> UltraCorruptedList;


    System.Random random;
    private int IndexProcess;
    bool readyCleanser;


    private void Start()
    {

    }

    private void OnDestroy()
    {
        
    }

    public override void ProcessUserInteraction(AbstractItem item, PropolisUserInteractions userAction)
    {
        if(userAction == PropolisUserInteractions.PRESS)
        {
            switch (item.Status)
            {
                case PropolisStatus.OFF: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.ON);break;
                case PropolisStatus.CORRUPTED: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.ON); break;
                case PropolisStatus.CLEANSER: StartCoroutine(ProcessCleanserExplosion(item.ParentGroup.ID,item.ID));break;
                default: SendItemData(item.ParentGroup.ID, item.ID,item.Status);break;
            }

            if (item.Status == PropolisStatus.ON || item.Status == PropolisStatus.CLEANSING)
            {
                item.StatusLocked = true;
            }


        }
        else
        {
            if (item.Status == PropolisStatus.ON || item.Status == PropolisStatus.CLEANSING)
            {
                item.StatusLocked = false;
            }
        }
    }

    //To be used instead of Update or FixedUpdate. 
    public override void UpdateGameLogic()
    {

        if(IndexProcess%3 == 0)
        {
            StartCoroutine(ProcessCorruptionOnEdge());
        }

        else
        {      
            StartCoroutine(ProcessCorruption());
        }

        if (IndexProcess % 4 == 0)
        {
            StartCoroutine(CreateRed());
            StartCoroutine(SpeadThatCorruption());
        }


        IndexProcess++;
        IndexProcess = IndexProcess % 30;

    }

    private void Update()
    {
        

    }

    public override void InitOnPlay()
    {
        base.InitOnPlay();        // va calculer chaque neighbors 
        random = new System.Random();
        Reset();


    }

    public override void Stop()
    {
       StopCoroutine(ProcessDeleteUltraCorrupted());

    }

    private void GenerateEdgeHexList()
    {
        EdgeHexList = new List<AbstractItem>();
        ListOfGroups.ForEach(x=>x.ChildItemsList.ForEach(y=> { if (y.Neighbors.Count < 6) { EdgeHexList.Add(y); } }));
    }

    private void Reset()
    {
        GenerateEdgeHexList();
        IndexProcess = 0; // nous donne le nombre de clics 
        UltraCorruptedList = new List<AbstractItem>();
        SetAllItemsTo(PropolisStatus.OFF);
        StopCoroutine(ProcessDeleteUltraCorrupted());
        StartCoroutine(ProcessDeleteUltraCorrupted());
    }


    private void CorruptHex(AbstractItem abstractItem)
    {
        if((abstractItem.Status == PropolisStatus.OFF || abstractItem.Status == PropolisStatus.ON && !abstractItem.StatusLocked))
        {
            SendItemData(abstractItem.ParentGroup.ID, abstractItem.ID, PropolisStatus.CORRUPTED);
        }
    }

    private void CleanserHex(AbstractItem abstractItem)
    {
            SendItemData(abstractItem.ParentGroup.ID, abstractItem.ID, PropolisStatus.CLEANSER);
    }
    private void UltraCorruptHex(AbstractItem abstractItem)
    {
        if ((abstractItem.Status == PropolisStatus.OFF || abstractItem.Status == PropolisStatus.ON || abstractItem.Status == PropolisStatus.CORRUPTED) && !abstractItem.StatusLocked)
        {
            SendItemData(abstractItem.ParentGroup.ID, abstractItem.ID, PropolisStatus.ULTRACORRUPTED);
            UltraCorruptedList.Add(abstractItem);
        }
    }
    private IEnumerator ProcessDeleteUltraCorrupted()
    {
        while (true)
        {
            for (int i = UltraCorruptedList.Count -1; i >=0; i--)
            {
                AbstractItem HexToProcesss = UltraCorruptedList[i];
                if (HexToProcesss.CountNeighborsWithStatus(PropolisStatus.ON) == 6)
                {
                    SendItemData(HexToProcesss.ParentGroup.ID, HexToProcesss.ID, PropolisStatus.ON);
                    UltraCorruptedList.Remove(HexToProcesss);
                    InstanciateCleanser();
                }
            }
            yield return new WaitForSeconds(PropolisGameSettings.IntervalProcessUltraCorrupted);
        }
    }
    private IEnumerator ProcessCorruptionOnEdge()
    {
        AbstractItem hexToCorrupted = EdgeHexList.ElementAt(random.Next(EdgeHexList.Count));
        CorruptHex(hexToCorrupted);
        List<AbstractItem> NeighborsToCorrupt = 
        hexToCorrupted.GetNeighborsWithStatus(PropolisGameSettings.StatusFreeToBeCorrupted);

        int numberOfHexToProcess = Mathf.Clamp(NeighborsToCorrupt.Count, 0, PropolisGameSettings.MaxEdgeHexNeighborsCorruption);

        for (int i = 0; i < numberOfHexToProcess; i++)
        {
            AbstractItem hexNeighborsToCorrupted = NeighborsToCorrupt.ElementAt(random.Next(NeighborsToCorrupt.Count));
            CorruptHex(hexNeighborsToCorrupted);
            NeighborsToCorrupt.Remove(hexNeighborsToCorrupted);
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
        }


    }



    private IEnumerator ProcessCleanserExplosion(int groupID, int itemID)
    {
        AbstractItem cleanser = GetAbstractItemFromIDS(groupID, itemID);
        List<AbstractItem> hexstoBeCleanned = new List<AbstractItem>(cleanser.Neighbors);
        int i=0;
        List<AbstractItem> hexCleannedOrderList = new List<AbstractItem>();

        SendItemData(cleanser.ParentGroup.ID, cleanser.ID, PropolisStatus.CLEANSING);

        while (i < cleanser.Neighbors.Count && hexstoBeCleanned.Count>0)
        {
            AbstractItem hex = hexstoBeCleanned.ElementAt(random.Next(hexstoBeCleanned.Count)) ;
            SendItemData(hex.ParentGroup.ID, hex.ID, PropolisStatus.CLEANSING);
            hexCleannedOrderList.Add(hex);
            hexstoBeCleanned.Remove(hex);
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
            i++;
        }

        yield return new WaitForSeconds(PropolisGameSettings.CleansingStateDuration);

        SendItemData(cleanser.ParentGroup.ID, cleanser.ID, PropolisStatus.ON);

        foreach (var hex in hexCleannedOrderList)
        {
            SendItemData(hex.ParentGroup.ID, hex.ID, PropolisStatus.ON);
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
        }

    }

    private IEnumerator ProcessCorruption()
    {

        List<AbstractItem> CorrupedHexs = new List<AbstractItem>();
        ListOfGroups
            .ForEach(x => x.ChildItemsList.Where(y => y.Status == PropolisStatus.CORRUPTED && (y.CountNeighborsWithStatus(PropolisStatus.ON) > 0 || y.CountNeighborsWithStatus(PropolisStatus.OFF) > 0))
            .ToList<AbstractItem>()
            .ForEach(z => CorrupedHexs.Add(z)));

        int HexCountToCorrupt = Mathf.Clamp(PropolisGameSettings.MaxEdgeHexNeighborsCorruption, 0, CorrupedHexs.Count);
        int i = 0;


        while (i < HexCountToCorrupt && CorrupedHexs.Count > 0)
        {
            AbstractItem Corruptor = CorrupedHexs.ElementAt(random.Next(CorrupedHexs.Count));
            List<AbstractItem> FreeToBeCorruptedHex = Corruptor.GetNeighborsWithStatus(PropolisGameSettings.StatusFreeToBeCorrupted);
            if (FreeToBeCorruptedHex.Count > 0)
            {
                AbstractItem HexToBeCorrupted = FreeToBeCorruptedHex.ElementAt(random.Next(FreeToBeCorruptedHex.Count));
                CorruptHex(HexToBeCorrupted);
                CorrupedHexs.Remove(Corruptor);
            }
       
            i++;
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
        }
  
    }

    private IEnumerator CreateRed()
    {
        var numUltra = PropolisGameSettings.NumOfUltraCorruped;
        var numMax    = PropolisGameSettings.MaxNumOfUltraCorruped;
        PotentialUtraCorrupt = new List<AbstractItem>();
        ListOfGroups.ForEach(x => x.ChildItemsList.ForEach(y => { if ((y.Neighbors.Count == 6)&&(y.Status == PropolisStatus.ON || y.Status == PropolisStatus.OFF || y.Status == PropolisStatus.CORRUPTED) && (y.CountNeighborsWithStatus(PropolisStatus.ULTRACORRUPTED)== 0)) { PotentialUtraCorrupt.Add(y); } }));

        int HexCountToUltraCorrupt = Mathf.Clamp(numUltra, 0, PotentialUtraCorrupt.Count);
        int i = 0;

        while (i < HexCountToUltraCorrupt && PotentialUtraCorrupt.Count > 0 && UltraCorruptedList.Count < numMax)
        {
            if (PotentialUtraCorrupt.Count > 0) { 
            AbstractItem UtraCorruptor =  PotentialUtraCorrupt.ElementAt(random.Next(PotentialUtraCorrupt.Count));
            UltraCorruptHex(UtraCorruptor);
            PotentialUtraCorrupt.Remove(UtraCorruptor);
            }
            i++;
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
        }

      
    }

    private IEnumerator SpeadThatCorruption()
    {
       
        List<AbstractItem> NeighborsToCorrupt = new List<AbstractItem>();
        UltraCorruptedList.ForEach(x => x.Neighbors.ForEach(y => { if (y.Status != PropolisStatus.CORRUPTED) { NeighborsToCorrupt.Add(y); } }));

        while (NeighborsToCorrupt.Count > 0)
        {
            AbstractItem hexToCorrupt = NeighborsToCorrupt.ElementAt(random.Next(NeighborsToCorrupt.Count));
            CorruptHex(hexToCorrupt);
            NeighborsToCorrupt.Remove(hexToCorrupt);
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn*0.5f);
        }
   
      
    }




    private void InstanciateCleanser()
    {
        
        var TileToCorrupt = 
         ListOfGroups.OrderByDescending(x => x.CountChildrenWithStatus(PropolisStatus.CORRUPTED)).First() // get the most corrupted parent
        .ChildItemsList.OrderByDescending(y=> y.CountNeighborsWithStatus(PropolisStatus.CORRUPTED)).First(); // get the hex with most corrupt Neighbors   
        CleanserHex(TileToCorrupt);

        //if (propolisData.BatteryLevel > 0.5f)
        //{

        //    if (readyCleanser == true)
        //    {
        //        CleanserHex(TileToCorrupt);
        //        readyCleanser = false;

        //    }

        //}
        //else { 
        //    if(readyCleanser != true)
        //        readyCleanser = true;
        //}
    }


    //private AbstractItem GetFarthestHexFrom (AbstractItem target, List <AbstractItem> searchList)
    //{
    //    try
    //    {
    //        AbstractItem FarthestHex = searchList.OrderBy(t => Vector3.Distance(target.transform.position, t.transform.position)).First();
    //        if (Vector3.Distance(target.transform.position, FarthestHex.transform.position) > PropolisGameSettings.MinPropraggationCorruptionDistance)
    //        {
    //            return FarthestHex;
    //        }
    //        else
    //        {
    //            return null; 
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        return null;
    //    }      
    //} 


}

