﻿using System;

namespace Propolis
{ 
    public static class PropolisGameSettings
    {
        public const float DefaultGameTickTime = 1f;
        public const float ScorePressOnActiveHex = 0.001f;
        public const float ScorePressOnCorruptedHex = 0.002f;
        public const float ScoreOnCleanUltraCorruptedHex = 0.250f;
        public const float ScorePressOnCleannerHex = 0.500f;
        public const int MaxEdgeHexNeighborsCorruption = 3;
        public const int MaxOfTilesToCorruptExtend = 3;
        public const float MinPropraggationCorruptionDistance = 1f;

    }
}

