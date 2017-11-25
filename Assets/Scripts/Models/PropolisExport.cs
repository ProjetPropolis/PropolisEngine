using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;


namespace Propolis
{
    public class PropolisExport : MonoBehaviour
    {

        public OSC SoundMaxOsc;


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
            if (Propolis.PropolisData.Instance.IsGamePlaying && PropolisData.Instance.LastEvent.Action == PropolisActions.UpdateItemStatus)
            {
                PropolisLastEventState lastEvent = Propolis.PropolisData.Instance.LastEvent;
                SendOscMessage(
                    "/"+ lastEvent.Type, 
                    lastEvent.GroupID,lastEvent.ID, 
                    PropolisData.Instance.GetItemDataById(lastEvent.GroupID, lastEvent.ID, lastEvent.Type).Status);
            }
        }

        private void SendOscMessage(string address, int value, int value2, int value3)
        {

            OscMessage message = new OscMessage();

            message.address = address;
            message.values.Add(value);
            message.values.Add(value2);
            message.values.Add(value3);
            SoundMaxOsc.Send(message);
        }
    }

}

