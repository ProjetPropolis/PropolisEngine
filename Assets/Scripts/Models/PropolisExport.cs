using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using UnityOSC;
using System;
using System.Net;
using System.Threading;


namespace Propolis
{
    public class PropolisExport : MonoBehaviour
    {

        public OSCClient BatteryOSC, SoundOSC, MolecularHUDOSC,PKOSC;
        public int BatteryPort, SoundPort, MolecularHUDPort, PKPort;
        public string BatteryAddress, SoundAddress, MolecularHUDAddress, PKAddress;
        public MolecularGameController molecularGameController;
        


        // Use this for initialization
        void Start()
        {
            PropolisData d = PropolisData.Instance;
            UpdateOscComponent(ref BatteryOSC, BatteryAddress, BatteryPort);
            UpdateOscComponent(ref SoundOSC, SoundAddress, SoundPort);
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
                Debug.Log(ex.Message);
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
                SendOscMessage(
                    "/" + lastEvent.Type,
                    lastEvent.GroupID, lastEvent.ID,
                    PropolisData.Instance.GetItemDataById(lastEvent.GroupID, lastEvent.ID, lastEvent.Type).Status, SoundOSC);
                if (lastStatus == PropolisStatus.ON && PropolisData.Instance.LastEvent.Type == PropolisDataTypes.HexGroup) {
                    SendPkMessage("/hexpress",1);
                    SendPkMessage("/hexpress", 0);
                }
                    



            }
            else if (PropolisData.Instance.LastEvent.Action == PropolisActions.SetBatteryLevel)
            {
                SendBatteryOscMessage("/battery",PropolisData.Instance.BatteryLevel);
            }


            ExportToHUDS();
            ExportToSound();
            ExportToPK();

             
        }



        private void ExportToPK() {
            PropolisData d = PropolisData.Instance;
            SendPkMessage("/reservoir", d.BatteryLevel);
            SendPkMessage("/WaveProgression", d.WaveProgress);
            SendPkMessage("/difficulty", PropolisGameSettings.CurrentDifficultyMultiplier);
            SendPkMessage("/WaveIsActive", d.WaveActivated ? 1 : 0);
        }

        private void ExportToSound()
        {
            PropolisData d = PropolisData.Instance;
            SendSoundMessage("/reservoir", d.BatteryLevel);
            SendSoundMessage("/WaveProgression", d.WaveProgress);
            SendSoundMessage("/difficulty", PropolisGameSettings.CurrentDifficultyMultiplier);
            SendSoundMessage("/WaveIsActive", d.WaveActivated ? 1 : 0);




            //int i = 0;
            //foreach (var molecule in d.AtomGroupList)
            //{
            //    SendSoundMessage(string.Format("/recipe_lvl2_{0}", molecule.ID), 0);
            //    foreach (var atom in molecule.Childrens)
            //    {
            //        SendSoundMessage(string.Format("/atomgroup{0}_{1}", molecule.ID, atom.ID), atom.Status);
            //    }

            //    i++;
            //}
        }

        private void ExportToHUDS()
        {
            PropolisData d = PropolisData.Instance;
            SendHUDMessage("/reservoir", d.BatteryLevel);
            SendHUDMessage("/WaveProgression", d.WaveProgress);
            SendHUDMessage("/WaveIsActive", d.WaveActivated ? 1 : 0 );




            int i = 0;
            foreach (var molecule in d.AtomGroupList)
            {
                foreach (var atom in molecule.Childrens)
                {
                    SendHUDMessage(string.Format("/atomgroup{0}_{1}",molecule.ID,atom.ID), atom.Status);
                }

                try
                {
                    SendHUDMessage(string.Format("/atomgroup_distanceWave{0}", molecule.ID), molecularGameController.DistancesFromWave[i]);
                }
                catch (Exception)
                {
                    return;
                }
               
                i++;
            }
            
        }
        public void ExportAllGroupPosition()
        {
            PropolisData.Instance.HexGroupList.ForEach(x => SendAbstractGroupPosition("/hexgroup_pos",x.ID,x.GetPosition().x,x.GetPosition().y));
            //PropolisData.Instance.AtomGroupList.ForEach(x => SendAbstractGroupPosition("/atomgroup_pos", x.ID, x.GetPosition().x, x.GetPosition().y));
            //PropolisData.Instance.RecipeGroupList.ForEach(x => SendAbstractGroupPosition("/recipegroup_pos", x.ID, x.GetPosition().x, x.GetPosition().y));
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

        }

        private void SendSoundMessage(string address, int value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<int>(value);
            SoundOSC.Send(message);

        }

        private void SendPkMessage(string address, float value)
        {
            OSCMessage message = new OSCMessage(address);
            message.Append<float>(value);
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

        }

        public void SendClimaxState(int state) {
            SendBatteryOscMessage("/startClimax", state);

        }
    }

}

