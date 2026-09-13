using System;
using UnityEngine;

namespace WaveDefense
{
    public class WaveController : MonoBehaviour
    {
        private bool _footholdSeen;

        private void Update()
        {
            // Only update wave simulation when actively in a gameplay match with spawned constructs
            var activeConstructs = UnityEngine.Object.FindObjectsByType<MainConstructGameObject>(FindObjectsSortMode.None);
            bool hasActiveConstruct = false;
            foreach (var constructObject in activeConstructs)
            {
                if (constructObject.isActiveAndEnabled && constructObject.Active)
                {
                    hasActiveConstruct = true;
                    break;
                }
            }

            if (!hasActiveConstruct)
            {
                return;
            }

            bool footholdAlive = false;
            foreach (var constructObject in activeConstructs)
            {
                if (constructObject.MainConstruct != null &&
                    constructObject.MainConstruct.GetBlueprintName().Equals(
                        WaveDefenseScenario.StartingStructureName, StringComparison.OrdinalIgnoreCase))
                {
                    footholdAlive = true;
                    break;
                }
            }

            var manager = WaveDefensePlugin.WaveManager;
            if (_footholdSeen && !footholdAlive)
            {
                manager?.TriggerDefeat();
                return;
            }

            if (footholdAlive)
            {
                _footholdSeen = true;
            }

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
