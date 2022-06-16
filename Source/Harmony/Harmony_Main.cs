using HarmonyLib;

namespace InTheDark
{
    [HarmonyPatch]
    static partial class HarmonyPatches
    {
        public static void Init()
        {
            var harmony = new Harmony("com.inthedark.jesterromut.mod");
            harmony.PatchAll();
        }
    }
}