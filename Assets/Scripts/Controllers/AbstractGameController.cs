using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Propolis
{
    public abstract class AbstractGameController : MonoBehaviour {

        public GameController GameController;
        public PropolisData propolisData;
        public GameObject GroupsPrefab;
        public Transform GameViewTransform; 
        public List<AbstractGroup> ListOfGroups;
        public string GroupDataType;


        // Use this for initialization
        void Awake() {
            propolisData = PropolisData.Instance;
            ListOfGroups = new List<AbstractGroup>();
            GroupDataType = GroupsPrefab.GetComponent<AbstractGroup>().DataType;
        }

        public void UpdateFromModel()
        {
            propolisData = PropolisData.Instance;
            if (propolisData.LastEvent.Type != GroupDataType && propolisData.LastEvent.Type != String.Empty)
                return;

            switch (propolisData.LastEvent.Action)
            {
                case PropolisActions.Create: ProcessCreationElement();break ;
                case PropolisActions.Delete: ProcessSupressionElement(); break;
                case PropolisActions.UpdateItemStatus: UpdateAbstractGroupItemStatus(); break;
                case PropolisActions.Update: UpdateAbstractGroup(); break;
                case PropolisActions.Load: LoadFromData(); break;
            }
        }

        private void LoadFromData()
        {
            DeleteAllComponents();
            propolisData.HexGroupList.ForEach(x=> InstantiateAbstractGroupGroup(x.ID));
        }
        private void UpdateAbstractGroup()
        {
            AbstractGroupData abstractGroupData = null;
            switch (GroupDataType)
            {
                case PropolisDataTypes.HexGroup: abstractGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.ID); break;
                case PropolisDataTypes.AtomGroup: abstractGroupData = propolisData.AtomGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.ID); break;

            }


            if (abstractGroupData != null)
            {
                ListOfGroups.ForEach(
                x =>
                {
                    if (x.ID == abstractGroupData.ID)
                    {
                        x.transform.position = abstractGroupData.GetPosition();
                        x.Osc.inPort = abstractGroupData.InPort;
                        x.Osc.outPort = abstractGroupData.OutPort;
                        x.Osc.outIP = abstractGroupData.IP;
                    }
                }
            );
            }      
            
        }
        public abstract void UpdateGameLogic();
        public virtual void InitOnPlay()
        {
            ListOfGroups.ForEach(x => x.ChildHexsList.ForEach(y => y.CalculateNeighborsList()));
        }

        public virtual void Stop()
        {

        }

        private void UpdateAbstractGroupItemStatus()
        {
            AbstractGroupData abstractGroupData = null;
            switch (GroupDataType)
            {
                case PropolisDataTypes.HexGroup: abstractGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID);break;
                case PropolisDataTypes.AtomGroup: abstractGroupData = propolisData.AtomGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID); break;

            }

            foreach (AbstractGroup ag in ListOfGroups)
            {
                if (ag.ID == propolisData.LastEvent.GroupID)
                {
                    foreach (AbstractItem ai in ag.ChildHexsList)
                    {
                    
                        if (ai.ID == propolisData.LastEvent.ID)
                        {
                            ai.Status = (PropolisStatus)abstractGroupData.Childrens.FirstOrDefault(x=>x.ID == ai.ID).Status;
                        }
                        
                    }
                }

            }



        }

        private void ProcessUpdateItemStatus()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: UpdateAbstractGroupItemStatus(); break;
                case PropolisDataTypes.AtomGroup: UpdateAbstractGroupItemStatus(); break;
            }
        }

        private void ProcessCreationElement()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: InstantiateAbstractGroupGroup(propolisData.LastEvent.ID); break;
                case PropolisDataTypes.AtomGroup: InstantiateAbstractGroupGroup(propolisData.LastEvent.ID); break;
            }
        }

        private void ProcessSupressionElement()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: DeleteGroup(); break;
                case PropolisDataTypes.AtomGroup: DeleteGroup(); break;
            }
        }

        private void DeleteGroup()
        {
            try
            {
                 AbstractGroup abstractGroup = ListOfGroups.First(x => x.ID == propolisData.LastEvent.ID);
                ListOfGroups.Remove(abstractGroup);

                Destroy(abstractGroup.transform.gameObject);
            }
            catch
            {

            }        
        }

        public void SendCommand(string command)
        {
            GameController.SendCommand(command);
        }

        private void InstantiateAbstractGroupGroup(int ID)
        {

            AbstractGroupData abstractGroupData = propolisData.GetGroupDataById(ID,GroupDataType);
            if (abstractGroupData != null)
            {
                GameObject gameObject = Instantiate(GroupsPrefab, abstractGroupData.GetPosition(), Quaternion.identity);
   	            AbstractGroup abstractGroup = gameObject.GetComponent<AbstractGroup>();
                //gameObject.transform.parent = GameViewTransform.transform;
                abstractGroup.ID = abstractGroupData.ID;
                abstractGroup.Osc.inPort = abstractGroupData.InPort;
                abstractGroup.Osc.outPort = abstractGroupData.OutPort;
                abstractGroup.Osc.outIP = abstractGroupData.IP;
                ListOfGroups.Add(abstractGroup);
                gameObject.SetActive(true);
            }

            
        }

        void DeleteAllComponents()
        {
            for (int i = ListOfGroups.Count; i > 0; i--)
            {
                Destroy(ListOfGroups[i - 1].gameObject);
                ListOfGroups.RemoveAt(i - 1);
            }

        }

        public void SendItemData(int groupID, int itemID, PropolisStatus status)
        {
            GameController.SendCommand(String.Format("uis {0} {1} {2} {3}", GroupDataType, groupID, itemID, (int)status));

        }

    }
}
