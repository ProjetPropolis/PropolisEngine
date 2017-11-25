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
        throw new System.NotImplementedException();
    }
    public override void InitOnPlay()
    {
            base.InitOnPlay();
    }
}
