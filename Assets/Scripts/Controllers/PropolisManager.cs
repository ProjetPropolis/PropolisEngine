﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text.RegularExpressions;
using SFB;
namespace Propolis
{
    public class PropolisManager:MonoBehaviour
    {
        public const char CommandSeparator = ' ';
        public const string LineFilterConsole = "\r\n";
        [SerializeField]
        public PropolisData _propolisData;
        public string ConsoleLog;
        private PropolisLastEventState _TempLastBuffer;
        public GameController GameController;
        public BatteryUiController BatteryUi;
        public PropolisExport PropolisExport;
        public PropolisAlertUIController AlertController;
        
        private void Awake()
        {
            ConsoleLog = "";
            PropolisStatsExporter.LoadStatsFromFile();
            _propolisData = PropolisData.Instance;
            

        }

        private void Start()
        {
            SendCommand("load");
            StartCoroutine(CleanConsole());
            StartCoroutine(ProcessStatsSaveLoop());
        }

        private IEnumerator ProcessStatsSaveLoop() {

            while (true)
            {
                yield return new WaitForSeconds(10);
                PropolisStatsExporter.SaveStats();
            }
        }

        private void OnDestroy()
        {
            StopCoroutine(CleanConsole());
            StopCoroutine(ProcessStatsSaveLoop());
        }
        public IEnumerator CleanConsole()
        {
            while (true)
            {
                ConsoleLog = "";
                yield return new WaitForSeconds(PropolisGameSettings.DeltaTimeBetweenConsoleClean);
            }
        }

        private void AppendToConsoleLog(string line)
        {
            //ConsoleLog += line + LineFilterConsole;
            //TO DELETE
            //var linesString = Regex.Split(ConsoleLog, "\r\n|\r|\n");
            //var lines = linesString;
            //ConsoleLog = string.Join(Environment.NewLine, lines.ToArray());
        }

        public void ClearConsole()
        {
            ConsoleLog = "";
        }

        //it's static so we can call it from anywhere
        public bool Save()
        {
            _propolisData.ResetGameData();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
                FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
                bf.Serialize(file, _propolisData);
                file.Close();
                AlertController.Show("Propolis Event", "Engine Data Saved");
                return true;
            }
            catch(Exception ex)
            {
                AppendToConsoleLog(ex.Message);
                return false;
            }
 
        }

        public bool SaveAs()
        {
            try
            {
                string path = StandaloneFileBrowser.SaveFilePanel("Save Propolis map as", "", "propolismap", "pmap");
                BinaryFormatter bf = new BinaryFormatter();
                //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
                FileStream file = File.Create(path); //you can call it anything you want
                bf.Serialize(file, _propolisData);
                file.Close();
                AlertController.Show("Propolis Event", "Engine Data Saved");
                return true;
            }
            catch (Exception ex)
            {
                AppendToConsoleLog(ex.Message);
                return false;
            }

        }

