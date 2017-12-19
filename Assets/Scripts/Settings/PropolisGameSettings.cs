using System;

namespace Propolis
{ 
    public static class PropolisGameSettings
    {
        public const float DefaultGameTickTime = 3.0f;
        public const float ScorePressOnActiveHex = 0.005f;
        public const float ScorePressOnCorruptedHex = 0.010f;
        public const float ScoreOnCleanUltraCorruptedHex = 0.100f;
        public const float ScorePressOnCleannerHex = 0.250f;
        public const float ScorePressOnRecipeLvl2 = 0.100f;
        public const float ScorePressOnRecipeLvl3 = 0.400f;
        public const int MaxEdgeHexNeighborsCorruption = 2;
        public const float DeltaTimeBetweenConsoleClean = 5;
        public const int MaxOfTilesToCorruptExtend = 2;
        public const float TimeBetweenAnimationSpawn = 0.1f;
        public const float IntervalProcessUltraCorrupted = 0.3f;
        public const int NumOfUltraCorruped = 1;
        public const int MaxNumOfUltraCorruped = 1;
        public const float CleansingStateDuration = 0.7f;
        public const float IntervalBetweenWaves = 20.0f;
        public const float WaveSpeed = 0.005f;
        public const float AtomSaturationCorruptionTime = 0.8f;
        public static PropolisStatus[] StatusFreeToBeCorrupted = new PropolisStatus[] { PropolisStatus.ON, PropolisStatus.OFF };
    }

}


