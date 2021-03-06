﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Linq;
using System;

public class MolecularGameController : AbstractGameController
{
    public GameObject WaveGameObject;
    private GameObject WaveGameObjectInstance;
    private WaveController WaveController;
    private System.Random random;
    public PropolisAlertUIController AlertController;
    public List<float> DistancesFromWave;
    private Coroutine _waveCoroutine;
    private Vector3 WaveOriginalTransform;
    private Coroutine waveCoroutine;
    //To be used instead of Update or FixedUpdate.

    private void Start()
    {
        WaveGameObjectInstance = Instantiate(WaveGameObject);
        WaveController = WaveGameObjectInstance.GetComponent<WaveController>();
    }
    public   override void UpdateGameLogic()
    {

    }

    public void ProcessDistancesFromMoleculeAndWave()
    {
        DistancesFromWave = new List<float>();
        foreach (var group in ListOfGroups)
        {
            DistancesFromWave.Add(( WaveController.transform.position.x - group.transform.position.x )/ ((GameArea.x + GameArea.width * PropolisGameSettings.WaveInitialDistanceRatioFromGameSize) - group.transform.position.x));

        }
    }

    public override void UpdateFromModel()
    {

        base.UpdateFromModel();
        if (PropolisData.Instance.LastEvent.Type == PropolisDataTypes.AtomGroup &&
            PropolisData.Instance.LastEvent.Action == PropolisActions.UpdateItemStatus && 
            PropolisData.Instance.LastEvent.ID == 9)
        {
            PropolisGroupItemData groupData = PropolisData.Instance.GetItemDataById(PropolisData.Instance.LastEvent.GroupID, PropolisData.Instance.LastEvent.ID, PropolisDataTypes.AtomGroup);
            try
            {
                AbstractGroup molecule = ListOfGroups.Find(x => x.ID == PropolisData.Instance.LastEvent.GroupID);
                molecule.IsLocked = groupData.Status == (int)PropolisStatus.SHIELD_ON;
            }
            catch (System.Exception)
            {
                //Debug.Log("Invalid shield atom id");
            }
        }
        
    }

    private void ProcessLevel2Climax(AbstractGroup group, PropolisRecipe recipe)
    {
        PropolisStatsExporter.IncrementStatValue("AtomRecipeDone");
        StartCoroutine(PlayLevel2Climax(group, recipe));
    }