        public bool Load()
        {
            if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                PropolisData.Instance = (PropolisData)bf.Deserialize(file);
                file.Close();
                _propolisData = PropolisData.Instance;
                _propolisData.ResetGameData();
                _propolisData.LastEvent = _TempLastBuffer;
                if (_propolisData.AtomGroupList == null)
                    _propolisData.AtomGroupList = new List<AbstractGroupData>();
                if (_propolisData.RecipeGroupList == null)
                    _propolisData.RecipeGroupList = new List<AbstractGroupData>();
                if (_propolisData.RecipeStack == null)
                {
                    _propolisData.RecipeStack = new Queue<PropolisRecipe>();
                }
                AlertController.Show("Propolis Event", "Engine Data Loaded");
                return true;
            }
            else
            {
                AppendToConsoleLog("Unable to load save file. Please save first");
                return false;
            }
        }

        public bool LoadAs()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Load Propolis map","","pmap",false);
            string path = paths[0];
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                PropolisData.Instance = (PropolisData)bf.Deserialize(file);
                file.Close();
                _propolisData = PropolisData.Instance;
                _propolisData.ResetGameData();
                _propolisData.LastEvent = _TempLastBuffer;
                if (_propolisData.AtomGroupList == null)
                    _propolisData.AtomGroupList = new List<AbstractGroupData>();
                if (_propolisData.RecipeGroupList == null)
                    _propolisData.RecipeGroupList = new List<AbstractGroupData>();
                if (_propolisData.RecipeStack == null)
                {
                    _propolisData.RecipeStack = new Queue<PropolisRecipe>();
                }
                AlertController.Show("Propolis Event", "Engine Data Loaded");
                return true;
            }
            else
            {
                AppendToConsoleLog("Unable to load save file. Please save first");
                return false;
            }
        }

        public void SendCommand(string rawCommand)
        {
            bool validCommand = false;
            string command;
            Queue<string> modelParams;
            AppendToConsoleLog(rawCommand);
            _TempLastBuffer = new PropolisLastEventState();
            _propolisData.LastEvent = new PropolisLastEventState();
            if(ParseCommand(rawCommand.ToLower(),out command, out modelParams))
            {
                _TempLastBuffer.Action = command;
                switch (command)
                {
                    case PropolisActions.Create:validCommand = Create(modelParams); break;
                    case PropolisActions.Save: validCommand = Save(); break;
                    case PropolisActions.SaveAs: validCommand = SaveAs(); break;
                    case PropolisActions.LoadAs: validCommand = LoadAs(); break;
                    case PropolisActions.Load: validCommand = Load(); break;
                    case PropolisActions.Update:validCommand = UpdateGroup(modelParams);break;
                    case PropolisActions.Delete: validCommand = Delete(modelParams); break;
                    case PropolisActions.UpdateItemStatus: validCommand = UpdateItemStatus(modelParams); break;
                    case PropolisActions.SetBatteryLevel: validCommand = SetBatteryLevel(modelParams); break;
                    case PropolisActions.SetWavePosition: validCommand = SetWavePosition(modelParams); break;
                    case PropolisActions.SetWaveActiveStatus: validCommand = SetWaveActiveStatus(modelParams); break;
                    case PropolisActions.Play: validCommand = PlayGame(); break;
                    case PropolisActions.Stop: validCommand = StopGame(); break;
                    case PropolisActions.PushRecipe: validCommand = PushRecipe(modelParams); break;
                    //case PropolisActions.AppStatus: validCommand = true; AppendToConsoleLog(_propolisData.ExportToJSON()); break;
                    default: AppendToConsoleLog("Error unknown command: " + rawCommand); break;
                }

            }
            else
            {
                AppendToConsoleLog("Error unknown command: " + rawCommand);
            }

            if (validCommand) {
                UpdateAllModules();
            }                
            else
            {
                AppendToConsoleLog("An error has occured, Look in Console log for more details...");
            }


        }


        private bool PlayGame()
        {
            _propolisData.LastEvent = _TempLastBuffer;
            _propolisData.IsGamePlaying = true;
            PropolisExport.ExportAllGroupPosition();
            return true;
        }

        private bool StopGame()
        {
            _propolisData.LastEvent = _TempLastBuffer;
            _propolisData.IsGamePlaying = false;
            return true;
        }

        private bool SetBatteryLevel(Queue<string> modelParams)
        {
            float batteryValue = 0 ;
            if (modelParams.Count != 1)
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for [BatteryLevel]");
                return false;
            }

            try
            {
                batteryValue = (float)Convert.ToDouble(modelParams.Dequeue());
                if (batteryValue < 0.0f && batteryValue > 1.0f)
                {
                    throw new FormatException();
                }               
            }
            catch
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for a float value between 0.0 and 1.0");
                return false;
            }
            _propolisData.BatteryLevel = batteryValue;
            _propolisData.LastEvent = _TempLastBuffer;

            return true;
        }

        private bool SetWavePosition(Queue<string> modelParams)
        {
            float wavePosition = .0f;
            if (modelParams.Count != 1)
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for [BatteryLevel]");
                return false;
            }

            try
            {
                wavePosition = (float)Convert.ToDouble(modelParams.Dequeue());
                if (wavePosition < 0.0f && wavePosition > 1.0f)
                {
                    throw new FormatException();
                }
            }
            catch
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for a float value between 0.0 and 1.0");
                return false;
            }
            _propolisData.WaveProgress = wavePosition;
            _propolisData.LastEvent = _TempLastBuffer;

            return true;
        }

        private bool SetWaveActiveStatus(Queue<string> modelParams)
        {
            bool waveActiveStatus = false;
            if (modelParams.Count != 1)
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for [BatteryLevel]");
                return false;
            }

            try
            {
                waveActiveStatus = Convert.ToBoolean(modelParams.Dequeue());

            }
            catch
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for a float value between 0.0 and 1.0");
                return false;
            }
            _propolisData.WaveActivated = waveActiveStatus;
            _propolisData.LastEvent = _TempLastBuffer;

            AlertController.Show("Propolis Event", waveActiveStatus ? "Wave Started" : "Wave Stopped");

            return true;
        }

        private bool PushRecipe(Queue<string> modelParams)
        {
            if (modelParams.Count != 3)
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for [BatteryLevel]");
                return false;
            }

            PropolisRecipe recipe = null;

            try
            {
                int rep1 = Convert.ToInt32(modelParams.Dequeue());
                int rep2 = Convert.ToInt32(modelParams.Dequeue());
                int rep3 = Convert.ToInt32(modelParams.Dequeue());
                recipe = new PropolisRecipe(rep1, rep2, rep3);
            }
            catch
            {
                AppendToConsoleLog("Error on sbl method, incorrect set of parameter, looking for a float value between 0.0 and 1.0");
                return false;
            }

            _propolisData.PushRecipe(recipe);
            _propolisData.LastEvent = _TempLastBuffer;
            if(_propolisData.RecipeStack.Count == 3)
                AlertController.Show("Propolis Event", "Recipe Created");
            return true;
        }

        public void RestartAllPythonScripts()
        {
            PropolisPIOperations.RestartPythonScript(AlertController,55);

            //foreach (var device in PropolisData.Instance.HexGroupList)
            //{
            //    PropolisPIOperations.RestartPythonScript(device.ID);
            //}

            //foreach (var device in PropolisData.Instance.AtomGroupList)
            //{
            //    PropolisPIOperations.RestartPythonScript(device.ID);
            //}
        }
        public void ShutdownAllDevice()
        {
            AlertController.Show("Propolis Device Communication", "Will close devices");
            PropolisPIOperations.Close(AlertController,55);
            //foreach (var device in PropolisData.Instance.HexGroupList)
            //{
            //    PropolisPIOperations.Close(device.ID);
            //}

            //foreach (var device in PropolisData.Instance.AtomGroupList)
            //{
            //    PropolisPIOperations.Close(device.ID);
            //}
        }

        private string GetLastConsoleEntry()
        {
            string[] separatorFilter = new string[] { LineFilterConsole };
            if (ConsoleLog == string.Empty)
                return "";

            string[] linesEntryArray = ConsoleLog.Split(separatorFilter, System.StringSplitOptions.RemoveEmptyEntries);

            return linesEntryArray[linesEntryArray.Length -1] ;
            
        }

        private bool Create(Queue<string> modelParams)
        {
            bool validCreate = false;
            if(modelParams.Count == 0)
            {
                AppendToConsoleLog("Error on create method, no create type and data passed to the manager");
                return false;
            }
            string type = modelParams.Dequeue();
            _TempLastBuffer.Type = type;
            switch (type)
            {
                case PropolisDataTypes.HexGroup: validCreate= CreateType(type, modelParams); break;
                case PropolisDataTypes.AtomGroup: validCreate = CreateType(type, modelParams); break;
                case PropolisDataTypes.RecipeGroup: validCreate = CreateType(type, modelParams); break;
                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }

            AlertController.Show("Propolis Event", string.Format("{0} Created", type));
            return validCreate;
        }

        private bool UpdateGroup(Queue<string> modelParams)
        {
            bool validUpdate = false;
            if (modelParams.Count == 0)
            {
                AppendToConsoleLog("Error on update method, no update type and data passed to the manager");
                return false;
            }
            string type = modelParams.Dequeue();
            _TempLastBuffer.Type = type;
            switch (type)
            {
                case PropolisDataTypes.HexGroup: validUpdate = UpdateType(type, modelParams); break;
                case PropolisDataTypes.AtomGroup: validUpdate = UpdateType(type, modelParams); break;
                case PropolisDataTypes.RecipeGroup: validUpdate = UpdateType(type, modelParams); break;
                default: AppendToConsoleLog("Error on update method, Invalid type : " + type); break;
            }


            return validUpdate;
        }

        private bool UpdateItemStatus(Queue<string> modelParams)
        {
            bool validUpdate = false;
            string statusMessage = null;
            

            if (modelParams.Count !=4   )
            {
                AppendToConsoleLog("Error on "+PropolisActions.UpdateItemStatus+" method, incorrect set of parameter, looking for [type] [ID]");
                return false;
            }

            string type = modelParams.Dequeue();
            _TempLastBuffer.Type = type;

            try
            {
                _TempLastBuffer.GroupID = int.Parse(modelParams.Dequeue());
            }
            catch
            {
                AppendToConsoleLog("Error on delete method, incorrect set of parameter, looking for [type] [GroupID] [ID] [Status]");
                return false;
            }

            string target = modelParams.Dequeue();
            int status; 

            try
            {
                status = int.Parse(modelParams.Dequeue());
            }
            catch
            {
                AppendToConsoleLog("Error on delete method, incorrect set of parameter, looking for [type] [GroupID] [ID] [Status]");
                return false;
            }

            if (target == "all")
            {
                _TempLastBuffer.ID = -1;
                switch (type)
                {
                    case PropolisDataTypes.HexGroup: validUpdate = _propolisData.UpdateItemStatus(PropolisDataTypes.HexGroup, _TempLastBuffer.GroupID, status, out statusMessage); break;
                    case PropolisDataTypes.AtomGroup: validUpdate = _propolisData.UpdateItemStatus(PropolisDataTypes.AtomGroup, _TempLastBuffer.GroupID, status, out statusMessage); break;
                    case PropolisDataTypes.RecipeGroup: validUpdate = _propolisData.UpdateItemStatus(PropolisDataTypes.RecipeGroup, _TempLastBuffer.GroupID, status, out statusMessage); break;
                    default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
                }
            }
            else
            {
                try
                {
                    _TempLastBuffer.ID = int.Parse(target);
                }
                catch
                {
                    AppendToConsoleLog("Error on delete method, incorrect set of parameter, looking for [type] [GroupID] [ID] [Status]");
                    return false;
                }
                switch (type)
                {
                    case PropolisDataTypes.HexGroup: validUpdate = _propolisData.UpdateItemStatus(PropolisDataTypes.HexGroup, _TempLastBuffer.GroupID, _TempLastBuffer.ID, status, out statusMessage); break;
                    case PropolisDataTypes.AtomGroup: validUpdate = _propolisData.UpdateItemStatus(PropolisDataTypes.AtomGroup, _TempLastBuffer.GroupID, _TempLastBuffer.ID, status, out statusMessage); break;
                    case PropolisDataTypes.RecipeGroup: validUpdate = _propolisData.UpdateItemStatus(PropolisDataTypes.RecipeGroup, _TempLastBuffer.GroupID, _TempLastBuffer.ID, status, out statusMessage); break;


                    default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
                }
            }

            if (validUpdate)
            {
                _propolisData.LastEvent = _TempLastBuffer;
            }
            else
            {
                if (statusMessage != null)
                {
                    AppendToConsoleLog(statusMessage);
                }
            }

            return validUpdate;
        }



        private bool Delete(Queue<string> modelParams)
        {
            bool validDelete = false;
            string statusMessage = null;
            if (modelParams.Count != 2)
            {
                AppendToConsoleLog("Error on delete method, incorrect set of parameter, looking for [type] [ID]");
                return false;
            }
            string type = modelParams.Dequeue();
            _TempLastBuffer.Type = type;
            try
            {
                _TempLastBuffer.ID = int.Parse(modelParams.Dequeue());
            }
            catch
            {
                AppendToConsoleLog("Error on delete method, incorrect set of parameter, looking for [type] [ID]");
                return false;               
            }
          
            switch (type)
            {
                case PropolisDataTypes.HexGroup: validDelete = _propolisData.DeleteDataGroup(PropolisDataTypes.HexGroup, _TempLastBuffer.ID, out statusMessage); break;
                case PropolisDataTypes.AtomGroup: validDelete = _propolisData.DeleteDataGroup(PropolisDataTypes.AtomGroup, _TempLastBuffer.ID, out statusMessage); break;
                case PropolisDataTypes.RecipeGroup: validDelete = _propolisData.DeleteDataGroup(PropolisDataTypes.RecipeGroup, _TempLastBuffer.ID, out statusMessage); break;
                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }

            if (validDelete)
            {
                _propolisData.LastEvent = _TempLastBuffer;
            }
            else
            {
                if (statusMessage != null)
                {
                    AppendToConsoleLog(statusMessage);
                }
            }

            return validDelete;
        }


        private bool UpdateItem<ItemType>(Queue<string> modelParams)
        {
            bool validCreate = false;
            if (modelParams.Count != 3)
            {
                AppendToConsoleLog("Error on create method, no create type and data passed to the manager");
                return false;
            }
            string type = modelParams.Dequeue();
            _TempLastBuffer.Type = type;
            try
            {

            }
            catch
            {
                AppendToConsoleLog("Invalid parameters for update item looking for [Type] [GroupID] [ID] [Status]");
                return false;
            }
            switch (type)
            {
                case PropolisDataTypes.Hex: validCreate = CreateType(type, modelParams); break;
                default: AppendToConsoleLog("Error on create method, Invalid type : " + type + "not an item look for atom or hex"); break;
            }


            return validCreate;
        }

        private bool CreateType(string type, Queue<string> modelParams) 
        {
            string statusMessage = null;
            PropolisDataType dataType = null;
            string[] arrayParams = modelParams.ToArray<string>();
            bool validCreation = true;

            if (modelParams.Count == 0)
            {
                AppendToConsoleLog("Error on create method, no create data");
                return false;
            }

            switch (type)
            {
                case PropolisDataTypes.HexGroup: dataType = new AbstractGroupData(arrayParams); ((AbstractGroupData)dataType).CreateChildrensForHexGroup(); break;
                case PropolisDataTypes.AtomGroup: dataType = new AbstractGroupData(arrayParams); ((AbstractGroupData)dataType).CreateChildrensForAtomGroup (); break;
                case PropolisDataTypes.RecipeGroup: dataType = new AbstractGroupData(arrayParams); ((AbstractGroupData)dataType).CreateChildrensForRecipeGroup(); break;

                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }

            if(dataType  == null)
            {
                return false;
            }

            _TempLastBuffer.ID = dataType.ID;
          
            if (dataType.Error == true)
            {
                AppendToConsoleLog("Error on create "+type+", invalid parameters");
                return false;
            }

            switch (type)
            {
                case PropolisDataTypes.HexGroup: validCreation = _propolisData.AddGroup((AbstractGroupData)dataType,PropolisDataTypes.HexGroup ,out statusMessage); break;
                case PropolisDataTypes.AtomGroup: validCreation = _propolisData.AddGroup((AbstractGroupData)dataType, PropolisDataTypes.AtomGroup, out statusMessage); break;
                case PropolisDataTypes.RecipeGroup: validCreation = _propolisData.AddGroup((AbstractGroupData)dataType, PropolisDataTypes.RecipeGroup, out statusMessage); break;

                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }

            if (validCreation)
            {
                _propolisData.LastEvent = _TempLastBuffer;
            }
            else
            {
                if (statusMessage != null)
                {
                    AppendToConsoleLog(statusMessage);
                }
            }
               
            return validCreation;
        }

        private bool UpdateType(string type, Queue<string> modelParams)
        {
            string statusMessage = null;
            PropolisDataType dataType = null;
            string[] arrayParams = modelParams.ToArray<string>();
            bool validUpdate= true;

            if (modelParams.Count == 0)
            {
                AppendToConsoleLog("Error on update method, no create data");
                return false;
            }

            switch (type)
            {
                case PropolisDataTypes.HexGroup: dataType = new AbstractGroupData(arrayParams); break;
                case PropolisDataTypes.AtomGroup: dataType = new AbstractGroupData(arrayParams); break;
                case PropolisDataTypes.RecipeGroup: dataType = new AbstractGroupData(arrayParams); break;
                default: AppendToConsoleLog("Error on update method, Invalid type : " + type); break;
            }

            if (dataType == null)
            {
                return false;
            }

            _TempLastBuffer.ID = dataType.ID;

            if (dataType.Error == true)
            {
                AppendToConsoleLog("Error on update " + type + ", invalid parameters"); 
                return false;
            }

            switch (type)
            {
                case PropolisDataTypes.HexGroup: validUpdate = _propolisData.UpdateGroup((AbstractGroupData)dataType,PropolisDataTypes.HexGroup, out statusMessage); break;
                case PropolisDataTypes.AtomGroup: validUpdate = _propolisData.UpdateGroup((AbstractGroupData)dataType, PropolisDataTypes.AtomGroup, out statusMessage); break;
                case PropolisDataTypes.RecipeGroup: validUpdate = _propolisData.UpdateGroup((AbstractGroupData)dataType, PropolisDataTypes.RecipeGroup, out statusMessage); break;

                default: AppendToConsoleLog("Error on update method, Invalid type : " + type); break;
            }

            if (validUpdate)
            {
                _propolisData.LastEvent = _TempLastBuffer;
            }
            else
            {
                if (statusMessage != null)
                {
                    AppendToConsoleLog(statusMessage);
                }
            }

            return validUpdate;
        }


        private bool AppendChildrenTypeToParent(string type, PropolisGroupItemData dataType,  Queue<string> modelParams)
        {
            bool validCreation = false;
            int parentID;

            if (modelParams.Count == 0)
            {
                AppendToConsoleLog("Error on create method, no create data passed to the manager");
                return false;
            }

            try
            {
                if (!int.TryParse(modelParams.Dequeue(), out parentID))
                {
                    return false;
                }
                try
                {
                    switch (type)
                    {
                        case PropolisDataTypes.Hex: _propolisData.HexGroupList.First(x => x.ID == parentID).Childrens.Add(dataType); break;
                       // case PropolisDataTypes.Hex: _propolisData.AtomGroupList.First(x => x.ID == parentID).Childrens.Add(dataType); break;
                    }
                }
                catch
                {
                    AppendToConsoleLog("Error on create method, trying to create" +type+" for inexistant parent");
                    return false;
                }

                
              
            }
            catch
            {
                return false;
            }     

            return validCreation;
        }

        private void UpdateAllModules()
        {
            PropolisExport.ExportFromModel();
            GameController.UpdateFromModel();
            if (PropolisData.Instance.LastEvent.Action == PropolisActions.SetBatteryLevel)
            {
                BatteryUi.BatteryValueUpdate(PropolisData.Instance.BatteryLevel);
            }
        }

        private bool ParseCommand(string rawCommand, out string command, out Queue<string> parsedCommand)
        {
            bool validParsing = false;
            command = null;
            parsedCommand = null;
            try
            {
               Queue<string> bufferCommand = new Queue <string>(rawCommand.Split(PropolisManager.CommandSeparator));
               command = bufferCommand.Dequeue();
               parsedCommand = bufferCommand;
               validParsing = true;                
            }

            catch
            {
                return false;
            }

            return validParsing;

        }

    }

}


