using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class MolecularGameController : AbstractGameController
{
    //To be used instead of Update or FixedUpdate. 
    public   override void UpdateGameLogic()
    {

    }
    public override void ProcessUserInteraction(AbstractItem item, PropolisUserInteractions userAction)
    {
        if (userAction == PropolisUserInteractions.PRESS)
        {
            switch (item.Status)
            {
                case PropolisStatus.OFF: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE1); break;
                case PropolisStatus.CORRUPTED: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE1); break;
                case PropolisStatus.RECIPE1: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE2); break;
                case PropolisStatus.RECIPE2: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE3); break;
                case PropolisStatus.RECIPE3: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE1); break;
                default: SendItemData(item.ParentGroup.ID, item.ID, item.Status); break;
            }

        }
    }
    public override void InitOnPlay()
    {
            base.InitOnPlay();
    }
}
