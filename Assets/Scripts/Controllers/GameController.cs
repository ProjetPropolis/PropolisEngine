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
                case PropolisActions.Play: break;
                case PropolisActions.Stop: break;
            }
        

        hiveGameController.UpdateFromModel();
        molecularGameController.UpdateFromModel();
    }

    private void  StartGame()
    {
        StartCoroutine(GameLoopCoroutine);
    }

    private void StopGame()
    {
        StopCoroutine(GameLoopCoroutine);
    }

    public IEnumerator ProcessGameLoop()
    {
        while (true)
        {
            hiveGameController.UpdateGameLogic();
            molecularGameController.UpdateGameLogic();

            yield return new WaitForSeconds(PropolisGameSettings.DefaultGameTickTime);
        }
    }

    private void ProcessGameEventToBatteryLevel()
    {

    }

   
    private void IncrementBatteryLevel(float increment)
    {
        SendCommand(string.Format("{0} {1}",PropolisActions.SetBatteryLevel,propolisData.BatteryLevel + increment));
    }
    
	void Update () {
    
    }

 
}
