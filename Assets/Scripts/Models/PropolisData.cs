using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



namespace Propolis
{
    [System.Serializable]
    public class PropolisData
    {
        [SerializeField]
        private static volatile PropolisData instance;
        private static object syncRoot = new Object();
        [SerializeField]
        public PropolisLastEventState LastEvent { get; set; }
        [SerializeField]
        public List<AbstractGroupData> HexGroupList{ get; set; }
        public List<AbstractGroupData> AtomGroupList { get; set; }
        public float BatteryLevel { get; set; }
        public bool IsGamePlaying { get; set; }


        private PropolisData() {

            AtomGroupList = new List<AbstractGroupData>();
            HexGroupList = new List<AbstractGroupData>();
            LastEvent = new PropolisLastEventState();
            IsGamePlaying = false;
            BatteryLevel = 0.0f;

        }

  

        public bool UpdateItemStatus(string type, int groupID, int id, int status, out string statusMessage)
        {
            statusMessage = null;

            if (GetGroupDataById(groupID,type) == null)
            {
                statusMessage = "No " + type + " with the id " + groupID + " can be found";
                return false;
            }

            switch (type)
            {
                case PropolisDataTypes.HexGroup:
                    HexGroupList = HexGroupList.Select(x => {
                        if (x.ID == groupID)
                        {
                            x.Childrens = x.Childrens.Select(c => {
                                if(c.ID == id)
                                    c.Status = status;
                                return c;
                            }).ToList<PropolisGroupItemData>();
                        }
                        return x;
                    }).ToList<AbstractGroupData>();
                    break;

                case PropolisDataTypes.AtomGroup:
                    AtomGroupList = AtomGroupList.Select(x => {
                        if (x.ID == groupID)
                        {
                            x.Childrens = x.Childrens.Select(c => {
                                if (c.ID == id)
                                    c.Status = status;
                                return c;
                            }).ToList<PropolisGroupItemData>();
                        }
                        return x;
                    }).ToList<AbstractGroupData>();
                    break;
            }
            return true;

        }

        public bool UpdateItemStatus(string type,int groupID,int status, out string statusMessage)
        {
            statusMessage = null;

            if (GetGroupDataById(groupID, type) == null)
            {
                statusMessage = "No " + type + " with the id " + groupID + " can be found";
                return false;
            }

            switch (type)
            {
                case PropolisDataTypes.HexGroup:
                    HexGroupList = HexGroupList.Select(x => {
                        if (x.ID == groupID)
                        {
                            x.Childrens = x.Childrens.Select(c => {
                                c.Status = status;
                                return c;
                            }).ToList<PropolisGroupItemData>();
                        }
                        return x;
                    }).ToList<AbstractGroupData>();
                    break;
                case PropolisDataTypes.AtomGroup:
                    AtomGroupList = AtomGroupList.Select(x => {
                        if (x.ID == groupID)
                        {
                            x.Childrens = x.Childrens.Select(c => {
                                c.Status = status;
                                return c;
                            }).ToList<PropolisGroupItemData>();
                        }
                        return x;
                    }).ToList<AbstractGroupData>();
                    break;
            }
            return true;
            
        }


        public bool DeleteDataGroup(string type, int id, out string statusMessage)
        {
            statusMessage = null;
            if (GetGroupDataById(id, type) == null)
            {
                statusMessage = "No " + type + " with the id " + id + " can be found";
                return false;
            }
            switch (type)
            {
                case PropolisDataTypes.HexGroup:
                    HexGroupList = HexGroupList.Where(x => x.ID != id).ToList<AbstractGroupData>(); break;
                case PropolisDataTypes.AtomGroup:
                    AtomGroupList = AtomGroupList.Where(x => x.ID != id).ToList<AbstractGroupData>(); break;
            }
            return true;
            
        }

        public void ResetGameData()
        {
            IsGamePlaying = false;
            BatteryLevel = 0.0f;
            if (HexGroupList == null)
                HexGroupList = new List<AbstractGroupData>();
            HexGroupList.ForEach(x => x.Childrens.ForEach(y => y.Status = (int)PropolisStatus.ON));
            if (AtomGroupList == null)
                AtomGroupList = new List<AbstractGroupData>();
            AtomGroupList.ForEach(x => x.Childrens.ForEach(y => y.Status = (int)PropolisStatus.OFF));
        }


        public bool AddGroup(AbstractGroupData abstractGroupData, string type, out string statusMessage)
        {
            if(GetGroupDataById(abstractGroupData.ID,type) == null)
            {
                switch (type)
                {
                    case PropolisDataTypes.HexGroup: HexGroupList.Add(abstractGroupData);break;
                    case PropolisDataTypes.AtomGroup: AtomGroupList.Add(abstractGroupData); break;
                }
                
                
                statusMessage = null;
                return true;
            }
            else
            {
                statusMessage = "An Hexgroup of the same id already exist";
                return false;
            }
        }

        public int [] GetAllIdOfType(string type)
        {
            int[] returnArray = null;

            switch (type)
            {
                case PropolisDataTypes.HexGroup: returnArray = HexGroupList.Select(x => x.ID).ToArray<int>(); break;
                case PropolisDataTypes.AtomGroup: returnArray = AtomGroupList.Select(x => x.ID).ToArray<int>(); break;
            }
            return returnArray; 
        }

        public bool UpdateGroup(AbstractGroupData abstractGroupData, string type, out string statusMessage)
        {
            if (GetGroupDataById(abstractGroupData.ID, type) != null)
            {
                switch (type)
                {
                    case PropolisDataTypes.HexGroup:
                        HexGroupList.ForEach(
                                x =>
                                {
                                    if (x.ID == abstractGroupData.ID)
                                    {
                                        x.OverrideData(abstractGroupData.x, abstractGroupData.y, abstractGroupData.IP, abstractGroupData.InPort, abstractGroupData.OutPort);
                                    }
                                }

                        );
                        break;
                    case PropolisDataTypes.AtomGroup:
                        AtomGroupList.ForEach(
                                x =>
                                {
                                    if (x.ID == abstractGroupData.ID)
                                    {
                                        x.OverrideData(abstractGroupData.x, abstractGroupData.y, abstractGroupData.IP, abstractGroupData.InPort, abstractGroupData.OutPort);
                                    }
                                }

                        );
                        break;
                }
                
                statusMessage = null;
                return true;
            }
            else
            {
                statusMessage = "No hexgroup exist with the id: "+ abstractGroupData.ID.ToString();
                return false;
            }
        }


        public AbstractGroupData GetGroupDataById(int id,string type)
        {
            AbstractGroupData returnValue = null;

            try
            {
                switch (type)
                {
                    case PropolisDataTypes.HexGroup: returnValue = HexGroupList.First(x => x.ID == id);break;
                    case PropolisDataTypes.AtomGroup: returnValue = AtomGroupList.First(x => x.ID == id); break;
                }
                
            }
            catch
            {
                return null; 
            }

            return returnValue;
        }

        public static PropolisData Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PropolisData();
                    }
                }

                return instance;
            }

            set
            {
                instance = value;   
            }
        }

        public string ExportToJSON()
        {
            return JsonUtility.ToJson(instance);
        } 

    }
}


