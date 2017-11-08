using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Propolis
{
    [System.Serializable]
    public class PropolisData
    {
        private static volatile PropolisData instance;
        private static object syncRoot = new Object();
        public PropolisLastEventState LastEvent { get; set; }

        public List<HexGroupData> HexGroupList{ get; set; }
 

        private PropolisData() {

            HexGroupList = new List<HexGroupData>();
            LastEvent = new PropolisLastEventState();

        }


        public bool DeleteDataGroup(string type, int id, out string statusMessage)
        {
            statusMessage = null;
            if (GetHexGroupDataById(id) == null)
            {
                statusMessage = "No " + type + " with the id " + id + "can be found";
                return false;
            }
            else
            {
                switch (type)
                {
                    case PropolisDataTypes.HexGroup:
                        HexGroupList = HexGroupList.Where(x => x.ID != id).ToList<HexGroupData>(); break;
                }
                return true;
            }
        }


     

        public bool AddHexGroup(HexGroupData hexGroupData, out string statusMessage)
        {
            if(GetHexGroupDataById(hexGroupData.ID) == null)
            {
                HexGroupList.Add(hexGroupData);
                for(int i= 0; i < 6; i++)
                {
                    hexGroupData.Childrens.Add(new PropolisGroupItemData(i));
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
        }

        public string ExportToJSON()
        {
            return JsonUtility.ToJson(instance);
        } 

    }
}


