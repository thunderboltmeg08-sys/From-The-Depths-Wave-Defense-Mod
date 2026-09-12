using System;
using UnityEngine;

namespace WaveDefense
{
    public class WaveController : MonoBehaviour
    {
        private void Update()
        {
            // Only update wave simulation when actively in a gameplay match with spawned constructs
            var activeConstructs = UnityEngine.Object.FindObjectsByType<MainConstructGameObject>(FindObjectsSortMode.None);
            if (activeConstructs == null || activeConstructs.Length == 0)
            {
                return;
            }

            var manager = WaveDefensePlugin.WaveManager;
            if (manager != null)
            {
                if (manager.State == WaveState.NotStarted)
                {
                    manager.StartGame();
                }

                manager.Update(Time.deltaTime);
            }
        }
    }
}
