using BaseX;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ChangePhotoTimer
{
    public class ChangePhotoTimer : NeosMod
    {
        public override string Name => "ChangePhotoTimer";
        public override string Author => "kokoa";
        public override string Version => "1.0.0";
        public override string Link => "";

        internal static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<double> countTimer = new ModConfigurationKey<double>("timeCount", "changeCount", () => 14.0);


        public override void OnEngineInit()
        {
            config = GetConfiguration();
            var harmony = new Harmony("com.kokoa.ChangePhotoTimer");
            harmony.PatchAll();
        }

        public static double GetTimerValue() {
            var time = config.GetValue(countTimer);
            return time;
        }

        [HarmonyPatch]
        class Patch
        {

            [HarmonyTranspiler]
            [HarmonyPatch(typeof(PhotoCaptureManager), "OnCommonUpdate")]
            static IEnumerable<CodeInstruction> OnCommonUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for(var i = 0; i < codes.Count; i++)
                {
                    var code = codes[i];
                    if (code.opcode == OpCodes.Ldc_R8 && codes[i + 1].opcode == OpCodes.Ble_Un_S)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChangePhotoTimer), nameof(ChangePhotoTimer.GetTimerValue)));
                    }
                    if (code.opcode == OpCodes.Ldc_R8 && codes[i + 1].opcode == OpCodes.Ldarg_0)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChangePhotoTimer), nameof(ChangePhotoTimer.GetTimerValue)));
                    }
                }
                return codes;
            }
        }
    }
}