using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Propolis
{
    public class HiveGameController : MonoBehaviour {

        public GameController GameController;
        public PropolisData propolisData;
        public GameObject hexGroupPrefab;
        public Transform HiveViewTransform;


        // Use this for initialization
        void Start() {
            propolisData = PropolisData.Instance;
        }

        public void UpdateFromModel()
        {
            switch (propolisData.LastEvent.Action)
            {
                case PropolisActions.Create: ProcessCreationElement();break ;
            }
        }

        private void ProcessCreationElement()
        {
            switch (propolisData.LastEvent.Type)
            {
                case PropolisDataTypes.HexGroup: InstantiateHexGroup(); break;
            }
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

            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
