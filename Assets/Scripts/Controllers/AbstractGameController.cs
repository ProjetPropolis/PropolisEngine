﻿using System;
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
            propolisData.HexGroupList.ForEach(x=>InstantiateHexGroup(x.ID));
        }
        private void UpdateAbstractGroup()
        {
            AbstractGroupData abstractGroupData = null;
            if(GroupDataType == PropolisDataTypes.HexGroup)
            {
                abstractGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID);
            }

            if(abstractGroupData != null)
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
        protected abstract void UpdateGameLogic();
      
        private void UpdateAbstractGroupItemStatus()
        {
            AbstractGroupData abstractGroupData = null;
            if (GroupDataType == PropolisDataTypes.HexGroup)
            {
                abstractGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID);
            }

            foreach (AbstractGroup ag in ListOfGroups)
            {
                if(ag.ID == propolisData.LastEvent.GroupID)
                {
                    foreach (AbstractItem  ai in ag.ChildHexsList)
                    {
                        foreach (var itemData in abstractGroupData.Childrens)
                        {
                            if (ai.ID == itemData.ID)
                            {
                                ai.Status = (PropolisStatus)itemData.Status;
                            }
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
            }
        }

        private void ProcessCreationElement()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: InstantiateHexGroup(propolisData.LastEvent.ID); break;
            }
        }

        private void ProcessSupressionElement()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: DeleteGroup(); break;
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

        private void InstantiateHexGroup(int ID)
        {

            AbstractGroupData abstractGroupData = propolisData.GetGroupDataById(ID,GroupDataType);
            if (abstractGroupData != null)
            {
                GameObject gameObject = Instantiate(GroupsPrefab, abstractGroupData.GetPosition(), Quaternion.identity);
   	            AbstractGroup abstractGroup = gameObject.GetComponent<AbstractGroup>();
                //  abstractGroup.transform.parent = GameViewTransform.transform;
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