    private IEnumerator PlayLevel2Climax(AbstractGroup group, PropolisRecipe recipe)
    {
        group.IsPlayingAnimation = true;
        yield return new WaitForSecondsRealtime(0.8f);
        foreach (var item in group.ChildItemsList)

        {
            if (item.ID != 9)
            {
                SendItemData(group.ID, item.ID, PropolisStatus.ON);

            }
        }


        yield return new WaitForSecondsRealtime(0.7f);
        foreach (var item in group.ChildItemsList)

        {
            if (item.ID != 9)
            {
                SendItemData(group.ID, item.ID,PropolisStatus.ANIM_BLACK);

            }
        }

        yield return new WaitForSecondsRealtime(0.7f);

        {
            foreach (var item in group.ChildItemsList)

            {
                if (item.ID != 9)
                {

                    SendItemData(group.ID, item.ID, (PropolisStatus)recipe.GetItem((int)(item.ID /3)));
                    yield return new WaitForSecondsRealtime(0.2f);
                    SendItemData(group.ID, item.ID, PropolisStatus.ANIM_BLACK);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }

           

        }


        yield return new WaitForSeconds(1f);

        foreach (var item in group.ChildItemsList)

        {
            if (item.ID != 9)
            {
                SendItemData(group.ID, item.ID, random.Next(3) + PropolisStatus.RECIPE1);
            }
        }

        group.IsPlayingAnimation = false;
    }
    private void RandomizeAtoms()
    {
        foreach (var group in ListOfGroups)
        {
            foreach (var item in group.ChildItemsList)

            {
                if (item.ID != 9)
                {
                    SendItemData(group.ID, item.ID, random.Next(3) + PropolisStatus.RECIPE1);
                }
            }
        }

    }

    private PropolisStatus ConvertRecipeToBlinking(PropolisStatus status)
    {
        return (PropolisStatus)(status + 5);
    }

    public void ReceivedRecipeInteracton(int parentID, int itemId, PropolisStatus status)
    {
       // StartCoroutine(AnimateBlinking(parentID, itemId, status));
    }

    //private IEnumerator AnimateBlinking(int parentID, int itemId, PropolisStatus status)
    //{

    //    //AbstractGroup group = ListOfGroups.First<AbstractGroup>(x => x.ID == parentID);
    //    //List<AbstractItem> items = group.ChildItemsList.Where<AbstractItem>(x => x.ID >= (itemId * 3)  && x.ID <= (itemId * 3 + 2)).ToList<AbstractItem>();
    //    //List<PropolisStatus> previousStatus = new List<PropolisStatus>();


    //    //foreach (var item in items)
    //    //{
    //    //    previousStatus.Add((PropolisStatus)item.status);
    //    //    item.StatusBackup = ((PropolisStatus)item.status == PropolisStatus.WAVECORRUPTED ? PropolisStatus.CORRUPTED : (PropolisStatus)item.status);
    //    //    SendItemData(group.ID, item.ID, ConvertRecipeToBlinking(status));
    //    //    item.StatusLocked = true;
    //    //}

    //    //yield return new WaitForSeconds(PropolisGameSettings.RecipeBlinkingHintTime);

    //    //for (int i = 0; i < items.Count; i++)
    //    //{
    //    //    SendItemData(group.ID, items[i].ID, (PropolisStatus)items[i].StatusBackup);
    //    //    items[i].StatusLocked = false;
    //    //}
    //}

    public void ReceiveShieldInteractionFromHive(int itemId, PropolisUserInteractions action)
    {
        try
        {
            AbstractGroup group = ListOfGroups.First<AbstractGroup>(x => x.ID == itemId);
            AbstractItem item = group.ChildItemsList.First<AbstractItem>(x => x.ID == 9);

            if (action == PropolisUserInteractions.PRESS)
            {
                item.StatusLocked = true;
                SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SHIELD_ON);
            }
            else
            {
                item.StatusLocked = false;
                SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SHIELD_OFF);
            }


        }
        catch (System.Exception)
        {


        }

    }

    public override void ProcessUserInteraction(AbstractItem item, PropolisUserInteractions userAction)
    {
        if (userAction == PropolisUserInteractions.PRESS && !item.ParentGroup.IsPlayingAnimation && !item.StatusLocked)
        {
            if (!item.IsShield)
            {
                switch (item.Status)
                {
                    case PropolisStatus.OFF: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE1); break;
                    case PropolisStatus.CORRUPTED: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE1); break;
                    case PropolisStatus.RECIPE1: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE2); break;
                    case PropolisStatus.RECIPE2: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE3); break;
                    case PropolisStatus.RECIPE3: SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.RECIPE1); break;
                    //default: SendItemData(item.ParentGroup.ID, item.ID, item.Status); break;
                }

                try
                {
                    AbstractGroup group = ListOfGroups.Find(x => x.ID == PropolisData.Instance.LastEvent.GroupID);
                    PropolisRecipe recipe = PropolisRecipe.ParseRecipe(group);
                    PropolisRecipeCompareStatus compareResult = PropolisData.Instance.RecipeStack.ToArray()[1].CompareTo(recipe);

                    if(recipe != null)
                    {

                        GameController.PropolisExport.SendRecipeEventToHUDSAndSound(group.ID, 2);
                        GameController.PropolisExport.SendSuccessfulRecipeToHUD(group, recipe);

                        ProcessLevel2Climax(group, recipe);
                        AlertController.Show("Propolis Event", "Climax Molecular level 2");

                        GameController.ProcessSuccessfulRecipe(PropolisRecipeCompareStatus.IMPERFECT);
                        
                    }


                    
                }
                catch (System.Exception)
                {


                }

            }
            else
            {
                SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SHIELD_ON);
            }


        }else if(userAction == PropolisUserInteractions.PULL_OFF /*&&  !item.StatusLocked*/)
        {
            if (item.IsShield)
            {
                item.TriggerDelayedStatus(PropolisStatus.SHIELD_OFF, PropolisGameSettings.ShieldDeactivationDelay);
            }
        }
    }
    public override void InitOnPlay()
    {
        base.InitOnPlay();
        random = new System.Random();
        Reset();    
        
    }

    private void Reset()
    {
        SetWaveActiveStatus(false);
        KillWave();
        ResetShields();
        RandomizeAtoms();
        StopAllCoroutines();
        GenerateWaveGameController();
        DistancesFromWave = new List<float>();
        SetWavePosition(0.0f);
        StartCoroutine(ProcessWaveTrigger());
        StartCoroutine(ProcessWaveMovement());
        ValidateStatusFromPlay();
    }

    public override void Stop()
    {
        SetWaveActiveStatus(false);
        KillWave();
        StopAllCoroutines();
        StopCoroutine(ProcessWaveTrigger());
        StopCoroutine(ProcessWaveMovement());
    }
    public void ResetShields()
    {
        ListOfItems.ForEach(x =>
        {
            if (x.ID == 9)
            {
                SendItemData(x.ParentGroup.ID, x.ID, PropolisStatus.SHIELD_OFF);
            }
        });
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

        yield return new WaitForSeconds(PropolisGameSettings.IntervalBetweenWaves);
        SetWavePosition(0.0f);
        SetWaveActiveStatus(true);
        
    }

    private void GenerateWaveGameController()
    {

        WaveGameObjectInstance.transform.position = new Vector3(GameArea.x + GameArea.width * PropolisGameSettings.WaveInitialDistanceRatioFromGameSize, GameArea.y + GameArea.height * .5f);
        WaveGameObjectInstance.transform.localScale = new Vector3(1, GameArea.height, 1);

        BoxCollider2D waveBoxCollider = WaveGameObjectInstance.GetComponent<BoxCollider2D>();
        waveBoxCollider.size = new Vector2(1.0f, GameArea.height);
    }   

    public void CorruptedAtomWithWave(AbstractItem atom)
    {
        if(!atom.IsShield && !atom.ParentGroup.IsLocked && !atom.ParentGroup.IsPlayingAnimation)
        {
            StartCoroutine(ProcessAtomCorruptionProgress(atom));
        }
    }

    public IEnumerator ProcessAtomCorruptionProgress(AbstractItem atom)
    {
        if (!atom.StatusLocked)
        {
            SendItemData(atom.ParentGroup.ID, atom.ID, PropolisStatus.WAVECORRUPTED);
            yield return new WaitForSeconds(PropolisGameSettings.AtomSaturationCorruptionTime);
            SendItemData(atom.ParentGroup.ID, atom.ID, PropolisStatus.CORRUPTED);
        }
        else {
            atom.StatusBackup = PropolisStatus.CORRUPTED;
        }

    }

    private IEnumerator ProcessWaveMovement()
    {
        while (true) { 
            WaveController.UpdateMovement();
            ProcessDistancesFromMoleculeAndWave();
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void KillWave()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }

    }
    public void BeginNewWaveCountdown()
    {
        KillWave();
        waveCoroutine =StartCoroutine(ProcessWaveTrigger());


    }
    

}
