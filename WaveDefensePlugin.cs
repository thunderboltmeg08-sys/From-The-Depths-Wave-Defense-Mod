using System;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Modding;

namespace WaveDefense
{
    public sealed class WaveDefensePlugin : GamePlugin
    {
        public string name => "WaveDefense";

        public Version version => new Version(0, 3, 0);

        public static WaveManager? WaveManager { get; private set; }

        public void OnLoad()
        {
            bool startingStructureFound = WaveDefenseScenario.HasStartingStructure(Environment.CurrentDirectory);
            WaveManager = new WaveManager(new WaveConfig(), new WaveSpawner());

            AdvLogger.LogInfo("WaveDefense loaded. Hold Your Ground wave system initialized.");
            AdvLogger.LogInfo("Starting structure: " + WaveDefenseScenario.StartingStructure +
                " | Defense position: " + WaveDefenseScenario.DefensePosition +
                " | Starter asset found: " + startingStructureFound);
        }

        public void OnSave()
        {
        }
    }
}
