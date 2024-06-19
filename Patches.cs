using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMoonClient;

namespace ROCSpeedHack
{
    public static class Patches
    {

        [HarmonyPatch(typeof(Il2CppMoonClient.MEntity))]
        [HarmonyPatch("RunSpeed")]
        [HarmonyPatch(new System.Type[] { typeof(Il2CppMoonClient.MEntity) })]
        [HarmonyPatch(MethodType.Getter)]
        public static class RunPatch
        {
            [HarmonyPostfix]
            static void Postfix(ref float __result, ref Il2CppMoonClient.MEntity __instance)
            {
                if (__instance.IsPlayer && __instance.IsLoaded && MainMod.runSpeedEnabled)
                {
                    __result = MainMod.runSpeed;
                }
            }
        }

        [HarmonyPatch(typeof(MMonsterHUDComponent))]
        [HarmonyPatch(nameof(MMonsterHUDComponent.IsTarget))]
        [HarmonyPatch(MethodType.Normal)]
        public static class HUDPatch
        {
            [HarmonyPostfix]
            static void Postfix(ref bool __result)
            {
                if (MainMod.showAllHealthBars)
                {
                    __result = true;
                }
            }
        }



        [HarmonyPatch(typeof(Il2CppMoonClient.MCameraConfig))]
        [HarmonyPatch("MaxCameraDis")]
        [HarmonyPatch(new System.Type[] { typeof(Il2CppMoonClient.MCameraConfig) })]
        [HarmonyPatch(MethodType.Getter)]

        public static class FOVPatch
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
    }
}
