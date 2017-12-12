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
                Debug.Log("Invalid shield atom id");
            }
        }
        
    }

    private void ProcessLevel2Climax(AbstractGroup group, PropolisRecipe recipe)
    {
        StopCoroutine(PlayLevel2Climax(group, recipe));
        StartCoroutine(PlayLevel2Climax(group, recipe));
    }



    private IEnumerator PlayLevel2Climax(AbstractGroup group, PropolisRecipe recipe)
    {
        foreach (var item in group.ChildItemsList)

        {
            if (item.ID != 9)
            {
                SendItemData(group.ID, item.ID,PropolisStatus.OFF);
                yield return new WaitForSeconds(0.05f);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            foreach (var item in group.ChildItemsList)

            {
                if (item.ID != 9)
                {
                    SendItemData(group.ID, item.ID, (PropolisStatus)recipe.GetItem(i));
                    yield return new WaitForSeconds(0.02f);
                }
            }

            yield return new WaitForSeconds(0.05f);

            foreach (var item in group.ChildItemsList)

            {
                if (item.ID != 9)
                {
                    SendItemData(group.ID, item.ID, PropolisStatus.OFF);
                    yield return new WaitForSeconds(0.02f);
                }
            }

        }
        yield return new WaitForSeconds(0.05f);
        foreach (var item in group.ChildItemsList)

        {
            if (item.ID != 9)
            {
                SendItemData(group.ID, item.ID, PropolisStatus.CLEANSER);
                yield return new WaitForSeconds(0.02f);
            }
        }

        yield return new WaitForSeconds(0.5f);
        foreach (var item in group.ChildItemsList)

        {
            if (item.ID != 9)
            {
                SendItemData(group.ID, item.ID, random.Next(3)+PropolisStatus.RECIPE1);
            }
        }


    }
    private void ValidateRecipe(AbstractGroup group)
    {
       
    }
    public override void ProcessUserInteraction(AbstractItem item, PropolisUserInteractions userAction)
    {
        if (userAction == PropolisUserInteractions.PRESS)
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
                    default: SendItemData(item.ParentGroup.ID, item.ID, item.Status); break;
                }

                try
                {
                    AbstractGroup group = ListOfGroups.Find(x => x.ID == PropolisData.Instance.LastEvent.GroupID);
                    PropolisRecipe recipe = PropolisRecipe.ParseRecipe(group);
                    PropolisRecipeCompareStatus compareResult = PropolisData.Instance.RecipeStack.ToArray()[1].CompareTo(recipe);

                    if(recipe != null)
                    {
                        if (compareResult == PropolisRecipeCompareStatus.PERFECT)
                        {
                            ProcessLevel2Climax(group, recipe);
                            GameController.PushRecipe();
                            GameController.ProcessSuccessfulRecipe(compareResult);
                        }
                        else
                        {
                            ProcessLevel2Climax(group, recipe);
                            GameController.PushRecipe();
                            GameController.ProcessSuccessfulRecipe(PropolisRecipeCompareStatus.IMPERFECT);
                        }
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


        }else if(userAction == PropolisUserInteractions.PULL_OFF)
        {
            if (item.IsShield)
            {
                SendItemData(item.ParentGroup.ID, item.ID, PropolisStatus.SHIELD_OFF);
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
        SetAllItemsTo(PropolisStatus.OFF);
        ListOfGroups.ForEach(x => x.ChildItemsList.ForEach(y => { if (y.ID == 9) { SendItemData(x.ID, y.ID, PropolisStatus.SHIELD_OFF); } }));
        GenerateWaveGameController();
        StopAllCoroutines();
        StartCoroutine(ProcessWaveTrigger());
        StartCoroutine(ProcessWaveMovement());
    }

    public override void Stop()
    {
        StopAllCoroutines();

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
