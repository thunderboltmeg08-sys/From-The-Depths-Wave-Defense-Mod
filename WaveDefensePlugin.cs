using System;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Modding;

namespace WaveDefense
{
    public sealed class WaveDefensePlugin : GamePlugin
    {
        public string name => "WaveDefense";

        public Version version => new Version(0, 1, 0);

        public void OnLoad()
        {
            bool startingStructureFound = WaveDefenseScenario.HasStartingStructure(Environment.CurrentDirectory);
            AdvLogger.LogInfo("WaveDefense loaded. Starting structure: " +
                WaveDefenseScenario.StartingStructure + ". Fixed defense position: " +
                WaveDefenseScenario.DefensePosition + ". Starter asset found: " +
                startingStructureFound + ".");
        }

        public void OnSave()
        {
        }
    }
}
