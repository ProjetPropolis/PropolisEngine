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

        public OSCClient BatteryOSC, SoundOSC, MolecularHUDOSC;
        public int BatteryPort, SoundPort, MolecularHUDPort;
        public string BatteryAddress, SoundAddress, MolecularHUDAddress;


        // Use this for initialization
        void Start()
        {
            PropolisData d = PropolisData.Instance;
            UpdateOscComponent(ref BatteryOSC, BatteryAddress, BatteryPort);
            UpdateOscComponent(ref SoundOSC, SoundAddress, SoundPort);
            UpdateOscComponent(ref MolecularHUDOSC, MolecularHUDAddress, MolecularHUDPort);

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

                ExportToHUDS();

            }
        }

        private void ExportToHUDS()
        {
            PropolisData d = PropolisData.Instance;
            SendHUDMessage("/reservoir", d.BatteryLevel);
            SendHUDMessage("/WaveProgression", d.WaveProgress);
            SendHUDMessage("/WaveIsActive", d.WaveActivated ? 1 : 0 );


            foreach (var molecule in d.AtomGroupList)
            {
                SendHUDMessage(string.Format("/recipe_lvl2_{0}", molecule.ID), 0);
                SendHUDMessage(string.Format("/recipe_lvl3_{0}", molecule.ID), 0);
                foreach (var atom in molecule.Childrens)
                {
                    SendHUDMessage(string.Format("/atomgroup{0}_{1}",molecule.ID,atom.ID), atom.Status);
                }
                
            }
            
        }
        public void SendRecipeEventToHUDS(int groupId,int lvl)
        {
            SendHUDMessage(string.Format("/recipe_lvl{1}_{0}", groupId,lvl),1);
            SendHUDMessage(string.Format("/recipe_lvl{1}_{0}", groupId, lvl), 0);
        }

        private void SendOscMessage(string address, int value, int value2, int value3)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(value);
            message.Append<int>(value2);
            message.Append<int>(value3);
            SoundOSC.Send(message);

        }

        private void SendHUDMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            MolecularHUDOSC.Send(message);

        }


        private void SendBatteryOscMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            BatteryOSC.Send (message);

        }
    }

}

