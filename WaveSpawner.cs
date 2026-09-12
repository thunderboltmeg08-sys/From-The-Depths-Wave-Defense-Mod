using System;
using System.Collections.Generic;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Ftd.Planets.Factions.Designs;
using UnityEngine;

namespace WaveDefense
{
    public class WaveSpawner
    {
        private readonly System.Random _rng = new System.Random();

        /// <summary>
        /// Calculates the match score of a design for a target difficulty using Gaussian distribution,
        /// matching FtD Adventure mode's AdventureModeDifficultyMean and AdventureModeDifficultySigma.
        /// </summary>
        public float CalculateDesignFitness(WorldSpecificationFactionDesign design, float targetDifficulty)
        {
            if (design == null || design.AdventureModeChance <= 0f)
            {
                return 0f;
            }

            float mean = design.AdventureModeDifficultyMean;
            float sigma = design.AdventureModeDifficultySigma > 0f ? design.AdventureModeDifficultySigma : 10f;

            float diff = targetDifficulty - mean;
            float exponent = -0.5f * (diff * diff) / (sigma * sigma);

            float gaussianScore = (float)Math.Exp(exponent);

            // Weight by the design's base adventure chance
            return gaussianScore * design.AdventureModeChance;
        }

        /// <summary>
        /// Selects candidate enemy designs matching the target difficulty.
        /// </summary>
        public List<WorldSpecificationFactionDesign> SelectWaveDesigns(
            IEnumerable<WorldSpecificationFactionDesign> availableDesigns,
            float targetDifficulty,
            int maxCount = 4)
        {
            var scored = new List<KeyValuePair<WorldSpecificationFactionDesign, float>>();

            foreach (var design in availableDesigns)
            {
                float fitness = CalculateDesignFitness(design, targetDifficulty);
                if (fitness > 0.01f)
                {
                    scored.Add(new KeyValuePair<WorldSpecificationFactionDesign, float>(design, fitness));
                }
            }

            // Sort by fitness descending
            scored.Sort((a, b) => b.Value.CompareTo(a.Value));

            var selected = new List<WorldSpecificationFactionDesign>();
            int count = Math.Min(maxCount, scored.Count);

            for (int i = 0; i < count; i++)
            {
                selected.Add(scored[i].Key);
            }

            return selected;
        }

        /// <summary>
        /// Generates a randomized perimeter spawn point around the defense position.
        /// </summary>
        public Vector3 CalculateSpawnPosition(DefensePosition center, float spawnDistance, float angleDegrees)
        {
            float radians = angleDegrees * (float)Math.PI / 180f;
            float x = (float)center.X + (spawnDistance * (float)Math.Sin(radians));
            float z = (float)center.Z + (spawnDistance * (float)Math.Cos(radians));
            float y = (float)center.Y;

            return new Vector3(x, y, z);
        }
    }
}
