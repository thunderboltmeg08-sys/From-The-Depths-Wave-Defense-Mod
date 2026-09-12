using System;

namespace WaveDefense
{
    public class WaveConfig
    {
        /// <summary>
        /// Starting difficulty for wave 1.
        /// </summary>
        public float InitialDifficulty { get; set; } = 10f;

        /// <summary>
        /// Difficulty increment per wave up to 100.
        /// </summary>
        public float DifficultyStep { get; set; } = 10f;

        /// <summary>
        /// Threshold at which difficulty shifts from stepping to linear scaling.
        /// </summary>
        public float ProportionalCap { get; set; } = 100f;

        /// <summary>
        /// Linear rate of difficulty increase per wave above 100.
        /// </summary>
        public float LinearStepAfterCap { get; set; } = 10f;

        /// <summary>
        /// Time in seconds the player must survive to complete a wave.
        /// </summary>
        public float WaveDurationSeconds { get; set; } = 180f;

        /// <summary>
        /// Preparation time in seconds between waves.
        /// </summary>
        public float IntermissionSeconds { get; set; } = 45f;

        /// <summary>
        /// Distance from the defense position at which enemy forces spawn.
        /// </summary>
        public float SpawnDistance { get; set; } = 1500f;

        /// <summary>
        /// Material reward granted to the player upon surviving a wave.
        /// </summary>
        public float BaseRewardMaterials { get; set; } = 2500f;

        /// <summary>
        /// Material reward scaling per difficulty level.
        /// </summary>
        public float RewardMaterialsPerDifficulty { get; set; } = 50f;

        /// <summary>
        /// Calculates the target adventure difficulty score for a given wave number (1-indexed).
        /// </summary>
        public float CalculateDifficulty(int waveNumber)
        {
            if (waveNumber <= 1)
            {
                return InitialDifficulty;
            }

            float difficulty = InitialDifficulty + (waveNumber - 1) * DifficultyStep;

            if (difficulty <= ProportionalCap)
            {
                return difficulty;
            }

            // Above 100, scale linearly based on the wave index beyond the cap
            int wavesToCap = (int)Math.Ceiling((ProportionalCap - InitialDifficulty) / DifficultyStep);
            int extraWaves = waveNumber - 1 - wavesToCap;

            return ProportionalCap + (extraWaves * LinearStepAfterCap);
        }

        /// <summary>
        /// Calculates material reward for surviving the given wave.
        /// </summary>
        public float CalculateWaveReward(int waveNumber)
        {
            float difficulty = CalculateDifficulty(waveNumber);
            return BaseRewardMaterials + (difficulty * RewardMaterialsPerDifficulty);
        }
    }
}
