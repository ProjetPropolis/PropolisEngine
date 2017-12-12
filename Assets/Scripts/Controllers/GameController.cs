using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Propolis;
using UnityOSC;

public class GameController : MonoBehaviour {

    List<GameObject> tuilesActives = new List<GameObject>();
    public PropolisManager propolisManager;
    public PropolisAlertUIController AlertUiController;

    public AbstractGameController hiveGameController, molecularGameController;
    public PropolisData propolisData;
    private IEnumerator GameLoopCoroutine;
    public float BatteryLevel = 0.0f;
    OSCServer server;
    object ThreadLock = new object();
    bool _mustReadData ;
    private System.Random random;


	void Start () {
        random = new System.Random();
        _mustReadData = false;
        propolisData = PropolisData.Instance;
        GameLoopCoroutine = ProcessGameLoop();
        server = new OSCServer(14001);
        server.PacketReceivedEvent += ProcessPacketReceived;
        server.Connect();

    }

    private void OnDestroy()
    {
        server.PacketReceivedEvent -= ProcessPacketReceived;
        server.Close();
    }

    private void ProcessPacketReceived(OSCServer sender, OSCPacket packet)
    {
        if (propolisData.IsGamePlaying && packet.Data.Count == 3)
        {
            //lock (ThreadLock)
            //{
                _mustReadData = true;
           // }
        }
    }

    private void Update()
    {

        //lock (ThreadLock)
        //{
            if (_mustReadData)
            {
                _mustReadData = false;
                switch (server.LastReceivedPacket.Address.Replace(@"/", ""))
                {
                    case PropolisDataTypes.HexGroup: hiveGameController.ProcessPacketFromOsc(server.LastReceivedPacket.Data);break;
                    case PropolisDataTypes.AtomGroup: molecularGameController.ProcessPacketFromOsc(server.LastReceivedPacket.Data); break;
                }
            }
        //}
    
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
            AbstractItem previousItemData =null;
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup:
                     previousItemData = hiveGameController.ListOfGroups.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID)
                    .ChildItemsList.FirstOrDefault(x => x.ID == propolisData.LastEvent.ID);
                    break;
                case PropolisDataTypes.AtomGroup:
                    previousItemData = molecularGameController.ListOfGroups.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID)
                   .ChildItemsList.FirstOrDefault(x => x.ID == propolisData.LastEvent.ID);
                    break;
            }

            if (previousItemData != null)
            {
                if (newItemData.Status == (int)PropolisStatus.ON || newItemData.Status == (int)PropolisStatus.CLEANSING)
                {
                    switch (previousItemData.PrevState)
                    {
                        case PropolisStatus.ON: IncrementBatteryLevel(PropolisGameSettings.ScorePressOnActiveHex); break;
                        case PropolisStatus.CORRUPTED: IncrementBatteryLevel(PropolisGameSettings.ScorePressOnCorruptedHex); break;
                        case PropolisStatus.ULTRACORRUPTED: IncrementBatteryLevel(PropolisGameSettings.ScoreOnCleanUltraCorruptedHex); break;
                        case PropolisStatus.CLEANSER: IncrementBatteryLevel(PropolisGameSettings.ScorePressOnCleannerHex); break;
                    }
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

    public void ProcessSuccessfulRecipe(PropolisRecipeCompareStatus status)
    {
        if(status == PropolisRecipeCompareStatus.IMPERFECT)
        {
            ((HiveGameController)hiveGameController).InstanciateCleanser();
        }else if(status == PropolisRecipeCompareStatus.PERFECT)
        {
            for (int i = 0; i < 3; i++)
            {
                ((HiveGameController)hiveGameController).InstanciateCleanser();
            }
        }
    }

    private void  StartGame()
    {
        Debug.Log("play");
        hiveGameController.InitOnPlay();
        molecularGameController.InitOnPlay();
        StopCoroutine(GameLoopCoroutine);
        StartCoroutine(GameLoopCoroutine);
        GenerateRecipe();
        AlertUiController.Show("Propolis Event", "Gameplay Started");

    }

    public void GenerateRecipe()
    {
        for (int i = 0; i < 3; i++)
        {
            PushRecipe();
        }
    }




    public void PushRecipe()
    {
        int r1 =random.Next(3) + (int)PropolisStatus.RECIPE1;
        int r2 = random.Next(3) + (int)PropolisStatus.RECIPE1;
        int r3 = random.Next(3) + (int)PropolisStatus.RECIPE1;
        SendCommand(string.Format("{0} {1} {2} {3}", PropolisActions.PushRecipe, r1, r2, r3));
    }

    private void StopGame()
    {
        Debug.Log("stop");
        hiveGameController.Stop();
        molecularGameController.Stop();
        StopCoroutine(GameLoopCoroutine);
        AlertUiController.Show("Propolis Event", "Gameplay Stopped");

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
   

 
}
