using System;
using System.Collections.Generic;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Core.Types;
using BrilliantSkies.Ftd.Modes.Campaign.AiSpawner;
using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Factions.Designs;
using UnityEngine;

namespace WaveDefense
{
    public class WaveSpawner
    {
        private readonly System.Random _rng = new System.Random();

        public int LastAvailableDesignCount { get; private set; }
        public int LastSelectedDesignCount { get; private set; }
        public string LastSpawnStatus { get; private set; } = "Not started";
        public WaveSpawner()
        {
        }

        /// <summary>
        /// Calculates the match score of a design for a target difficulty using Gaussian distribution,
        /// matching FtD Adventure mode's AdventureModeDifficultyMean and AdventureModeDifficultySigma.
        /// </summary>
        public float CalculateDesignFitness(WorldSpecificationFactionDesign design, float targetDifficulty)
        {
            if (design == null)
            {
                return 0f;
            }

            float mean = design.AdventureModeDifficultyMean > 0f ? design.AdventureModeDifficultyMean : 20f;
            float sigma = design.AdventureModeDifficultySigma > 0f ? design.AdventureModeDifficultySigma : 15f;

            float diff = targetDifficulty - mean;
            float exponent = -0.5f * (diff * diff) / (sigma * sigma);

            return (float)Math.Exp(exponent);
        }

        /// <summary>
        /// Gathers all available hostile faction designs from the active world planet.
        /// </summary>
        public List<WorldSpecificationFactionDesign> GetAvailableEnemyDesigns()
        {
            var designs = new List<WorldSpecificationFactionDesign>();

            try
            {
                var planet = Planet.i;
                if (planet != null && planet.Factions != null)
                {
                    foreach (var faction in planet.Factions.Factions)
                    {
                        // Exclude player faction (ID 783346167)
                        if (faction.Id.Id == 783346167 || faction.Name.Equals("Player", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (faction.Designs != null)
                        {
                            foreach (var design in faction.Designs.Designs)
                            {
                                if (design != null)
                                {
                                    designs.Add(design);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("WaveSpawner: Error gathering enemy designs: " + ex);
            }

            LastAvailableDesignCount = designs.Count;
            return designs;
        }

        /// <summary>
        /// Selects candidate enemy designs matching the target difficulty.
        /// </summary>
        public List<WorldSpecificationFactionDesign> SelectWaveDesigns(
            IEnumerable<WorldSpecificationFactionDesign> availableDesigns,
            float targetDifficulty,
            int maxCount = 3)
        {
            var scored = new List<KeyValuePair<WorldSpecificationFactionDesign, float>>();

            foreach (var design in availableDesigns)
            {
                float fitness = CalculateDesignFitness(design, targetDifficulty);
                if (fitness > 0.001f)
                {
                    scored.Add(new KeyValuePair<WorldSpecificationFactionDesign, float>(design, fitness));
                }
            }

            scored.Sort((a, b) => b.Value.CompareTo(a.Value));

            if (scored.Count == 0)
            {
                foreach (var design in availableDesigns)
                {
                    scored.Add(new KeyValuePair<WorldSpecificationFactionDesign, float>(design, 1f));
                }
            }

            var selected = new List<WorldSpecificationFactionDesign>();
            int count = Math.Min(maxCount, scored.Count);

            for (int i = 0; i < count; i++)
            {
                selected.Add(scored[i].Key);
            }

            LastSelectedDesignCount = selected.Count;
            return selected;
        }

        /// <summary>
        /// Spawns an enemy design into the physical world at a specified perimeter location.
        /// </summary>
        public bool SpawnEnemyUnit(WorldSpecificationFactionDesign design, Vector3 worldPosition)
        {
            try
            {
                ObjectId enemyTeam = design.Id.FactionId;
                Vector3d univPos = new Vector3d(worldPosition.x, worldPosition.y, worldPosition.z);
                Vector3 toCenter = new Vector3(68.73599f, 0f, 193.3501f) - worldPosition;
                if (toCenter.sqrMagnitude < 0.01f) toCenter = Vector3.forward;
                Quaternion rot = Quaternion.LookRotation(toCenter.normalized, Vector3.up);

                var fleetId = new ObjectId(true);
                var unitSpawner = new AiUnitSpawner(enemyTeam, SpawnStateFlags.IsInPlay, univPos, rot);
                var spawnedFleet = unitSpawner.SpawnExactThing(design.Id, fleetId);
                if (spawnedFleet != null)
                {
                    LastSpawnStatus = "Spawned " + design.Name;
                    AdvLogger.LogInfo(string.Format("WaveSpawner: Spawned in-play enemy '{0}' at {1}", design.Name, worldPosition));
                    return true;
                }

                LastSpawnStatus = "FtD returned no fleet for " + design.Name;
            }
            catch (Exception ex)
            {
                LastSpawnStatus = "Spawn failed: " + ex.GetBaseException().Message;
                Debug.LogError("WaveSpawner: Failed to spawn unit: " + ex);
            }

            return false;
        }

        /// <summary>
        /// Generates a randomized perimeter spawn point around the defense position.
        /// </summary>
        public Vector3 CalculateSpawnPosition(DefensePosition center, float spawnDistance, float angleDegrees)
        {
            float radians = angleDegrees * (float)Math.PI / 180f;
            float x = (float)center.X + (spawnDistance * (float)Math.Sin(radians));
            float z = (float)center.Z + (spawnDistance * (float)Math.Cos(radians));
            float y = (float)center.Y + 25f; // Slight altitude offset to clear ground terrain

            return new Vector3(x, y, z);
        }
    }
}
