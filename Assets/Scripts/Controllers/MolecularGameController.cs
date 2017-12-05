using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class MolecularGameController : AbstractGameController
{
    public GameObject WaveGameObject;
    private GameObject WaveGameObjectInstance;
    //To be used instead of Update or FixedUpdate.

    private void Start()
    {
        WaveGameObjectInstance = Instantiate(WaveGameObject);
    }
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

        GenerateWaveGameController();
    }

    private void GenerateWaveGameController()
    {




        WaveGameObjectInstance.transform.position = new Vector3(GameArea.x + GameArea.width, GameArea.y + GameArea.height * .5f);

        WaveGameObjectInstance.transform.localScale = new Vector3(1, GameArea.height, 1);


        BoxCollider2D waveBoxCollider = WaveGameObjectInstance.GetComponent<BoxCollider2D>();
        waveBoxCollider.size = new Vector2(1.0f, GameArea.height);
       

    }
}
