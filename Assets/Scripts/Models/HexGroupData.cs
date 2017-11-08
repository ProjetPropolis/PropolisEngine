using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    [System.Serializable]
    public class HexGroupData: PropolisDataType
    {
        public HexGroupData(string[] modelParams) : base()
        {
            Error = true;
            if(modelParams.Length  == 6)
            {
                try
                {
                    ID = Convert.ToInt32(modelParams[0]);
                    Position = new Vector3(
                        float.Parse(modelParams[1]),
                        float.Parse(modelParams[2]),
                        0);

                        
                    IP = modelParams[3];
                    InPort = Convert.ToInt32(modelParams[4]);
                    OutPort = Convert.ToInt32(modelParams[5]);
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

