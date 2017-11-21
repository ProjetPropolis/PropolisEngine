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
            for (int i = 0; i < 3; i++)
            {
                ProcessCorruptionOnEdge();
            }
            
            
        }

        ProcessCorruption();
        IndexProcess++;
        IndexProcess = IndexProcess % 10;
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

        

                List<AbstractItem> CorrupedHexs = new List<AbstractItem>();
                ListOfGroups.ForEach(x => x.ChildHexsList.Where(y => y.Status == PropolisStatus.CORRUPTED).ToList<AbstractItem>().ForEach(z => CorrupedHexs.Add(z)));

                
                CorrupedHexs.ForEach(x => {
                    try
                    {
                        AbstractItem hexToCorrupted = x.Neighbors
                        .First(y => y.Status == PropolisStatus.OFF || y.Status == PropolisStatus.ON);
                        CorruptHex(hexToCorrupted);
                    }
                    catch (Exception)
                    {

                    }
               

                });
                

       
        
    }




    //private AbstractItem GetNeighboorOfHex(AbstractItem hex)
    //{

    //}
}
