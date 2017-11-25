using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Propolis;

public class GameController : MonoBehaviour {

    List<GameObject> tuilesActives = new List<GameObject>();
    public PropolisManager propolisManager;

    public AbstractGameController hiveGameController, molecularGameController;
    public PropolisData propolisData;
    private IEnumerator GameLoopCoroutine;
    public float BatteryLevel = 0.0f;


	void Start () {
        propolisData = PropolisData.Instance;
        GameLoopCoroutine = ProcessGameLoop();
    }

    public void SendCommand(string command)
    {
        propolisManager.SendCommand(command);
    }

    private void UpdateBattery()
    {

        if (propolisData.IsGamePlaying && 
            propolisData.LastEvent.Action == PropolisActions.UpdateItemStatus)
        {
           PropolisGroupItemData newItemData =propolisData.GetGroupDataById(propolisData.LastEvent.GroupID, 
               propolisData.LastEvent.Type).Childrens.FirstOrDefault(x=> x.ID == propolisData.LastEvent.ID);

            AbstractItem previousItemData = hiveGameController.ListOfGroups.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID)
                .ChildHexsList.FirstOrDefault(x=>x.ID == propolisData.LastEvent.ID);

            if (newItemData.Status == (int)PropolisStatus.ON || newItemData.Status == (int)PropolisStatus.CLEANSING)
            {
                switch (previousItemData.PrevState)
                {
                    case PropolisStatus.ON: IncrementBatteryLevel(PropolisGameSettings.ScorePressOnActiveHex);break;
                    case PropolisStatus.CORRUPTED: IncrementBatteryLevel(PropolisGameSettings.ScorePressOnCorruptedHex); break;
                    case PropolisStatus.ULTRACORRUPTED: IncrementBatteryLevel(PropolisGameSettings.ScoreOnCleanUltraCorruptedHex); break;
                    case PropolisStatus.CLEANSER: IncrementBatteryLevel(PropolisGameSettings.ScorePressOnCleannerHex); break;
                }
            }

        }


    }

    public void UpdateFromModel()
    {
        propolisData = PropolisData.Instance;
                     
            BatteryLevel = propolisData.BatteryLevel;
            switch (propolisData.LastEvent.Action)
            {
                case PropolisActions.Play:StartGame(); break;
                case PropolisActions.Stop:StopGame(); break;
            }
        

        hiveGameController.UpdateFromModel();
        molecularGameController.UpdateFromModel();
        UpdateBattery();


    }

    private void  StartGame()
    {
        Debug.Log("play");
        hiveGameController.InitOnPlay();
        molecularGameController.InitOnPlay();
        StartCoroutine(GameLoopCoroutine);
    }

    private void StopGame()
    {
        Debug.Log("stop");
        hiveGameController.Stop();
        molecularGameController.Stop();
        StopCoroutine(GameLoopCoroutine);

    }

    public IEnumerator ProcessGameLoop()
    {
        while (true)
        {
            hiveGameController.UpdateGameLogic();
            molecularGameController.UpdateGameLogic();
            //Debug.Log("looping game loop");
            yield return new WaitForSeconds(PropolisGameSettings.DefaultGameTickTime);
        }

    }


   
    public void IncrementBatteryLevel(float increment)
    {
        float futureBatteryLevel = propolisData.BatteryLevel + increment;
        if (futureBatteryLevel >= 1)
        {
            SendCommand(string.Format("{0} {1}", PropolisActions.SetBatteryLevel, 0));
            //@TODO ADD TRIGGER FOR FULL BATTERY
        }
        else
        {
            SendCommand(string.Format("{0} {1}", PropolisActions.SetBatteryLevel, propolisData.BatteryLevel + increment));
        }
        
    }

    public void ProcessUserInteraction(string type, AbstractItem item, PropolisUserInteractions userAction)
    {
        switch (type)
        {
            case PropolisDataTypes.HexGroup: hiveGameController.ProcessUserInteraction(item, userAction);break;
            case PropolisDataTypes.AtomGroup: molecularGameController.ProcessUserInteraction(item, userAction);break;
        }
    }
    
	void Update () {
    
    }

 
}
