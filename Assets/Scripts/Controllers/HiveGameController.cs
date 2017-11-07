using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Propolis
{
    public class HiveGameController : MonoBehaviour {

        public GameController GameController;
        public PropolisData propolisData;
        public GameObject hexGroupPrefab;

        // Use this for initialization
        void Start() {

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
                case PropolisDataTypes.HexGroup: break;
            }
        }

        private void InstantiateHexGroup()
        {
            
            HexGroupData hexGroupData = propolisData.GetHexGroupDataById(propolisData.LastEvent.ID);
            if(hexGroupData != null)
                Instantiate(hexGroupPrefab, hexGroupData.Position, Quaternion.identity);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
