using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    public class HexGroupData: IPropolisDataType,IPropolisGroupDataType
    {
        public HexGroupData(string[] modelParams)
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
                    Childrens = new List<IPropolisDataType>();

                }
                catch
                {
                    return;
                }

                Error = false;

            }
        }
        public int ID { get; set; }
        public Vector3 Position { get; set; }
        public List<IPropolisDataType> Childrens{get;set;}
        public string IP { get; set; }
        public int OutPort { get; set; }
        public int InPort { get; set; }
        public new bool Error { get; set; }
    }
}

