using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis {
    [System.Serializable]
    public class PropolisDataType 
    {
        public PropolisDataType()
        {
            ID = 15;
            Error = true;
            Status = 0;
        }
        public int ID { get; set; }
        public bool Error { get; set; }
        public int Status { get; set; }
    }
}


