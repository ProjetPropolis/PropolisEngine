using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Linq;
using System;

public class HiveGameController : AbstractGameController
{
    List<AbstractItem> EdgeHexList;
    System.Random random;
    private int IndexProcess;

    //To be used instead of Update or FixedUpdate. 
    public override void UpdateGameLogic()
    {
        if(IndexProcess%2 == 0)
        {
            ProcessCorruptionOnEdge();
        }
        else
        {
         
            ProcessCorruption();

        }

   
        
        IndexProcess++;
        IndexProcess = IndexProcess % 30;
    }

    public override void InitOnPlay()
    {
        base.InitOnPlay();        // va calculer chaque neighbors 
        random = new System.Random();
        GenerateEdgeHexList();
        IndexProcess = 0; // nous donne le nombre de clics 

    }

    private void GenerateEdgeHexList()
    {
        EdgeHexList = new List<AbstractItem>();
        ListOfGroups.ForEach(x=>x.ChildHexsList.ForEach(y=> { if (y.Neighbors.Count < 6) { EdgeHexList.Add(y); } }));
    }


    private void CorruptHex(AbstractItem abstractItem)
    {
        if(abstractItem.Status == PropolisStatus.OFF || abstractItem.Status == PropolisStatus.ON)
        {
            SendItemData(abstractItem.ParentGroup.ID, abstractItem.ID, PropolisStatus.CORRUPTED);
        }
    }

    private void ProcessCorruptionOnEdge()
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
        }


    }

    private void ProcessCorruption()
    {

        List<AbstractItem> CorrupedHexs = new List<AbstractItem>();
        ListOfGroups
            .ForEach(x => x.ChildHexsList.Where(y => y.Status == PropolisStatus.CORRUPTED && (y.CountNeighborsWithStatus(PropolisStatus.ON) > 0 || y.CountNeighborsWithStatus(PropolisStatus.OFF) > 0))
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
        }
   

       
        
          

        
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
