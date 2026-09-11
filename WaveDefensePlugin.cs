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
            AdvLogger.LogInfo("WaveDefense loaded. Wave system is ready for implementation.");
        }

        public void OnSave()
        {
        }
    }
}
