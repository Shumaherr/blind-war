﻿using System.Collections.Generic;
using FMOD;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "fmod" + dllSuffix;
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "fmodstudio" + dllSuffix;
    }
}
#endif

namespace FMODUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class PlatformAndroid : Platform
    {
        static PlatformAndroid()
        {
            Settings.AddPlatformTemplate<PlatformAndroid>("2fea114e74ecf3c4f920e1d5cc1c4c40");
        }

        public override string DisplayName => "Android";

        public override void DeclareUnityMappings(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.Android, this);

#if UNITY_EDITOR
            settings.DeclareBuildTarget(BuildTarget.Android, this);
#endif
        }

        public override string GetBankFolder()
        {
            return StaticGetBankFolder();
        }

        public static string StaticGetBankFolder()
        {
            return Settings.Instance.AndroidUseOBB ? Application.streamingAssetsPath : "file:///android_asset";
        }

        public override string GetPluginPath(string pluginName)
        {
            return StaticGetPluginPath(pluginName);
        }

        public static string StaticGetPluginPath(string pluginName)
        {
            return string.Format("lib{0}.so", pluginName);
        }

#if UNITY_EDITOR
        public override Legacy.Platform LegacyIdentifier => Legacy.Platform.Android;

        protected override IEnumerable<string> GetRelativeBinaryPaths(BuildTarget buildTarget, bool allVariants,
            string suffix)
        {
            yield return "android/fmod.jar";

            foreach (var architecture in new[] { "arm64-v8a", "armeabi-v7a", "x86" })
            {
                yield return string.Format("android/{0}/libfmod{1}.so", architecture, suffix);
                yield return string.Format("android/{0}/libfmodstudio{1}.so", architecture, suffix);
            }
        }

        public override bool SupportsAdditionalCPP(BuildTarget target)
        {
            // Unity parses --additional-cpp arguments specified via
            // PlayerSettings.SetAdditionalIl2CppArgs() incorrectly when the Android
            // Export Project option is set.
            return false;
        }
#endif
#if UNITY_EDITOR
        public override OutputType[] ValidOutputTypes => sValidOutputTypes;

        private static readonly OutputType[] sValidOutputTypes =
        {
            new() { displayName = "Java Audio Track", outputType = OUTPUTTYPE.AUDIOTRACK },
            new() { displayName = "OpenSL ES", outputType = OUTPUTTYPE.OPENSL },
            new() { displayName = "AAudio", outputType = OUTPUTTYPE.AAUDIO }
        };

        public override int CoreCount => MaximumCoreCount;
#endif
    }
}