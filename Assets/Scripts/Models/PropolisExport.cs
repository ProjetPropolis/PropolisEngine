using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using UnityOSC;
using System;
using System.Net;
using System.Threading;
using System.Linq;


namespace Propolis
{
    public class PropolisExport : MonoBehaviour
    {

        public OSCClient BatteryOSC, SoundOSC, SoundOSC2, MolecularHUDOSC,PKOSC;
        public int BatteryPort, SoundPort, SoundPort2, MolecularHUDPort, PKPort;
        public string BatteryAddress, SoundAddress, SoundAddress2, MolecularHUDAddress, PKAddress;
        public MolecularGameController molecularGameController;
        


        // Use this for initialization
        void Awake()
        {
            PropolisData d = PropolisData.Instance;
            UpdateOscComponent(ref BatteryOSC, BatteryAddress, BatteryPort);
            UpdateOscComponent(ref SoundOSC, SoundAddress, SoundPort);
            UpdateOscComponent(ref SoundOSC2, SoundAddress2, SoundPort2);
            UpdateOscComponent(ref MolecularHUDOSC, MolecularHUDAddress, MolecularHUDPort);
            UpdateOscComponent(ref PKOSC, PKAddress, PKPort);

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
                //Debug.Log(ex.Message);
            }
            
        }


        // Update is called once per frame
        void Update()
        {

        }


        public void ExportFromModel()
        {
            if (PropolisData.Instance.LastEvent.Action== PropolisActions.UpdateItemStatus)
            {
                PropolisStatus lastStatus = (PropolisStatus)PropolisData.Instance.GetItemDataById(PropolisData.Instance.LastEvent.GroupID, PropolisData.Instance.LastEvent.ID, PropolisData.Instance.LastEvent.Type).Status;
                PropolisLastEventState lastEvent = Propolis.PropolisData.Instance.LastEvent;
                if (PropolisData.Instance.LastEvent.Type == PropolisDataTypes.HexGroup)
                {

                    SendOscMessage(
                        "/" + lastEvent.Type,
                        lastEvent.GroupID, lastEvent.ID,
                        PropolisData.Instance.GetItemDataById(lastEvent.GroupID, lastEvent.ID, lastEvent.Type).Status, SoundOSC);
                }

                if (PropolisData.Instance.LastEvent.Type == PropolisDataTypes.AtomGroup && PropolisData.Instance.IsGamePlaying)
                {
                    SendOscMessage(
                      "/" + lastEvent.Type,
                      lastEvent.GroupID, lastEvent.ID,
                      PropolisData.Instance.GetItemDataById(lastEvent.GroupID, lastEvent.ID, lastEvent.Type).Status, SoundOSC2);

                    if (PropolisData.Instance.LastEvent.ID == 9)
                    {
                        SendPkMessage("/ShieldsCount",
                            molecularGameController.ListOfItems
                            .Where(x => x.ID == 9 && x.status == PropolisStatus.SHIELD_ON)
                            .Count()
                        );

                    }
                }


                if(PropolisData.Instance.LastEvent.Type == PropolisDataTypes.AtomGroup)
                {
                    SendHUDMessage(string.Format("/atomgroup{0}_{1}", lastEvent.GroupID, lastEvent.ID), PropolisData.Instance.GetItemDataById(lastEvent.GroupID, lastEvent.ID, lastEvent.Type).Status);

                }


                if (lastStatus == PropolisStatus.ON && PropolisData.Instance.LastEvent.Type == PropolisDataTypes.HexGroup) {
                    SendPkMessage("/hexpress", 1);
                    SendPkMessage("/hexpress", 0);
                    SendBatteryOscMessage("/hexpress", 1);
                    SendBatteryOscMessage("/hexpress", 0);

                } 


            }
            else if (PropolisData.Instance.LastEvent.Action == PropolisActions.SetBatteryLevel)
            {
                SendBatteryOscMessage("/battery",PropolisData.Instance.BatteryLevel);
            }


            if (PropolisData.Instance.IsGamePlaying)
            {

                switch (PropolisData.Instance.LastEvent.Action)
                {
                    case PropolisActions.SetWaveActiveStatus: SendAllWaveStatus(); break;
                    case PropolisActions.SetWavePosition: SendAllWaveProgression(); break;
                    case PropolisActions.SetBatteryLevel: SendAllResevoirStatus(); break;
                }

            }


        }
        private void SendAllWaveStatus()
        {
            PropolisData d = PropolisData.Instance;
            SendPkMessage("/WaveIsActive", d.WaveActivated ? 1 : 0);
            SendSoundMessage("/WaveIsActive", d.WaveActivated ? 1 : 0);
            SendHUDMessage("/WaveIsActive", d.WaveActivated ? 1 : 0);
        }

