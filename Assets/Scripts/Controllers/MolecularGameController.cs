using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class MolecularGameController : AbstractGameController
{
    public GameObject WaveGameObject;
    private GameObject WaveGameObjectInstance;
    private WaveController WaveController;
    private System.Random random;
    //To be used instead of Update or FixedUpdate.

    private void Start()
    {
        WaveGameObjectInstance = Instantiate(WaveGameObject);
        WaveController = WaveGameObjectInstance.GetComponent<WaveController>();
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
        Reset();       
    }

    private void Reset()
    {
        SetAllItemsTo(PropolisStatus.OFF);
        GenerateWaveGameController();
        StartCoroutine(ProcessWaveTrigger());
        StartCoroutine(ProcessWaveMovement());
    }

    public override void Stop()
    {
        StopCoroutine(ProcessWaveTrigger());
        StartCoroutine(ProcessWaveMovement());

    }

    public void SetWavePosition(float position)
    {
        position = Mathf.Clamp(position, 0.0f, 1.0f);
        SendCommand(string.Format("{0} {1}", PropolisActions.SetWavePosition, position));
    }

    public void SetWaveActiveStatus(bool status)
    {
        SendCommand(string.Format("{0} {1}", PropolisActions.SetWaveActiveStatus, status));
    }

    private IEnumerator ProcessWaveTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(PropolisGameSettings.IntervalBetweenWaves);
            SetWavePosition(0.0f);
            SetWaveActiveStatus(true);
        }
    }

    private void GenerateWaveGameController()
    {

        WaveGameObjectInstance.transform.position = new Vector3(GameArea.x + GameArea.width, GameArea.y + GameArea.height * .5f);

        WaveGameObjectInstance.transform.localScale = new Vector3(1, GameArea.height, 1);


        BoxCollider2D waveBoxCollider = WaveGameObjectInstance.GetComponent<BoxCollider2D>();
        waveBoxCollider.size = new Vector2(1.0f, GameArea.height);
       

    }   

    public void CorruptedAtomWithWave(AbstractItem atom)
    {
        if(!atom.IsShield && !atom.ParentGroup.IsLocked)
        {
            StartCoroutine(ProcessAtomCorruptionProgress(atom));
        }
    }

    public IEnumerator ProcessAtomCorruptionProgress(AbstractItem atom)
    {
        SendItemData(atom.ParentGroup.ID, atom.ID, PropolisStatus.WAVECORRUPTED);
        yield return new WaitForSeconds(PropolisGameSettings.AtomSaturationCorruptionTime);
        SendItemData(atom.ParentGroup.ID, atom.ID, PropolisStatus.CORRUPTED);
    }

    private IEnumerator ProcessWaveMovement()
    {
        while (true) { 
            WaveController.UpdateMovement();
            yield return new WaitForSeconds(0.03f);
        }
    }

}
