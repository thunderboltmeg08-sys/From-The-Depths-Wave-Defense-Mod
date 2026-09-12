using System;
using UnityEngine;

namespace WaveDefense
{
    public class WaveController : MonoBehaviour
    {
        private void Update()
        {
            var manager = WaveDefensePlugin.WaveManager;
            if (manager != null)
            {
                manager.Update(Time.deltaTime);
            }
        }
    }
}
