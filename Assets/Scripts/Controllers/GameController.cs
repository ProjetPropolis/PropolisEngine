using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Propolis;

public class GameController : MonoBehaviour {

    List<GameObject> tuilesActives = new List<GameObject>();
    public PropolisManager propolisManager;

    public AbstractGameController hiveGameController, molecularGameController;
    public PropolisData propolisData;
    private IEnumerator GameLoopCoroutine;


	void Start () {
        propolisData = PropolisData.Instance;
        GameLoopCoroutine = ProcessGameLoop();
    }

    public void SendCommand(string command)
    {
        propolisManager.SendCommand(command);
    }


    public void UpdateFromModel()
    {
        propolisData = PropolisData.Instance;

            switch (propolisData.LastEvent.Action)
            {

                case PropolisActions.UpdateItemStatus: ProcessGameEventToBatteryLevel(); break;
                case PropolisActions.Play:StartGame(); break;
                case PropolisActions.Stop:StopGame(); break;
            }
        

        hiveGameController.UpdateFromModel();
        molecularGameController.UpdateFromModel();

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
        StopCoroutine(GameLoopCoroutine);
    }

    public IEnumerator ProcessGameLoop()
    {
        while (true)
        {
            hiveGameController.UpdateGameLogic();
            molecularGameController.UpdateGameLogic();
            Debug.Log("looping game loop");
            yield return new WaitForSeconds(PropolisGameSettings.DefaultGameTickTime);
        }

    }

    private void ProcessGameEventToBatteryLevel()
    {

    }

   
    public void IncrementBatteryLevel(float increment)
    {
        SendCommand(string.Format("{0} {1}",PropolisActions.SetBatteryLevel,propolisData.BatteryLevel + increment));
    }
    
	void Update () {
    
    }

 
}
