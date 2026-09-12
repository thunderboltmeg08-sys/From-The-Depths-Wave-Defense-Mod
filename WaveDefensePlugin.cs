using System;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Modding;
using UnityEngine;

namespace WaveDefense
{
    public sealed class WaveDefensePlugin : GamePlugin, GamePlugin_PostLoad
    {
        public string name => "WaveDefense";

        public Version version => new Version(0, 3, 4);

        public static WaveManager? WaveManager { get; private set; }
        public static GameObject? RunnerObject { get; private set; }

        public void OnLoad()
        {
            try
            {
                bool startingStructureFound = WaveDefenseScenario.HasStartingStructure(Environment.CurrentDirectory);
                WaveManager = new WaveManager(new WaveConfig(), new WaveSpawner());

                AdvLogger.LogInfo("WaveDefense loaded: Hold Your Ground wave manager initialized.");
                AdvLogger.LogInfo("Starting structure: " + WaveDefenseScenario.StartingStructure +
                    " | Defense position: " + WaveDefenseScenario.DefensePosition +
                    " | Starter asset found: " + startingStructureFound);

                EnsureRunner();
            }
            catch (Exception ex)
            {
                Debug.LogError("WaveDefense OnLoad error: " + ex);
            }
        }

        public void OnSave()
        {
        }

        public bool AfterAllPluginsLoaded()
        {
            try
            {
                EnsureRunner();
                AdvLogger.LogInfo("WaveDefense: AfterAllPluginsLoaded confirmed runner active.");
            }
            catch (Exception ex)
            {
                Debug.LogError("WaveDefense AfterAllPluginsLoaded error: " + ex);
            }

            return true;
        }

        private static void EnsureRunner()
        {
            if (RunnerObject == null)
            {
                RunnerObject = new GameObject("WaveDefense_Runner");
                UnityEngine.Object.DontDestroyOnLoad(RunnerObject);
                RunnerObject.AddComponent<WaveController>();
                RunnerObject.AddComponent<WaveHud>();
            }
        }
    }
}
