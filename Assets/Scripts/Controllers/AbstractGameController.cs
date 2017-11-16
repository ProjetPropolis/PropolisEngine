using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Propolis
{
    public class AbstractGameController : MonoBehaviour {

        public GameController GameController;
        public PropolisData propolisData;
        public GameObject GroupsPrefab;
        public Transform GameViewTransform; 
        public List<AbstractGroup> ListGroups;


        // Use this for initialization
        void Start() {
            propolisData = PropolisData.Instance;
            ListGroups = new List<AbstractGroup>();
        }

        public void UpdateFromModel()
        {
            propolisData = PropolisData.Instance;
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
            AbstractGroupData hexGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID);

            ListGroups.ForEach(
                x =>
                {
                    if (x.ID == hexGroupData.ID)
                    {
                        x.transform.position = hexGroupData.GetPosition();
                        x.Osc.inPort = hexGroupData.InPort;
                        x.Osc.outPort = hexGroupData.OutPort;
                        x.Osc.outIP = hexGroupData.IP;
                    }   
                }
            );
        }
        private void UpdateAbstractGroupItemStatus()
        {
            //if all hex of an hexgroup have changed
            AbstractGroupData abstractGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID);

            foreach (AbstractGroup ag in ListGroups)
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
                case PropolisDataTypes.HexGroup: DeleteHexGroup(); break;
            }
        }

        private void DeleteHexGroup()
        {
            try
            {
                AbstractGroup abstractGroup = ListGroups.First(x => x.ID == propolisData.LastEvent.ID);
                ListGroups.Remove(abstractGroup);

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

            AbstractGroupData hexGroupData = propolisData.GetHexGroupDataById(ID);
            if (hexGroupData != null)
            {
                GameObject gameObject = Instantiate(GroupsPrefab, hexGroupData.GetPosition(), Quaternion.identity);
   	            AbstractGroup abstractGroup = gameObject.GetComponent<AbstractGroup>();
                abstractGroup.transform.parent = GameViewTransform.transform;
                abstractGroup.ID = hexGroupData.ID;
                abstractGroup.Osc.inPort = hexGroupData.InPort;
                abstractGroup.Osc.outPort = hexGroupData.OutPort;
                abstractGroup.Osc.outIP = hexGroupData.IP;
                ListGroups.Add(abstractGroup);
                gameObject.SetActive(true);
            }

            
        }

        void DeleteAllComponents()
        {
            for (int i = ListGroups.Count; i >0 ; i--)
            {
                Destroy(ListGroups[i-1].gameObject);
                ListGroups.RemoveAt(i-1);
            }
            
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
