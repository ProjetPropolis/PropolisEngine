using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Propolis
{
    public static class PropolisDataTypes
    {
        public const string Hex = "hex";
        public const string HexGroup = "hexgroup";
    }

    public static class PropolisActions
    {
        public const string Create = "create";
        public const string Update = "update";
    }
    public class PropolisManager:MonoBehaviour
    {
        public const char CommandSeparator = ' ';
        public const string LineFilterConsole = "\r\n";
        public PropolisData _propolisData;
        public string ConsoleLog;
        private PropolisLastEventState _TempLastBuffer;
        public GameController GameController;
        
        private void Awake()
        {
            ConsoleLog = "";
   
            _propolisData = PropolisData.Instance;

        }

        private void AppendToConsoleLog(string line)
        {
            ConsoleLog += line + LineFilterConsole;
        }

        public void ClearConsole()
        {
            ConsoleLog = "";
        }

        public string SendCommand(string rawCommand)
        {
            bool validCommand = false;
            string command;
            Queue<string> modelParams;
            AppendToConsoleLog(rawCommand);
            _TempLastBuffer = new PropolisLastEventState();
            if(ParseCommand(rawCommand.ToLower(),out command, out modelParams))
            {
                _TempLastBuffer.Action = command;
                switch (command)
                {
                    case "create":validCommand = Create(modelParams); break;
                    case "update": validCommand = false; break;
                    case "update-all": validCommand = false; break;
                    case "delete": validCommand = false; break;
                    case "save": validCommand = false; break;
                    case "load": validCommand = false; break;
                    case "status": validCommand = true; AppendToConsoleLog(_propolisData.ExportToJSON()); break;
                    default: AppendToConsoleLog("Error unknown command: " + rawCommand); break;
                }

            }
            else
            {
                AppendToConsoleLog("Error unknown command: " + rawCommand);
            }

            if (validCommand) {
                AppendToConsoleLog("Action Successful");
                UpdateAllModules();
            }                
            else
            {
                AppendToConsoleLog("An error has occured, Look in Console log for more details...");
            }


            return GetLastConsoleEntry();
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
                case PropolisDataTypes.Hex: validCreate = CreateType(type, modelParams); break;
                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }


            return validCreate;
        }

        private bool CreateType(string type, Queue<string> modelParams) 
        {
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
                case PropolisDataTypes.HexGroup: dataType = new HexGroupData(arrayParams); break;
                case PropolisDataTypes.Hex: dataType = new HexData(arrayParams); break;
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
                case PropolisDataTypes.HexGroup: _propolisData.HexGroupList.Add((HexGroupData)dataType); break;
                case PropolisDataTypes.Hex: validCreation= AppendChildrenTypeToParent(type, dataType, modelParams); break;
                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }

            if (validCreation)
                _propolisData.LastEvent = _TempLastBuffer;
            return validCreation;
        }


        private bool AppendChildrenTypeToParent(string type, PropolisDataType dataType,  Queue<string> modelParams)
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
            GameController.UpdateFromModel();
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


