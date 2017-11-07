using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


