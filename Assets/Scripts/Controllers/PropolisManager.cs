using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Propolis
{


   
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
                    case PropolisActions.Create:validCommand = Create(modelParams); break;
                    case PropolisActions.Delete: validCommand = Delete(modelParams); break;

                    case PropolisActions.AppStatus: validCommand = true; AppendToConsoleLog(_propolisData.ExportToJSON()); break;
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
                default: AppendToConsoleLog("Error on create method, Invalid type : " + type); break;
            }


            return validCreate;
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
                case PropolisDataTypes.HexGroup: dataType = new HexGroupData(arrayParams); break;
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
                case PropolisDataTypes.HexGroup: validCreation = _propolisData.AddHexGroup((HexGroupData)dataType,out statusMessage); break;
                case PropolisDataTypes.Hex: validCreation= AppendChildrenTypeToParent(type, dataType, modelParams); break;
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


