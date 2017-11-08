using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Propolis
{
    [System.Serializable]
    public class PropolisLastEventState
    {
        public PropolisLastEventState()
        {
            Type = "";
            Action = "";
            ID = -1;

        }
        public PropolisLastEventState(string type, string action, int id)
        {
            Type = type;
            Action = action;
            ID = id;

        }
        public string Type { get; set; }
        public string Action { get; set; }
        public int ID { get; set; }
        public int GroupID { get; set; }
    }
}


