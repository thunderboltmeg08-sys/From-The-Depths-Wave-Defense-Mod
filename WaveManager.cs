using System;
using BrilliantSkies.Core.Logger;
using UnityEngine;

namespace WaveDefense
{
    public enum WaveState
    {
        NotStarted,
        Intermission,
        WaveActive,
        WaveCompleted,
        Defeated
    }

    public class WaveManager
    {
        public WaveConfig Config { get; }
        public WaveSpawner Spawner { get; }

        public int CurrentWaveNumber { get; private set; } = 0;
        public WaveState State { get; private set; } = WaveState.NotStarted;
        public float StateTimeRemaining { get; private set; } = 0f;
        public float CurrentTargetDifficulty { get; private set; } = 0f;
        public float TotalMaterialsAwarded { get; private set; } = 0f;

        public event Action<int, float>? OnWaveStarted;
        public event Action<int, float>? OnWaveSurvived;
        public event Action<float>? OnIntermissionStarted;

        public WaveManager(WaveConfig? config = null, WaveSpawner? spawner = null)
        {
            Config = config ?? new WaveConfig();
            Spawner = spawner ?? new WaveSpawner();
        }

        public void StartGame()
        {
            CurrentWaveNumber = 0;
            TotalMaterialsAwarded = 0f;
            StartIntermission(Config.IntermissionSeconds);
            AdvLogger.LogInfo("Hold Your Ground: Wave Defense initialized. Prepare for Wave 1.");
        }

        public void Update(float deltaTime)
        {
            if (State == WaveState.NotStarted || State == WaveState.Defeated)
            {
                return;
            }

            StateTimeRemaining -= deltaTime;

            if (StateTimeRemaining <= 0f)
            {
                OnStateTimerExpired();
            }
        }

        private void OnStateTimerExpired()
        {
            if (State == WaveState.Intermission)
            {
                AdvanceToNextWave();
            }
            else if (State == WaveState.WaveActive)
            {
                CompleteCurrentWave();
            }
        }

        private void AdvanceToNextWave()
        {
            CurrentWaveNumber++;
            CurrentTargetDifficulty = Config.CalculateDifficulty(CurrentWaveNumber);
            State = WaveState.WaveActive;
            StateTimeRemaining = Config.WaveDurationSeconds;

            AdvLogger.LogInfo(string.Format("=== WAVE {0} STARTED === Difficulty: {1:F1} | Survive for {2:F0}s",
                CurrentWaveNumber, CurrentTargetDifficulty, Config.WaveDurationSeconds));

            // Execute enemy physical spawning for this wave
            try
            {
                var availableDesigns = Spawner.GetAvailableEnemyDesigns();
                var selectedDesigns = Spawner.SelectWaveDesigns(availableDesigns, CurrentTargetDifficulty);

                AdvLogger.LogInfo(string.Format("WaveDefense: found {0} enemy designs, selected {1} for wave {2}.",
                    Spawner.LastAvailableDesignCount, Spawner.LastSelectedDesignCount, CurrentWaveNumber));

                System.Random rng = new System.Random();
                float baseAngle = (float)(rng.NextDouble() * 360.0);

                for (int i = 0; i < selectedDesigns.Count; i++)
                {
                    float angle = (baseAngle + (i * 25f)) % 360f;
                    Vector3 spawnPos = Spawner.CalculateSpawnPosition(
                        WaveDefenseScenario.DefensePosition,
                        Config.SpawnDistance,
                        angle);

                    Spawner.SpawnEnemyUnit(selectedDesigns[i], spawnPos);
                }

                if (selectedDesigns.Count == 0)
                {
                    Debug.LogError("WaveDefense: no enemy designs were available for this wave.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("WaveManager: Error executing wave spawn: " + ex);
            }

            OnWaveStarted?.Invoke(CurrentWaveNumber, CurrentTargetDifficulty);
        }

        private void CompleteCurrentWave()
        {
            State = WaveState.WaveCompleted;
            float reward = Config.CalculateWaveReward(CurrentWaveNumber);
            TotalMaterialsAwarded += reward;

            AdvLogger.LogInfo(string.Format("=== WAVE {0} SURVIVED! === Reward: +{1:F0} Materials | Total Earned: {2:F0}",
                CurrentWaveNumber, reward, TotalMaterialsAwarded));

            OnWaveSurvived?.Invoke(CurrentWaveNumber, reward);

            // Transition to intermission for next wave preparation
            StartIntermission(Config.IntermissionSeconds);
        }

        private void StartIntermission(float seconds)
        {
            State = WaveState.Intermission;
            StateTimeRemaining = seconds;

            AdvLogger.LogInfo(string.Format("Intermission: Next wave in {0:F0}s. Fortify defenses and spend resources.", seconds));
            OnIntermissionStarted?.Invoke(seconds);
        }

        public void SkipIntermission()
        {
            if (State == WaveState.Intermission)
            {
                StateTimeRemaining = 0f;
                AdvanceToNextWave();
            }
        }

        public void TriggerDefeat()
        {
            State = WaveState.Defeated;
            AdvLogger.LogInfo(string.Format("Foothold Overwhelmed! Survived {0} waves. Total materials earned: {1:F0}",
                CurrentWaveNumber > 0 ? CurrentWaveNumber - 1 : 0, TotalMaterialsAwarded));
        }
    }
}
