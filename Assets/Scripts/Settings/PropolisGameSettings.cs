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
    }
}

