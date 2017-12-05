using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using UnityOSC;
using System;
using System.Net;


namespace Propolis
{
    public class PropolisExport : MonoBehaviour
    {

        public OSCClient BatteryOSC, SoundOSC;
        public int BatteryPort, SoundPort;
        public string BatteryAddress, SoundAddress;


        // Use this for initialization
        void Start()
        {
            UpdateOscComponent(ref BatteryOSC, BatteryAddress, BatteryPort);
            UpdateOscComponent(ref SoundOSC, SoundAddress, SoundPort);
        }

        private void UpdateOscComponent(ref OSCClient client, string ip, int port)
        {
            try
            {
                IPAddress address;
                IPAddress.TryParse(ip, out address);
                client = new OSCClient(address, port);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            
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
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(value);
            message.Append<int>(value2);
            message.Append<int>(value3);
            SoundOSC.Send(message);

        }

        private void SendBatteryOscMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            BatteryOSC.Send (message);

        }
    }

}

