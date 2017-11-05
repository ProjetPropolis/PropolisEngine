using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    public sealed class PropolisData
    {
        private static volatile PropolisData instance;
        private static object syncRoot = new Object();

        public List<HexGroupData> HexGroupList{ get; set; }
 

        private PropolisData() {

            HexGroupList = new List<HexGroupData>();

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

    }
}


