using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Linq;
using System;
using UnityOSC;

public class RecipeGameController : AbstractGameController
{



    System.Random random;
    public MolecularGameController molecularGameController;


    private void Start()
    {

    }

    private void OnDestroy()
    {
        
    }
    public override void  UpdateGameLogic()
    {

    }


    public override void ProcessUserInteraction(AbstractItem item, PropolisUserInteractions userAction)
    {
       /* if(userAction == PropolisUserInteractions.PRESS && item.ParentGroup.DataType == PropolisDataTypes.RecipeGroup)
        {
            if (!item.IsShield)
            {
                molecularGameController.ReceivedRecipeInteracton(item.ParentGroup.ID + 10,item.ID,(PropolisStatus) item.status);
            }
            else
            {
                SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SHIELD_ON);
                molecularGameController.ReceiveShieldInteractionFromHive(item.ParentGroup.ID + 10, PropolisUserInteractions.PRESS);
            }
        }
        else if(item.ParentGroup.DataType == PropolisDataTypes.RecipeGroup)
        {
            if (!item.IsShield)
            {

            }
            else
            {
                SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SHIELD_OFF);
                molecularGameController.ReceiveShieldInteractionFromHive(item.ParentGroup.ID + 10, PropolisUserInteractions.PULL_OFF);
            }
        }*/
    }

  

    private void Update()
    {
        

    }

    public void UpdateFromNewRecipe()
    {
        /*if (PropolisData.Instance.RecipeStack.Count >= 3)
        {
            PropolisRecipe recipe = PropolisData.Instance.RecipeStack.ToArray()[1];

            foreach (var group in ListOfGroups)
            {
                foreach (var item in group.ChildItemsList)
                {
                    if(item.ID <= 3)
                    SendItemData(group.ID,item.ID, (PropolisStatus  )recipe.GetItem(item.ID));
                }

            }
        }*/
    }

    public override void InitOnPlay()
    {
        base.InitOnPlay();        // va calculer chaque neighbors 
        Reset();
    }

    public override void Stop()
    {


    }



    private void Reset()
    {
        foreach (var group in ListOfGroups)
        {
            foreach (var item in group.ChildItemsList)
            {
                if(item.ID == 3)
                {
                    SendItemData(group.ID, item.ID, PropolisStatus.SHIELD_OFF);
                }
            }
        }

    }


  


}

