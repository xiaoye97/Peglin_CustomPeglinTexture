using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using System.IO;
using HarmonyLib;
using PeglinUI.MainMenu;
using Peglin;

namespace CustomPeglinTexture
{
    [BepInPlugin("me.xiaoye97.plugin.Peglin.CustomPeglinTexture", "CustomPeglinTexture", "1.0.0")]
    public class CustomPeglinTexture : BaseUnityPlugin
    {
        public static byte[] PeglinTexBytes;
        private static bool isReplaced;
        private static bool isCanReplace;

        public void Awake()
        {
            Logger.LogMessage("CustomPeglinTexture Awake.");
            isCanReplace = LoadPeglinTexture();
            if (isCanReplace)
            {
                Logger.LogMessage("Loaded peglin texture, can replace.");
                Harmony.CreateAndPatchAll(typeof(CustomPeglinTexture));
            }
            else
            {
                Logger.LogError("Unable to load peglin texture.");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PeglinBattleAnimationController), "OnEnable")]
        public static void PeglinBattleAnimationController_OnEnable_Patch(PeglinBattleAnimationController __instance)
        {
            TryReplaceTexture(__instance.gameObject);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AnimationEventBroadcaster), "AnimationAction")]
        public static void AnimationEventBroadcaster_AnimationAction_Patch(AnimationEventBroadcaster __instance)
        {
            TryReplaceTexture(__instance.gameObject);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ChangeAnimSpeedByVertSpeed), "Start")]
        public static void ChangeAnimSpeedByVertSpeed_Start_Patch(ChangeAnimSpeedByVertSpeed __instance)
        {
            TryReplaceTexture(__instance.gameObject);
        }

        public bool LoadPeglinTexture()
        {
            string path = $"{Paths.PluginPath}/peglin.png";
            try
            {
                if (File.Exists(path))
                {
                    var bytes = File.ReadAllBytes(path);
                    Texture2D tex = new Texture2D(2, 2);
                    if (tex.LoadImage(bytes))
                    {
                        PeglinTexBytes = bytes;
                        return true;
                    }
                }
            }
            catch ( Exception ex)
            {
                Logger.LogError(ex);
            }
            return false;
        }

        public static void TryReplaceTexture(GameObject obj)
        {
            if (isCanReplace)
            {
                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null && sr.sprite.texture != null)
                {
                    if(sr.sprite.texture.name == "peglin")
                    {
                        sr.sprite.texture.LoadImage(PeglinTexBytes);
                    }
                }
            }
        }
    }
}
