﻿using System.Collections;
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
    public PropolisAnimator animator;
    public PropolisExport PropolisExport {
        get
        {
            return propolisManager.PropolisExport;
        }
    }
    public AbstractGameController hiveGameController, molecularGameController, recipeGameController;
    public PropolisData propolisData;
    private IEnumerator GameLoopCoroutine;
    public float BatteryLevel = 0.0f;
    OSCServer server;
    object ThreadLock = new object();
    bool _mustReadData ;
    private System.Random random;

    public void SetDetectionOFF()
    {
        hiveGameController.ListOfItems.ForEach(x => x.BlockDectection());
        molecularGameController.ListOfItems.ForEach(x => x.BlockDectection());
    }

    public void SetDetectionON()
    {
        hiveGameController.ListOfItems.ForEach(x => x.RestoreDectection());
        molecularGameController.ListOfItems.ForEach(x => x.RestoreDectection());
    }

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
                    case PropolisDataTypes.RecipeGroup: recipeGameController.ProcessPacketFromOsc(server.LastReceivedPacket.Data); break;
                }
            }
        //}
    
    }

    public void SendCommand(string command)
    {
        propolisManager.SendCommand(command);
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
        recipeGameController.UpdateFromModel();


    }

    public void ProcessSuccessfulRecipe(PropolisRecipeCompareStatus status)
    {
        if(status == PropolisRecipeCompareStatus.IMPERFECT)
        {
            for (int i = 0; i < PropolisGameSettings.AmountOfSpawnedCleanserOnRecipe; i++)
            {
                ((HiveGameController)hiveGameController).InstanciateCleanser();
            }
        }
        else if(status == PropolisRecipeCompareStatus.PERFECT)
        {
            for (int i = 0; i < 3; i++)
            {
                ((HiveGameController)hiveGameController).InstanciateCleanser();
            }
        }
    }

    private void  StartGame()
    {
        StopAllCoroutines();
        PropolisGameSettings.CurrentDifficultyMultiplier = 1.0f;
        SetDetectionON();
        PropolisExport.SendClimaxState(0);
        animator.Desactivate();
        hiveGameController.InitOnPlay();
        molecularGameController.InitOnPlay();
        recipeGameController.InitOnPlay();
        StopCoroutine(ProcessBatteryUpdate());
        StartCoroutine(ProcessBatteryUpdate());
        StopCoroutine(GameLoopCoroutine);
        StartCoroutine(GameLoopCoroutine);
        StopCoroutine(ProcessDifficultyLevel());
        StartCoroutine(ProcessDifficultyLevel());
        GenerateRecipe();
        AlertUiController.Show("Propolis Event", "Gameplay Started");

    }

    private IEnumerator ProcessBatteryUpdate()
    {
        while (true)
        {

            yield return new WaitForSecondsRealtime(PropolisGameSettings.BatteryUpdateDeltaTime);
                // Debug.Log("---------------StartOfNewBatteryUpdate---------------");
            float currenBoardRatio = hiveGameController.GetRatioOfGivenPropolisStatus(PropolisStatus.ON);
            if (currenBoardRatio < PropolisGameSettings.CriticalOnHexRatio)
            {
                //Debug.Log(string.Format("---------CriticalLostOf :{0} ---------", PropolisGameSettings.BatteryLevelLostWhenCritical));
                IncrementBatteryLevel(PropolisGameSettings.BatteryLevelLostWhenCritical);
            }
            else
            {
                float wishedUpdateAdjustentFactor = 1;
                if (currenBoardRatio < PropolisGameSettings.MinStableDifficultyThreshold)
                {
                    wishedUpdateAdjustentFactor = 1.15f;
                }
                else if(currenBoardRatio > PropolisGameSettings.MaxStableDifficultyThreshold)
                {
                    wishedUpdateAdjustentFactor = 0.85f;
                }
                float numberOfWishedUpdate = PropolisGameSettings.TargetIntervalBetweenClimaxes / PropolisGameSettings.BatteryUpdateDeltaTime;
                //Debug.Log(string.Format("---------NumberOfWishedUpdate :{0} ---------", numberOfWishedUpdate));
                //Debug.Log(string.Format("---------BoardRatio :{0} ---------", currenBoardRatio));
                //Debug.Log(string.Format("---------BufferCalculation :{0} ---------", numberOfWishedUpdate * currenBoardRatio * 2f));
                float BatteryIncrement = 1 / (numberOfWishedUpdate * wishedUpdateAdjustentFactor);
                //Debug.Log(string.Format("---------BatteryIncrement :{0} ---------", BatteryIncrement));

                IncrementBatteryLevel(BatteryIncrement);
            }

            //Debug.Log("---------------EndOfNewBatteryUpdate---------------");
        }
            
        
    }

    private IEnumerator ProcessDifficultyLevel()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(PropolisGameSettings.DifficultyUpdateDeltaTime);
            float ratioOfActiveHex = hiveGameController.GetRatioOfGivenPropolisStatus(PropolisStatus.ON);
            if (ratioOfActiveHex < PropolisGameSettings.MinStableDifficultyThreshold)
            {
                PropolisGameSettings.CurrentDifficultyMultiplier -= PropolisGameSettings.DifficultyModifier *2.0f;
            }
            else if (ratioOfActiveHex > PropolisGameSettings.MaxStableDifficultyThreshold)
            {
                PropolisGameSettings.CurrentDifficultyMultiplier += PropolisGameSettings.DifficultyModifier;

                if (PropolisGameSettings.CurrentDifficultyMultiplier > PropolisGameSettings.MaxDifficulty)
                    PropolisGameSettings.CurrentDifficultyMultiplier = PropolisGameSettings.MaxDifficulty;
            }

            PropolisGameSettings.CurrentDifficultyMultiplier = PropolisGameSettings.CurrentDifficultyMultiplier < 1f ? 1f : PropolisGameSettings.CurrentDifficultyMultiplier;

        }
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
        ((RecipeGameController)recipeGameController).UpdateFromNewRecipe();
       
    }

    private void StopGame()
    {
        Debug.Log("stop");
        hiveGameController.Stop();
        molecularGameController.Stop();
        recipeGameController.Stop();
        StopCoroutine(GameLoopCoroutine);
        AlertUiController.Show("Propolis Event", "Gameplay Stopped");

    }

    public IEnumerator ProcessGameLoop()
    {
        while (true)
        {
            hiveGameController.UpdateGameLogic();
            molecularGameController.UpdateGameLogic();
            recipeGameController.UpdateGameLogic();
            //Debug.Log("looping game loop");
            yield return new WaitForSeconds(Mathf.Max(PropolisGameSettings.DefaultGameTickTime / PropolisGameSettings.CurrentDifficultyMultiplier));
        }

    }


   
    public void IncrementBatteryLevel(float increment)
    {
        float futureBatteryLevel = propolisData.BatteryLevel + (increment);

        futureBatteryLevel = Mathf.Clamp(futureBatteryLevel, 0, 1);
       
        if (futureBatteryLevel >= 1 || futureBatteryLevel < 0)
        {
            if (futureBatteryLevel >= 1) {
                PropolisStatsExporter.IncrementStatValue("ReservoirFilled");
                animator.StartClimax();
            }
            SendCommand(string.Format("{0} {1}", PropolisActions.SetBatteryLevel, 0));
        }
        else
        {
            SendCommand(string.Format("{0} {1}", PropolisActions.SetBatteryLevel, futureBatteryLevel));
        }
        
    }

    public void ProcessUserInteraction(string type, AbstractItem item, PropolisUserInteractions userAction)
    {
        if (PropolisData.Instance.IsGamePlaying) {

            switch (type)
            {
                case PropolisDataTypes.HexGroup: hiveGameController.ProcessUserInteraction(item, userAction); break;
                case PropolisDataTypes.AtomGroup: molecularGameController.ProcessUserInteraction(item, userAction); break;
                case PropolisDataTypes.RecipeGroup: recipeGameController.ProcessUserInteraction(item, userAction); break;
            }
        }

    }
   

 
}
