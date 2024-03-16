using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppMoonClient;

using Vector3 = UnityEngine.Vector3;
using System.IO;

namespace ROCSpeedHack
{
    public class MainMod : MelonMod
    {
        public static float runSpeed = 9f;
        public static bool unlockCameraDistance = false;
        public static bool showAllHealthBars = false;
        public static Vector3 playerPos;
        public static MelonLogger.Instance logger;
        public static bool runSpeedEnabled = true;
        private static bool showRunSpeed = true;
        public static MPlayer player;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            MainMod.logger = LoggerInstance;
            HarmonyInstance.PatchAll();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                showRunSpeed = !showRunSpeed;
            }
        }

        [HarmonyPatch(typeof(Il2CppMoonClient.MEntity))]
        [HarmonyPatch("RunSpeed")]
        [HarmonyPatch(new System.Type[] { typeof(Il2CppMoonClient.MEntity) })]
        [HarmonyPatch(MethodType.Getter)]
        public class Patch
        {
            [HarmonyPostfix]
            static void Postfix(ref float __result, ref Il2CppMoonClient.MEntity __instance)
            {
                if (__instance.IsPlayer && __instance.IsLoaded && runSpeedEnabled)
                {
                    __result = MainMod.runSpeed;
                }
            }
        }

        [HarmonyPatch(typeof(MMonsterHUDComponent))]
        [HarmonyPatch(nameof(MMonsterHUDComponent.IsTarget))]
        [HarmonyPatch(MethodType.Normal)]
        public class HUDPatch
        {
            [HarmonyPostfix]
            static void Postfix(ref bool __result)
            {
                if (showAllHealthBars)
                {
                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(MMoveComponent))]
        [HarmonyPatch(nameof(MMoveComponent.TargetPos))]
        [HarmonyPatch(new System.Type[] { typeof(MMoveComponent) })]
        [HarmonyPatch(MethodType.Getter)]
        public class MovementPatch
        {
            [HarmonyPostfix]
            static void Postfix(ref Vector3 __result, ref MMoveComponent __instance)
            {
                MainMod.player = __instance.Player;
                MainMod.playerPos = __result;
            }
        }

        [HarmonyPatch(typeof(Il2CppMoonClient.MCameraConfig))]
        [HarmonyPatch("MaxCameraDis")]
        [HarmonyPatch(new System.Type[] { typeof(Il2CppMoonClient.MCameraConfig) })]
        [HarmonyPatch(MethodType.Getter)]
        public class FOVPatch
        {
            [HarmonyPostfix]
            static void Postfix(ref float __result, ref Il2CppMoonClient.MCameraConfig __instance)
            {
                if (MainMod.unlockCameraDistance)
                {
                    __result = __result * 3;
                }
            }
        }

        private void runLuaFile()
        {
            if (File.Exists("inject.lua"))
            {
                MLuaClientHelper.DoLuaString(File.ReadAllText("inject.lua"));
            }
            else
            {
                File.WriteAllText("inject.lua", "CommonUI.Dialog.ShowOKDlg(true, nil, \"Lua injection working!\", function()\r\nend)");
                runLuaFile();
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
                    runSpeed++;
                    LoggerInstance.Msg("RunSpeed set to " + runSpeed.ToString());
                }

                // Minus button for decreasing run speed
                Rect minusButtonRect = new Rect(Screen.width / 2 - 135, 10, 30, 30);
                if (GUI.Button(minusButtonRect, "-"))
                {
                    runSpeed--;
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
                    runLuaFile();
                }
            }
        }
    }
}
