using System;

namespace Propolis
{ 
    public static class PropolisGameSettings
    {
        public const float DefaultGameTickTime = 1.5f; // Tick update de la corruption par défaut (difficulté à 1)     
        public const int MaxEdgeHexNeighborsCorruption = 2; // Le nombre de corrompus à la fois par les côté
        public const float DeltaTimeBetweenConsoleClean = 5; //don't touch
        public const int MaxOfTilesToCorruptExtend = 2;// Le nombre de corrompus à la fois lors de la corruption pas côtés
        public const float TimeBetweenAnimationSpawn = 0.1f;//don't touch
        public const float IntervalProcessUltraCorrupted = 0.3f; //don't touch
        public const int NumOfUltraCorruped = 1;//don't touch
        public const int MaxNumOfUltraCorruped = 2; // max d'ultra corrupted (rouge) quand la difficulté est à 1
        public const float HexSafeTimeAfterCleanse = 2.0f; // Durée de protection d'une tuile jaune quand elle provient d'une bombe bleu
        public const float CleansingStateDuration = 0.7f;
        public const float IntervalBetweenWaves = 40.0f; // Interval de seconde entre les vagues 60.0f
        public const float WaveSpeed = 0.00125f; //Rapidité de la vague valeur assez random 0.001f
        public const float AtomSaturationCorruptionTime = 1.6f;//don't touch
        public static float CurrentDifficultyMultiplier = 1.0f;//don't touch
        public const float DifficultyModifier = 0.1f; //Incrément de la diffulté
        public const float MinStableDifficultyThreshold = 0.25f; // Pourcentage plancher decrease difficulty
        public const float MaxStableDifficultyThreshold = 0.35f; // Pourcentage plancher increase difficulty
        public const float DifficultyUpdateDeltaTime = 0.3f; // Temp entre deux analyse de la difficultée
        public const float BatteryUpdateDeltaTime =  5.0f;// Temp entre deux analyse pour le réservoir
        public const float TargetIntervalBetweenClimaxes =21000.0f; // Durée souhaité  entre les climax
        public const float CriticalOnHexRatio = 0.05f; // Taux d'occupation du plancher minimum pour que le réservoir ne se vide pas
        public const float BatteryLevelLostWhenCritical = -0.10f; // Pourcentage du réservoir perdu si necessaire
        public const float RecipeBlinkingHintTime = 0.7f; // no longer apply
        public const float ShieldDeactivationDelay = 3.0f;
        public const float MaxDifficulty = 15.0f;
        public const float UltraCorruptedHintTime = 2f;
        public const float WaveInitialDistanceRatioFromGameSize = 1.5f; // Distance de la wave 1 étant collé sur les atomes (avertir charli si changement)
        public static PropolisStatus[] StatusFreeToBeCorrupted = new PropolisStatus[] { PropolisStatus.ON, PropolisStatus.OFF }; // Dont fucking touch this.
    }

}


