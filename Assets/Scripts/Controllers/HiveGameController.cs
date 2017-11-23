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
        if(IndexProcess%4 == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                ProcessCorruptionOnEdge();
            }
            
            
        }
        for (int i = 0; i < 4; i++)
        {
            ProcessCorruption();
        }
        ;
        IndexProcess++;
        IndexProcess = IndexProcess % 30;
    }

    public override void InitOnPlay()
    {
        base.InitOnPlay();
        random = new System.Random();
        GenerateEdgeHexList();
        IndexProcess = 0;
        // EdgeHexList.ForEach(x=> SendItemData(x.ParentGroup.i))
        //EdgeHexList.ForEach(x => SendItemData(x.ParentGroup.ID, x.ID, PropolisStatus.CLEANSER));

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
    }

    private void ProcessCorruption()
    {

        //AbstractItem hexToCorrupted = x.Neighbors
        //                .First(y => y.Status == PropolisStatus.OFF || y.Status == PropolisStatus.ON);
        //CorruptHex(hexToCorrupted);

        List<AbstractItem> CorrupedHexs = new List<AbstractItem>();
        ListOfGroups
            .ForEach(x => x.ChildHexsList.Where(y => y.Status == PropolisStatus.CORRUPTED && (y.CountNeighborsWithStatus(PropolisStatus.ON) > 0 || y.CountNeighborsWithStatus(PropolisStatus.OFF) > 0))
            .ToList<AbstractItem>()
            .ForEach(z => CorrupedHexs.Add(z)));

        if (CorrupedHexs.Count > 0) {
            try
            {
                AbstractItem HexToCorrupted = CorrupedHexs.ElementAt(random.Next(CorrupedHexs.Count)).Neighbors.First(x => x.Status == PropolisStatus.ON || x.Status == PropolisStatus.OFF );
                CorruptHex(HexToCorrupted);
            }
            catch (Exception)
            {

               
            }

        }



    }




    //private AbstractItem GetNeighboorOfHex(AbstractItem hex)
    //{

    //}
}
