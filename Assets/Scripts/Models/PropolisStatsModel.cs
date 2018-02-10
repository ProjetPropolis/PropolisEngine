using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Propolis
{
    [Serializable]
    public class PropolisStatsModel
    {
        public int AlveolesPressed { get { return alveolesPressed; } set { alveolesPressed = value; } }
        public int ReservoirFilled  { get { return reservoirFilled; } set { reservoirFilled = value; } }
        public int AtomRecipeDone { get { return atomRecipeDone; } set { atomRecipeDone = value; } }
        public int CleanserPressed { get { return cleanserPressed; } set { cleanserPressed = value; } }
        public int UltraCorruptedDestroyed { get { return ultraCorruptedDestroyed; } set { ultraCorruptedDestroyed = value; } }

        public int alveolesPressed = 0;
        public int reservoirFilled = 0;
        public int atomRecipeDone = 0;
        public int cleanserPressed = 0;
        public int ultraCorruptedDestroyed = 0;
    }

}
