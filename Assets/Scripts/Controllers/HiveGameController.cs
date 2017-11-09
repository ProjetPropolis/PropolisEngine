using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Propolis
{
    public class HiveGameController : MonoBehaviour {

        public GameController GameController;
        public PropolisData propolisData;
        public GameObject hexGroupPrefab;
        public Transform HiveViewTransform;
        public List<HexGroup> ListHexGroup;


        // Use this for initialization
        void Start() {
            propolisData = PropolisData.Instance;
            ListHexGroup = new List<HexGroup>();
        }

        public void UpdateFromModel()
        {
            switch (propolisData.LastEvent.Action)
            {
                case PropolisActions.Create: ProcessCreationElement();break ;
                case PropolisActions.Delete: ProcessSupressionElement(); break;
                case PropolisActions.UpdateItemStatus: UpdateHexGroupItemStatus(); break;
            }
        }

        private void UpdateHexGroupItemStatus()
        {
            //if all hex of an hexgroup have changed
            HexGroupData hexGroupData = propolisData.HexGroupList.FirstOrDefault(x => x.ID == propolisData.LastEvent.GroupID);

            foreach (HexGroup hg in ListHexGroup)
            {
                if(hg.ID == propolisData.LastEvent.GroupID)
                {
                    foreach (Hex hex in hg.ChildHexsList)
                    {
                        foreach (var hexData in hexGroupData.Childrens)
                        {
                            if (hex.ID == hexData.ID)
                            {
                                hex.Status = (PropolisStatus)hexData.Status;
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
                case PropolisDataTypes.HexGroup: UpdateHexGroupItemStatus(); break;
            }
        }

        private void ProcessCreationElement()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: InstantiateHexGroup(); break;
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
                HexGroup hexgroup = ListHexGroup.First(x => x.ID == propolisData.LastEvent.ID);
                ListHexGroup.Remove(hexgroup);

                Destroy(hexgroup.transform.gameObject);
            }
            catch
            {

            }        
        }

        public void SendCommand(string command)
        {
            GameController.SendCommand(command);
        }

        private void InstantiateHexGroup()
        {

            HexGroupData hexGroupData = propolisData.GetHexGroupDataById(propolisData.LastEvent.ID);
            if (hexGroupData != null)
            {
                GameObject gameObject = Instantiate(hexGroupPrefab, hexGroupData.Position, Quaternion.identity);
   	              HexGroup hexGroup = gameObject.GetComponent<HexGroup>();
                hexGroup.transform.parent = HiveViewTransform;
                hexGroup.ID = hexGroupData.ID;
                hexGroup.Osc.inPort = hexGroupData.InPort;
                hexGroup.Osc.outPort = hexGroupData.OutPort;
                hexGroup.Osc.outIP = hexGroupData.IP;
                ListHexGroup.Add(hexGroup);
                gameObject.SetActive(true);
            }

            
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
