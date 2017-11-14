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
        public List<HexGroupData> HexGroupList{ get; set; }
 

        private PropolisData() {

            HexGroupList = new List<HexGroupData>();
            LastEvent = new PropolisLastEventState();

        }

  

        public bool UpdateItemStatus(string type, int groupID, int id, int status, out string statusMessage)
        {
            statusMessage = null;

            if (GetHexGroupDataById(groupID) == null)
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
                    }).ToList<HexGroupData>();
                    break;
            }
            return true;

        }

        public bool UpdateItemStatus(string type,int groupID,int status, out string statusMessage)
        {
            statusMessage = null;

            if (GetHexGroupDataById(groupID) == null)
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
                    }).ToList<HexGroupData>();
                    break;
            }
            return true;
            
        }


        public bool DeleteDataGroup(string type, int id, out string statusMessage)
        {
            statusMessage = null;
            if (GetHexGroupDataById(id) == null)
            {
                statusMessage = "No " + type + " with the id " + id + " can be found";
                return false;
            }
            switch (type)
            {
                case PropolisDataTypes.HexGroup:
                    HexGroupList = HexGroupList.Where(x => x.ID != id).ToList<HexGroupData>(); break;
            }
            return true;
            
        }


        public bool AddHexGroup(HexGroupData hexGroupData, out string statusMessage)
        {
            if(GetHexGroupDataById(hexGroupData.ID) == null)
            {
                HexGroupList.Add(hexGroupData);
                
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
            }
            return returnArray; 
        }

        public bool UpdateHexGroup(HexGroupData hexGroupData, out string statusMessage)
        {
            if (GetHexGroupDataById(hexGroupData.ID) != null)
            {
                HexGroupList.ForEach(
                    x =>
                    {
                        if(x.ID == hexGroupData.ID)
                        {
                            x.OverrideData(hexGroupData.x, hexGroupData.y, hexGroupData.IP, hexGroupData.InPort, hexGroupData.OutPort); 
                        }
                    }

                );

                statusMessage = null;
                return true;
            }
            else
            {
                statusMessage = "No hexgroup exist with the id: "+ hexGroupData.ID.ToString();
                return false;
            }
        }


        public HexGroupData GetHexGroupDataById(int id)
        {
            HexGroupData returnValue = null;

            try
            {
                returnValue = HexGroupList.First(x => x.ID == id);
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


