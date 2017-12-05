using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;


namespace Propolis
{
    public class PropolisExport : MonoBehaviour
    {



    // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ExportFromModel()
        {
            if (Propolis.PropolisData.Instance.IsGamePlaying)
            {
                if (PropolisData.Instance.LastEvent.Action== PropolisActions.UpdateItemStatus)
                {
                    PropolisLastEventState lastEvent = Propolis.PropolisData.Instance.LastEvent;
                    SendOscMessage(
                        "/" + lastEvent.Type,
                        lastEvent.GroupID, lastEvent.ID,
                        PropolisData.Instance.GetItemDataById(lastEvent.GroupID, lastEvent.ID, lastEvent.Type).Status);
                }
                else if (PropolisData.Instance.LastEvent.Action == PropolisActions.SetBatteryLevel)
                {
                    SendBatteryOscMessage("/battery",PropolisData.Instance.BatteryLevel);
                }

            }
        }

        private void SendOscMessage(string address, int value, int value2, int value3)
        {


        }

        private void SendBatteryOscMessage(string address, float value)
        {


        }
    }

}

