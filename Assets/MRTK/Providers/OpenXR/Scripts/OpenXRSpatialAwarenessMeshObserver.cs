// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

#if MSFT_OPENXR_0_9_4_OR_NEWER
using Microsoft.MixedReality.OpenXR;
using Unity.Profiling;
using UnityEngine.XR.OpenXR;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "OpenXR Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK",
        true,
        SupportedUnityXRPipelines.XRSDK)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/spatial-awareness-getting-started")]
    public class OpenXRSpatialAwarenessMeshObserver :
        GenericXRSDKSpatialMeshObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenXRSpatialAwarenessMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        protected override bool IsActiveLoader =>
#if MSFT_OPENXR_0_9_4_OR_NEWER
            LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>();
#else
            false;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER

#if MSFT_OPENXR_0_9_4_OR_NEWER
        private static readonly ProfilerMarker ConfigureObserverVolumePerfMarker = new ProfilerMarker($"[MRTK] {nameof(OpenXRSpatialAwarenessMeshObserver)}.ConfigureObserverVolume");

        /// <inheritdoc/>
        protected override void ConfigureObserverVolume()
        {
            using (ConfigureObserverVolumePerfMarker.Auto())
            {
                base.ConfigureObserverVolume();

                MixedRealitySpatialAwarenessMeshObserverProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessMeshObserverProfile;
                if (profile == null)
                {
                    return;
                }

                MeshComputeSettings settings = new MeshComputeSettings
                {
                    MeshType = (profile.DisplayOption == SpatialAwarenessMeshDisplayOptions.Visible) ? MeshType.Visual : MeshType.Collider,
                    VisualMeshLevelOfDetail = MapMRTKLevelOfDetailToOpenXR(profile.LevelOfDetail),
                    OcclusionHint = true
                };

                MeshSettings.SetMeshComputeSettings(settings);
            }
        }

        private VisualMeshLevelOfDetail MapMRTKLevelOfDetailToOpenXR(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            switch (levelOfDetail)
            {
                case SpatialAwarenessMeshLevelOfDetail.Coarse:
                    return VisualMeshLevelOfDetail.Coarse;
                case SpatialAwarenessMeshLevelOfDetail.Medium:
                    return VisualMeshLevelOfDetail.Medium;
                case SpatialAwarenessMeshLevelOfDetail.Fine:
                    return VisualMeshLevelOfDetail.Fine;
                case SpatialAwarenessMeshLevelOfDetail.Unlimited:
                    return VisualMeshLevelOfDetail.Unlimited;
                case SpatialAwarenessMeshLevelOfDetail.Custom:
                default:
                    Debug.LogError($"Unsupported LevelOfDetail value {levelOfDetail}. Defaulting to {VisualMeshLevelOfDetail.Coarse}");
                    return VisualMeshLevelOfDetail.Coarse;
            }
        }
#endif // MSFT_OPENXR_0_9_4_OR_NEWER
    }
}
