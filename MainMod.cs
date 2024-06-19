using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppMoonClient;
using System.IO;
using System.Text;
using System.Reflection;

namespace ROCSpeedHack
{
    public class MainMod : MelonMod
    {
        public static float runSpeed = 9f;
        public static bool unlockCameraDistance = false;
        public static bool showAllHealthBars = false;
        public static MelonLogger.Instance logger;
        public static bool runSpeedEnabled = true;
        private static bool showRunSpeed = true;
        private string lastSceneName = string.Empty;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            MainMod.logger = LoggerInstance;
            foreach (MethodBase based in HarmonyInstance.GetPatchedMethods())
            {
                logger.Msg($"PATCHED METHOD {based.Name} {based.FullDescription()}");
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (lastSceneName == "GameEntry")
            {
                logger.Msg("Running hooks.lua");
                runLuaFile("hooks.lua", Encoding.UTF8.GetString(Properties.Resources.hooks));
            }
            lastSceneName = sceneName;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                showRunSpeed = !showRunSpeed;
            }

            /*
            if (Input.GetKeyDown(KeyCode.Z))
            {
                autoSkipCutScenes = !autoSkipCutScenes;
                logger.Msg($"Cutscene autoskip set to {autoSkipCutScenes}");
            }
            if (autoSkipCutScenes)
            {
                if (MCutSceneMgr._instance.IsPlaying)
                {
                    MCutSceneMgr._instance.Skip();
                }
            }

            */
        }





        private void runLuaFile(string filename, string defaultContent)
        {
            if (File.Exists(filename))
            {
                MLuaClientHelper.DoLuaString(File.ReadAllText(filename));
            }
            else
            {
                File.WriteAllText(filename, defaultContent);
                runLuaFile(filename, defaultContent);
            }
        }   

        public override void OnGUI()
        {
            if (showRunSpeed)
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.normal.textColor = Color.white;

                // Display the run speed value in a box
                Rect labelRect = new Rect(Screen.width / 2 - 100, 10, 200, 30);
                GUI.Box(labelRect, ""); // Add a background box
                GUI.Label(labelRect, "Run Speed: " + runSpeed.ToString(), style);

                // Plus button for increasing run speed
                Rect plusButtonRect = new Rect(Screen.width / 2 + 105, 10, 30, 30);
                if (GUI.Button(plusButtonRect, "+"))
                {
                    runSpeed = (float)(runSpeed + 0.2);
                    LoggerInstance.Msg("RunSpeed set to " + runSpeed.ToString());
                }

                // Minus button for decreasing run speed
                Rect minusButtonRect = new Rect(Screen.width / 2 - 135, 10, 30, 30);
                if (GUI.Button(minusButtonRect, "-"))
                {
                    runSpeed = (float)(runSpeed - 0.2);
                    LoggerInstance.Msg("RunSpeed set to " + runSpeed.ToString());
                }

                // Button to toggle unlocking camera distance
                Rect CameraButtonRect = new Rect(Screen.width / 2 - 100, 50, 200, 30);
                if (GUI.Button(CameraButtonRect, $"Unlock camera distance {unlockCameraDistance}"))
                {
                    unlockCameraDistance = !unlockCameraDistance;
                    MainMod.logger.Msg($"Unlock camera set to {unlockCameraDistance}");
                }

                // Button to toggle showing all health bars
                Rect HealthBarsButtonRect = new Rect(Screen.width / 2 - 100, 90, 200, 30);
                if (GUI.Button(HealthBarsButtonRect, $"Show all health bars {showAllHealthBars}"))
                {
                    showAllHealthBars = !showAllHealthBars;
                    MainMod.logger.Msg($"Show all health bars set to {showAllHealthBars}");
                }

                // Button to toggle run speed
                Rect toggleButtonRect = new Rect(Screen.width / 2 - 100, 130, 200, 30);
                if (GUI.Button(toggleButtonRect, $"Run Speed ({(runSpeedEnabled ? "Enabled" : "Disabled")})"))
                {
                    runSpeedEnabled = !runSpeedEnabled;
                    MainMod.logger.Msg($"Run speed set to {(runSpeedEnabled ? "Enabled" : "Disabled")}");
                }

                // Button to run Lua file
                Rect luaButtonRect = new Rect(Screen.width / 2 - 100, 170, 200, 30);
                if (GUI.Button(luaButtonRect, "Run Lua"))
                {
                    runLuaFile("inject.lua", Encoding.UTF8.GetString(Properties.Resources.inject));
                }
            }
        }

    }
}
