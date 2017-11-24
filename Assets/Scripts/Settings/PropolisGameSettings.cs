using System;

namespace Propolis
{ 
    public static class PropolisGameSettings
    {
        public const float DefaultGameTickTime = 2.5f;
        public const float ScorePressOnActiveHex = 0.001f;
        public const float ScorePressOnCorruptedHex = 0.002f;
        public const float ScoreOnCleanUltraCorruptedHex = 0.250f;
        public const float ScorePressOnCleannerHex = 0.500f;
        public const int MaxEdgeHexNeighborsCorruption = 3;
        public const float DeltaTimeBetweenConsoleClean = 5;
        public const int MaxOfTilesToCorruptExtend = 2;
        public const float TimeBetweenAnimationSpawn = 0.1f;
        public const float IntervalProcessUltraCorrupted = 0.3f;
        public static PropolisStatus[] StatusFreeToBeCorrupted = new PropolisStatus[] { PropolisStatus.ON, PropolisStatus.OFF };
    }

}


