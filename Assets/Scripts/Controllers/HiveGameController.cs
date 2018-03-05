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
    Coroutine SuperCleanExplosionCoroutine;
    Coroutine SuperCleanCoroutine;


    System.Random random;
    private int IndexProcess;
    bool readyCleanser;
    public bool canSpawnSuperClean = true, isPlayingSuperClean = false;


    private void KillSuperCleanCoroutine()
    {
        if(SuperCleanCoroutine != null)
        {
            StopCoroutine(SuperCleanCoroutine);
        }
    }

    private IEnumerator ProcessSuperCleanSpawn()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(50);
            try
            {
                AbstractItem item = ListOfItems[random.Next(ListOfItems.Count)];
                InstantiateSuperClean(item);
            }
            catch (Exception)
            {

                throw;
            }
        }
      
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {

    }

    public override void ProcessUserInteraction(AbstractItem item, PropolisUserInteractions userAction)
    {
        if (userAction == PropolisUserInteractions.PRESS && item.ParentGroup.DataType == PropolisDataTypes.HexGroup)
        {
            PropolisStatsExporter.IncrementStatValue("AlveolesPressed");
            switch (item.Status)
            {
                case PropolisStatus.OFF: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.ON); break;
                case PropolisStatus.CORRUPTED: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.ON); break;
                case PropolisStatus.CLEANSER: StartCoroutine(ProcessCleanserExplosion(item.ParentGroup.ID, item.ID)); PropolisStatsExporter.IncrementStatValue("CleanserPressed"); break;
                case PropolisStatus.ULTRACORRUPTED_CLEAR_HINT: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.ON); break;
                case PropolisStatus.ULTRACORRUPTED: ProcessUltraCorruptedHint(item); break;
                case PropolisStatus.SUPER_CLEAN_TRIGGER: StartProcessSuperCleanExplosion(item);break;
                default: SendItemData(item.ParentGroup.ID, item.ID, item.Status); break;
            }

            if (item.Status == PropolisStatus.ON || item.Status == PropolisStatus.CLEANSING)
            {
                item.StatusLocked = true;
                if (item.Status == PropolisStatus.ON)
                {
                    item.AutoUnlockIn(PropolisGameSettings.AutoUnlockHexAfter / PropolisGameSettings.CurrentDifficultyMultiplier);
                    item.TriggerDelayedStatus(PropolisStatus.CORRUPTED, PropolisGameSettings.AutoUnlockHexAfter * 2 / PropolisGameSettings.CurrentDifficultyMultiplier);
                }
            }


        }
        else if (item.ParentGroup.DataType == PropolisDataTypes.HexGroup)
        {
            if (item.Status == PropolisStatus.ON || item.Status == PropolisStatus.CLEANSING)
            {
                item.StatusLocked = false;
            }
            else if (item.Status == PropolisStatus.ULTRACORRUPTED)
            {
                CancelProcessUltraCorruptedHint(item);
            }
        }
    }


    private void ProcessUltraCorruptedHint(AbstractItem item) {
        item.StatusLocked = true;
        SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.ULTRACORRUPTED);
        foreach (var n in item.Neighbors)
        {
            if (n.status == PropolisStatus.CORRUPTED)
            {
                SendItemData(n.ParentGroup.ID, n.ID, PropolisStatus.ULTRACORRUPTED_CLEAR_HINT);
            }
        }

    }


    private void CancelProcessUltraCorruptedHint(AbstractItem item)
    {
        item.StatusLocked = false;
        foreach (var n in item.Neighbors)
        {
            if (n.status == PropolisStatus.ULTRACORRUPTED_CLEAR_HINT)
            {
                SendItemData(n.ParentGroup.ID, n.ID, PropolisStatus.CORRUPTED);
            }
        }

    }

    //To be used instead of Update or FixedUpdate. 
    public override void UpdateGameLogic()
    {
        StartCoroutine(ProcessCorruptionOnEdge());
        StartCoroutine(SpeadThatCorruption());
        StartCoroutine(CreateRed());
        StartCoroutine(ProcessCorruption());
        CreateCleanserNecessary();
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
        StopAllCoroutines();

    }

    public void InstantiateSuperClean(AbstractItem item)
    {
        if (item.ParentGroup.DataType == PropolisDataTypes.HexGroup && canSpawnSuperClean)
        {
            canSpawnSuperClean = false;
            if (item.status == PropolisStatus.ULTRACORRUPTED)
            {
                try
                {
                    UltraCorruptedList.Remove(item);
                }
                catch (Exception)
                {
                }

            }
            SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SUPER_CLEAN_TRIGGER);
        }
    }
    void StartProcessSuperCleanExplosion(AbstractItem item)
    {
        KillSuperCleanExplosionCoroutine();
        StartCoroutine(ProcessSuperCleanExplosion(item));
    }
    void KillSuperCleanExplosionCoroutine()
    {
        if (SuperCleanExplosionCoroutine != null)
            StopCoroutine(SuperCleanExplosionCoroutine);
    }
    IEnumerator ProcessSuperCleanExplosion(AbstractItem item)
    {
        isPlayingSuperClean = true;
        GameController.PropolisExport.SendSuperCleanExplosionStatusToSound(1);
        SendItemData(item.ParentGroup.ID, item.ID, GetRandomSuperCleanStatus());

        foreach(var n in item.Neighbors)
        {
            SendItemData(n.ParentGroup.ID, n.ID, GetRandomSuperCleanStatus());
            if (n.status == PropolisStatus.ULTRACORRUPTED)
            {
                try
                {
                    UltraCorruptedList.Remove(item);
                }
                catch (Exception)
                {
                }

            }
            yield return new WaitForSecondsRealtime(0.05f);

            foreach (var sn in n.Neighbors)
            {
                if (sn.status == PropolisStatus.ULTRACORRUPTED)
                {
                    try
                    {
                        UltraCorruptedList.Remove(item);
                    }
                    catch (Exception)
                    {
                    }

                }
                SendItemData(sn.ParentGroup.ID, sn.ID, GetRandomSuperCleanStatus());
                yield return new WaitForSecondsRealtime(0.05f);

            }
        }

        yield return new WaitForSecondsRealtime(0.05f);
        foreach (var n in item.Neighbors)
        {
            foreach (var sn in n.Neighbors)            
            {
                if (sn.status == PropolisStatus.ULTRACORRUPTED)
                {
                    try
                    {
                        UltraCorruptedList.Remove(item);
                    }
                    catch (Exception)
                    {
                    }

                }
                CleanserHex(sn);
                yield return new WaitForSecondsRealtime(0.05f);

            }
          
        }





        yield return new WaitForSecondsRealtime(0.2f);
        ExploseAllCleanser();
        yield return new WaitForSecondsRealtime(3f);
        GameController.IncrementBatteryLevel(0.05f);
        GameController.PropolisExport.SendSuperCleanExplosionStatusToSound(0);
        canSpawnSuperClean = true;
        isPlayingSuperClean = false;
    }

    private PropolisStatus GetRandomSuperCleanStatus() {
       return (PropolisStatus)random.Next((int)PropolisStatus.SUPER_CLEAN_TURQUOISE_FADE, (int)PropolisStatus.SUPER_CLEAN_TRIGGER);
    }

    private void ExploseAllCleanser()
    {
        try
        {
            var ListOfAllCleanser = ListOfItems.Where(x => x.status == PropolisStatus.CLEANSER);
            foreach(var cleanser in ListOfAllCleanser)
            {
                StartCoroutine(ProcessCleanserExplosion(cleanser.ParentGroup.ID, cleanser.ID));
            }
        }
        catch (Exception)
        {

        }
    }
    

    private void GenerateEdgeHexList()
    {
        EdgeHexList = new List<AbstractItem>();
        ListOfGroups.ForEach(x=>x.ChildItemsList.ForEach(y=> { if (y.Neighbors.Count < 6) { EdgeHexList.Add(y); } }));
    }

    private void Reset()
    {
        canSpawnSuperClean = true;
        isPlayingSuperClean = false;
        KillSuperCleanCoroutine();
        SuperCleanCoroutine = StartCoroutine(ProcessSuperCleanSpawn());
        KillSuperCleanExplosionCoroutine();
        GenerateEdgeHexList();
        IndexProcess = 0; // nous donne le nombre de clics 
        UltraCorruptedList = new List<AbstractItem>();
        SetAllItemsTo(PropolisStatus.CORRUPTED);       
        StopCoroutine(ProcessDeleteUltraCorrupted());
        StartCoroutine(ProcessDeleteUltraCorrupted());
        InstanciateCleanser();
        ValidateStatusFromPlay();
    }


    private void CorruptHex(AbstractItem abstractItem)
    {
        if((abstractItem.Status == PropolisStatus.OFF || abstractItem.Status == PropolisStatus.ON && !abstractItem.StatusLocked))
        {
            if (abstractItem.GetNeighborsWithStatus(PropolisStatus.ULTRACORRUPTED).Where(x => x.StatusLocked).Any())
            {
                SendItemData(abstractItem.ParentGroup.ID, abstractItem.ID, PropolisStatus.ULTRACORRUPTED_CLEAR_HINT);
            }
            else
            {
                SendItemData(abstractItem.ParentGroup.ID, abstractItem.ID, PropolisStatus.CORRUPTED);
            }

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
            abstractItem.Neighbors.ForEach(x => CorruptHex(x));
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
                    PropolisStatsExporter.IncrementStatValue("UltraCorruptedDestroye");
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

        int numberOfHexToProcess = Mathf.Clamp(NeighborsToCorrupt.Count, 0, (int)(PropolisGameSettings.MaxEdgeHexNeighborsCorruption * PropolisGameSettings.CurrentDifficultyMultiplier));

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
        GameController.PropolisExport.SendAllCleanserPress();
        AbstractItem cleanser = GetAbstractItemFromIDS(groupID, itemID);
        List<AbstractItem> hexstoBeCleanned = new List<AbstractItem>(cleanser.Neighbors);
        int i=0;
        List<AbstractItem> hexCleannedOrderList = new List<AbstractItem>();

        SendItemData(cleanser.ParentGroup.ID, cleanser.ID, PropolisStatus.ON);
        cleanser.StatusLocked = true;

        while (i < cleanser.Neighbors.Count && hexstoBeCleanned.Count>0)
        {
            AbstractItem hex = hexstoBeCleanned.ElementAt(random.Next(hexstoBeCleanned.Count)) ;
            hex.StatusLocked = true;
            if(hex.status == PropolisStatus.CLEANSER)
            {
                StartCoroutine(ProcessCleanserExplosion(hex.ParentGroup.ID, hex.ID));
            }
            else
            {
                if(hex.status == PropolisStatus.ULTRACORRUPTED)
                {
                    try
                    {
                        UltraCorruptedList.Remove(hex);
                    }
                    catch (Exception)
                    {
                    }
                    
                }
                SendItemData(hex.ParentGroup.ID, hex.ID, PropolisStatus.ON);
            }
            hex.TriggerDelayedStatus(PropolisStatus.CORRUPTED, PropolisGameSettings.AutoUnlockHexAfter * 2 / PropolisGameSettings.CurrentDifficultyMultiplier);
            hexCleannedOrderList.Add(hex);
            hexstoBeCleanned.Remove(hex);
            yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
            i++;
        }



        yield return new WaitForSeconds(PropolisGameSettings.HexSafeTimeAfterCleanse);

        foreach (var item in cleanser.Neighbors)
        {
            item.StatusLocked = false;
        }
        cleanser.StatusLocked = false;


        //yield return new WaitForSeconds(PropolisGameSettings.CleansingStateDuration);

        //SendItemData(cleanser.ParentGroup.ID, cleanser.ID, PropolisStatus.ON);

        //foreach (var hex in hexCleannedOrderList)
        //{
        //    SendItemData(hex.ParentGroup.ID, hex.ID, PropolisStatus.ON);
        //    yield return new WaitForSeconds(PropolisGameSettings.TimeBetweenAnimationSpawn);
        //}

    }
    public void CreateCleanserNecessary()
    {
        if (GetRatioOfGivenPropolisStatus(PropolisStatus.CLEANSER) < PropolisGameSettings.WishedPourcentageOfCleanser)
        {
            InstanciateCleanser();
        }
    }

    private IEnumerator ProcessCorruption()
    {

        List<AbstractItem> CorrupedHexs = new List<AbstractItem>();
        ListOfGroups
            .ForEach(x => x.ChildItemsList.Where(y => y.Status == PropolisStatus.CORRUPTED && (y.CountNeighborsWithStatus(PropolisStatus.ON) > 0 || y.CountNeighborsWithStatus(PropolisStatus.OFF) > 0))
            .ToList<AbstractItem>()
            .ForEach(z => CorrupedHexs.Add(z)));

        int HexCountToCorrupt = Mathf.Clamp((int)(PropolisGameSettings.MaxEdgeHexNeighborsCorruption * PropolisGameSettings.CurrentDifficultyMultiplier), 0, CorrupedHexs.Count);
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

        while (i < HexCountToUltraCorrupt && PotentialUtraCorrupt.Count > 0 && UltraCorruptedList.Count <(int)( numMax * PropolisGameSettings.CurrentDifficultyMultiplier))
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

    public void InstanciateCleanser()
    {
        var TileToCorrupt = 
         ListOfGroups.OrderByDescending(x => x.CountChildrenWithStatus(PropolisStatus.CORRUPTED)).First() // get the most corrupted parent
        .ChildItemsList.Where(y=>y.status != PropolisStatus.ULTRACORRUPTED &&
        y.status != PropolisStatus.CLEANSER &&
        y.status != PropolisStatus.SUPER_CLEAN_GREEN &&
        y.status != PropolisStatus.SUPER_CLEAN_TURQUOISE &&
        y.status != PropolisStatus.SUPER_CLEAN_TURQUOISE_FADE &&
        y.status != PropolisStatus.SUPER_CLEAN_TRIGGER
        ).OrderByDescending(y=> y.CountNeighborsWithStatus(PropolisStatus.CORRUPTED)).First(); // get the hex with most corrupt Neighbors   
        CleanserHex(TileToCorrupt);


    }



}

