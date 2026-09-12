using System;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Modding;
using UnityEngine;

namespace WaveDefense
{
    public sealed class WaveDefensePlugin : GamePlugin
    {
        public string name => "WaveDefense";

        public Version version => new Version(0, 3, 1);

        public static WaveManager? WaveManager { get; private set; }
        public static GameObject? RunnerObject { get; private set; }

        public void OnLoad()
        {
            bool startingStructureFound = WaveDefenseScenario.HasStartingStructure(Environment.CurrentDirectory);
            WaveManager = new WaveManager(new WaveConfig(), new WaveSpawner());

            // Create persistent Unity Runner for game loop and HUD
            if (RunnerObject == null)
            {
                RunnerObject = new GameObject("WaveDefense_Runner");
                UnityEngine.Object.DontDestroyOnLoad(RunnerObject);
                RunnerObject.AddComponent<WaveController>();
                RunnerObject.AddComponent<WaveHud>();
            }

            AdvLogger.LogInfo("WaveDefense loaded. Hold Your Ground wave runner & HUD active.");
            AdvLogger.LogInfo("Starting structure: " + WaveDefenseScenario.StartingStructure +
                " | Defense position: " + WaveDefenseScenario.DefensePosition +
                " | Starter asset found: " + startingStructureFound);
        }

        public void OnSave()
        {
        }
    }
}