        private void SendAllWaveProgression() {
            PropolisData d = PropolisData.Instance;
            SendPkMessage("/WaveProgression", d.WaveProgress);
            SendSoundMessage("/WaveProgression", d.WaveProgress);
            SendHUDMessage("/WaveProgression", d.WaveProgress);

        }

        private void SendAllResevoirStatus()
        {
            PropolisData d = PropolisData.Instance;
            SendPkMessage("/reservoir", d.BatteryLevel);
            SendSoundMessage("/reservoir", d.BatteryLevel);
            SendHUDMessage("/reservoir", d.BatteryLevel);

        }


      

       
        public void ExportAllGroupPosition()
        {
            PropolisData.Instance.HexGroupList.ForEach(x => SendAbstractGroupPosition("/hexgroup_pos",x.ID,x.GetPosition().x,x.GetPosition().y));
            //PropolisData.Instance.AtomGroupList.ForEach(x => SendAbstractGroupPosition("/atomgroup_pos", x.ID, x.GetPosition().x, x.GetPosition().y));
            //PropolisData.Instance.RecipeGroupList.ForEach(x => SendAbstractGroupPosition("/recipegroup_pos", x.ID, x.GetPosition().x, x.GetPosition().y));
        }

        public void SendAllCleanserPress()
        {
            SendBatteryOscMessage("/cleanser", 1);
            SendBatteryOscMessage("/cleanser", 0);
            SendPkMessage("/cleanser", 1);
            SendPkMessage("/cleanser", 0);

        }

        public void SendSuccessfulRecipeToHUD(AbstractGroup group, PropolisRecipe recipe) {
            SendHUDMessage(string.Format("/recipe{0}_{1}", group.ID, 0), recipe.GetItem(0));
            SendHUDMessage(string.Format("/recipe{0}_{1}", group.ID, 1), recipe.GetItem(1));
            SendHUDMessage(string.Format("/recipe{0}_{1}", group.ID, 2), recipe.GetItem(2));
        }
        public void SendRecipeEventToHUDSAndSound(int groupId,int lvl)
        {
            StartCoroutine(StartSendRecipeClimax(groupId,lvl));
        }

        IEnumerator StartSendRecipeClimax(int groupId, int lvl) {


                SendHUDMessage(string.Format("/recipe_lvl{1}_{0}", groupId, lvl), 1);
                SendSoundMessage(string.Format("/recipe_lvl{1}_{0}", groupId, lvl), 1);
                yield return new WaitForSecondsRealtime(.5f);
                SendHUDMessage(string.Format("/recipe_lvl{1}_{0}", groupId, lvl), 0);
                SendSoundMessage(string.Format("/recipe_lvl{1}_{0}", groupId, lvl), 0);


            
        }

        private void SendOscMessage(string address, int value, int value2, int value3, OSCClient client)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(value);
            message.Append<int>(value2);
            message.Append<int>(value3);
            client.Send(message);

        }

        private void SendAbstractGroupPosition(string address, int groupId, float x, float y)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(groupId);
            message.Append<float>(x);
            message.Append<float>(y);
            SoundOSC.Send(message);
        }

        private void SendHUDMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            MolecularHUDOSC.Send(message);

        }

        private void SendSoundMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            SoundOSC.Send(message);
            SoundOSC2.Send(message);

        }

        private void SendSoundMessage(string address, int value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(value);
            SoundOSC.Send(message);
            if(PropolisData.Instance.IsGamePlaying)
                SoundOSC2.Send(message);

        }

        private void SendPkMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            PKOSC.Send(message);

        }
        private void SendPkMessage(string address, int value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(value);
            PKOSC.Send(message);

        }
        private void SendBatteryOscMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
            BatteryOSC.Send (message);

        }

        public void SendClimaxStep(int step) {
            SendBatteryOscMessage("/climax",step);
            SendHUDMessage("/climax", step);
            SendSoundMessage("/climax", step);
            SendPkMessage("/climax", step);
        }

        public void SendClimaxState(int state) {
            SendBatteryOscMessage("/startClimax", state);
            SendHUDMessage("/startClimax", state);
            SendSoundMessage("/startClimax", state);
            SendPkMessage("/startClimax", state);

        }
    }

}

