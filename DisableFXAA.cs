using HarmonyLib;
using ResoniteModLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DisableFXAA
{
    public class DisableFXAA : ResoniteMod
    {
        public override string Name => "DisableFXAA";
        public override string Author => "DoubleStyx, FalsePattern";
        public override string Version => "2.0.0";
        public override string Link => "https://github.com/FalsePattern/DisableFXAA";

        private static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> fxaaEnabled = new ModConfigurationKey<bool>("Enable FXAA", "", () => false);

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.DoubleStyx.DisableFXAA");
            harmony.PatchAll();

            config = GetConfiguration();
            ModConfiguration.OnAnyConfigurationChanged += OnConfigurationChanged;
        }

        private void OnConfigurationChanged(ConfigurationChangedEvent @event)
        {
            Camera main = Camera.main;
            SetFxaaActive(main, config.GetValue(fxaaEnabled));
        }

        public static void SetFxaaActive(Camera camera, bool state)
        {
            PostProcessLayer layer = camera.GetComponent<PostProcessLayer>();
            layer.antialiasingMode = state
                ? PostProcessLayer.Antialiasing.FastApproximateAntialiasing
                : PostProcessLayer.Antialiasing.None;
        }

        [HarmonyPatch(typeof(CameraInitializer), "SetupPostProcessing")]
        static class Patch
        {
            static void Postfix(Camera c, bool motionBlur, bool screenspaceReflections, bool mainCamera = false, bool vr = false, bool singleCapture = false)
            {
                Camera main = Camera.main;
                SetFxaaActive(main, config.GetValue(fxaaEnabled));
            }
        }
    }
}