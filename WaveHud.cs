using System;
using UnityEngine;

namespace WaveDefense
{
    public class WaveHud : MonoBehaviour
    {
        private GUIStyle? _boxStyle;
        private GUIStyle? _headerStyle;
        private GUIStyle? _timerStyle;
        private GUIStyle? _infoStyle;
        private GUIStyle? _buttonStyle;
        private bool _stylesInitialized = false;

        public bool IsVisible { get; set; } = true;

        private void Update()
        {
            // Toggle HUD visibility with F8 key
            if (Input.GetKeyDown(KeyCode.F8))
            {
                IsVisible = !IsVisible;
            }
        }

        private void InitStyles()
        {
            if (_stylesInitialized) return;

            Texture2D bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, new Color(0.08f, 0.08f, 0.12f, 0.88f));
            bgTex.Apply();

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = bgTex },
                padding = new RectOffset(16, 16, 10, 10),
                alignment = TextAnchor.MiddleCenter
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            _timerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            _infoStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            _stylesInitialized = true;
        }

        private void OnGUI()
        {
            if (!IsVisible) return;

            var manager = WaveDefensePlugin.WaveManager;
            if (manager == null) return;

            // Only show when a game session has started or in progress
            if (manager.State == WaveState.NotStarted)
            {
                DrawStartPrompt(manager);
                return;
            }

            InitStyles();

            float hudWidth = 420f;
            float hudHeight = 135f;
            float posX = (Screen.width - hudWidth) / 2f;
            float posY = 15f;

            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(posX, posY, hudWidth, hudHeight), _boxStyle);
            GUILayout.BeginVertical();

            int timeSeconds = Mathf.Max(0, Mathf.CeilToInt(manager.StateTimeRemaining));
            int minutes = timeSeconds / 60;
            int seconds = timeSeconds % 60;
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (manager.State == WaveState.Intermission)
            {
                _headerStyle!.normal.textColor = new Color(1f, 0.82f, 0.2f); // Amber / Gold
                _timerStyle!.normal.textColor = new Color(1f, 0.95f, 0.6f);

                GUILayout.Label("🛡️ FORTIFICATION PHASE", _headerStyle);
                GUILayout.Label("Next Wave In: " + timeString, _timerStyle);
                GUILayout.Label(string.Format("Preparing for Wave {0} (Target Difficulty: {1:F0})",
                    manager.CurrentWaveNumber + 1,
                    manager.Config.CalculateDifficulty(manager.CurrentWaveNumber + 1)), _infoStyle);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("⚡ Start Wave Now", _buttonStyle, GUILayout.Width(140), GUILayout.Height(24)))
                {
                    manager.SkipIntermission();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (manager.State == WaveState.WaveActive)
            {
                _headerStyle!.normal.textColor = new Color(1f, 0.28f, 0.28f); // Crimson / Red
                _timerStyle!.normal.textColor = new Color(1f, 0.65f, 0.65f);

                GUILayout.Label(string.Format("⚔️ WAVE {0} ASSAULT", manager.CurrentWaveNumber), _headerStyle);
                GUILayout.Label("Survive For: " + timeString, _timerStyle);
                GUILayout.Label(string.Format("Difficulty: {0:F0} | Objective: Hold the Foothold!",
                    manager.CurrentTargetDifficulty), _infoStyle);
                GUILayout.Label(string.Format("Bounty Earned: +{0:F0} Materials | [F8] Toggle HUD",
                    manager.TotalMaterialsAwarded), _infoStyle);
            }
            else if (manager.State == WaveState.Defeated)
            {
                _headerStyle!.normal.textColor = Color.red;
                _timerStyle!.normal.textColor = Color.white;

                GUILayout.Label("💀 FOOTHOLD OVERWHELMED", _headerStyle);
                GUILayout.Label(string.Format("Waves Survived: {0}", Mathf.Max(0, manager.CurrentWaveNumber - 1)), _timerStyle);
                GUILayout.Label(string.Format("Total Bounty Collected: {0:F0} Materials", manager.TotalMaterialsAwarded), _infoStyle);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawStartPrompt(WaveManager manager)
        {
            InitStyles();

            float width = 320f;
            float height = 75f;
            float posX = (Screen.width - width) / 2f;
            float posY = 15f;

            GUILayout.BeginArea(new Rect(posX, posY, width, height), _boxStyle);
            GUILayout.BeginVertical();

            _headerStyle!.normal.textColor = new Color(0.4f, 0.85f, 1f);
            GUILayout.Label("HOLD YOUR GROUND", _headerStyle);

            if (GUILayout.Button("⚔️ Start Wave Defense", _buttonStyle, GUILayout.Height(30)))
            {
                manager.StartGame();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
