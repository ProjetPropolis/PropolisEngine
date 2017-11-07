using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    [System.Serializable]
    public class HexGroupData: PropolisDataType,IPropolisGroupDataType
    {
        public HexGroupData(string[] modelParams) : base()
        {
            Error = true;
            if(modelParams.Length  == 7)
            {
                try
                {
                    ID = Convert.ToInt32(modelParams[0]);
                    Position = new Vector3(
                        Convert.ToInt32(modelParams[1]),
                        Convert.ToInt32(modelParams[2]),
                        Convert.ToInt32(modelParams[3])

                        );
                    IP = modelParams[4];
                    InPort = Convert.ToInt32(modelParams[5]);
                    OutPort = Convert.ToInt32(modelParams[6]);
                    Childrens = new List<PropolisDataType>();

                }
                catch
                {
                    return;
                }

                Error = false;

            }
        }
        public Vector3 Position { get; set; }
        public List<PropolisDataType> Childrens{get;set;}
        public string IP { get; set; }
        public int OutPort { get; set; }
        public int InPort { get; set; }
    }
}

