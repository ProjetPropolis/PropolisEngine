﻿using System;
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
                    SetPosition(new Vector3(
                        float.Parse(modelParams[1]),
                        float.Parse(modelParams[2]),
                        0));

                        
                    IP = modelParams[3];
                    InPort = Convert.ToInt32(modelParams[4]);
                    OutPort = Convert.ToInt32(modelParams[5]);
                    Childrens = new List<PropolisGroupItemData>();
                    for (int i = 0; i < 7; i++)
                    {
                        Childrens.Add(new PropolisGroupItemData(i));
                    }

                }
                catch
                {
                    return;
                }

                Error = false;

            }
        }
        //A vector 3 is not use because it cannot be serialized
        public Vector3 GetPosition()
        {
            return new Vector3(x, y, z);  
        }

        //A vector 3 is not use because it cannot be serialized
        public void SetPosition(Vector3 position)
        {
            x = position.x;
            y = position.x;
            z = position.x;
        }

        [SerializeField]
        public List<PropolisGroupItemData> Childrens{get;set;}
        public string IP { get; set; }
        public int OutPort { get; set; }
        public int InPort { get; set; }
        //A vector 3 is not use because it cannot be serialized
        public float x, y, z;

        public void OverrideData(float _x , float _y, string _IP, int _inPort, int _OutPort)
        {
            x = _x;
            y = _y;
            InPort = _inPort;
            OutPort = _OutPort;
        }
    }
}

