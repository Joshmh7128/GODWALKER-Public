using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace VolumetricLights {

    public class VolumetricLightsDepthPrePassFeature : ScriptableRendererFeature {

        static class ShaderParams {
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int CustomDepthTexture = Shader.PropertyToID("_CustomDepthTexture");
            public static int CustomDepthAlphaCutoff = Shader.PropertyToID("_AlphaCutOff");
            public static int CustomDepthBaseMap = Shader.PropertyToID("_BaseMap");

            public const string SKW_DEPTH_PREPASS = "VF2_DEPTH_PREPASS";
            public const string SKW_CUSTOM_DEPTH_ALPHA_TEST = "DEPTH_PREPASS_ALPHA_TEST";
        }


        public class DepthRenderPass : ScriptableRenderPass {

            public VolumetricLightsDepthPrePassFeature settings;

            const string m_ProfilerTag = "CustomDepthPrePass";
            const string m_DepthOnlyShader = "Hidden/VolumetricLights/DepthOnly";
            const string m_CustomDepthTextureName = "_CustomDepthTexture";

            FilteringSettings m_FilteringSettings;
            int currentCutoutLayerMask;
            readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
            readonly List<Renderer> cutOutRenderers = new List<Renderer>();

            RTHandle m_Depth;
            Material depthOnlyMaterial, depthOnlyMaterialCutOff;
            Material[] depthOverrideMaterials;

            public DepthRenderPass(VolumetricLightsDepthPrePassFeature settings) {
                this.settings = settings;
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

                //m_Depth.Init("_CustomDepthTexture");
                m_Depth = RTHandles.Alloc(ShaderParams.CustomDepthTexture, name: m_CustomDepthTextureName);
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
                m_FilteringSettings = new FilteringSettings(RenderQueueRange.transparent, 0);
                SetupKeywords();
                FindAlphaClippingRenderers();
            }

            void SetupKeywords() {
                if (settings.transparentLayerMask != 0 || settings.alphaCutoutLayerMask != 0) {
                    Shader.EnableKeyword(ShaderParams.SKW_DEPTH_PREPASS);
                } else {
                    Shader.DisableKeyword(ShaderParams.SKW_DEPTH_PREPASS);
                }
            }

            void FindAlphaClippingRenderers() {
                cutOutRenderers.Clear();
                if (settings.alphaCutoutLayerMask == 0) return;
                Renderer[] rr = FindObjectsOfType<Renderer>();
                for (int r = 0; r < rr.Length; r++) {
                    if (((1 << rr[r].gameObject.layer) & settings.alphaCutoutLayerMask) != 0) {
                        cutOutRenderers.Add(rr[r]);
                    }
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                if (settings.transparentLayerMask != m_FilteringSettings.layerMask || settings.alphaCutoutLayerMask != currentCutoutLayerMask) {
                    m_FilteringSettings = new FilteringSettings(RenderQueueRange.transparent, settings.transparentLayerMask);
                    if (settings.alphaCutoutLayerMask != currentCutoutLayerMask) {
                        FindAlphaClippingRenderers();
                    }
                    currentCutoutLayerMask = settings.alphaCutoutLayerMask;
                    SetupKeywords();
                }
                RenderTextureDescriptor depthDesc = cameraTextureDescriptor;
                depthDesc.colorFormat = RenderTextureFormat.Depth;
                depthDesc.depthBufferBits = 32;
                depthDesc.msaaSamples = 1;

                cmd.GetTemporaryRT(ShaderParams.CustomDepthTexture, depthDesc, FilterMode.Point);
                cmd.SetGlobalTexture(ShaderParams.CustomDepthTexture, m_Depth);
                ConfigureTarget(m_Depth); //.Identifier());
                ConfigureClear(ClearFlag.All, Color.black);

            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                if (settings.transparentLayerMask == 0 && settings.alphaCutoutLayerMask == 0) return;
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                if (settings.alphaCutoutLayerMask != 0) {
                    if (depthOnlyMaterialCutOff == null) {
                        Shader depthOnlyCutOff = Shader.Find(m_DepthOnlyShader);
                        depthOnlyMaterialCutOff = new Material(depthOnlyCutOff);
                    }
                    int renderersCount = cutOutRenderers.Count;
                    if (depthOverrideMaterials == null || depthOverrideMaterials.Length < renderersCount) {
                        depthOverrideMaterials = new Material[renderersCount];
                    }
                    for (int k = 0; k < renderersCount; k++) {
                        Renderer renderer = cutOutRenderers[k];
                        if (renderer != null && renderer.isVisible) {
                            Material mat = renderer.sharedMaterial;
                            if (mat != null) {
                                if (depthOverrideMaterials[k] == null) {
                                    depthOverrideMaterials[k] = Instantiate(depthOnlyMaterialCutOff);
                                    depthOverrideMaterials[k].EnableKeyword(ShaderParams.SKW_CUSTOM_DEPTH_ALPHA_TEST);
                                }
                                Material overrideMaterial = depthOverrideMaterials[k];
                                overrideMaterial.SetFloat(ShaderParams.CustomDepthAlphaCutoff, settings.alphaCutOff);
                                if (mat.HasProperty(ShaderParams.CustomDepthBaseMap)) {
                                    overrideMaterial.SetTexture(ShaderParams.MainTex, mat.GetTexture(ShaderParams.CustomDepthBaseMap));
                                } else if (mat.HasProperty(ShaderParams.MainTex)) {
                                    overrideMaterial.SetTexture(ShaderParams.MainTex, mat.GetTexture(ShaderParams.MainTex));
                                }
                                cmd.DrawRenderer(renderer, overrideMaterial);
                            }
                        }
                    }

                }

                if (settings.transparentLayerMask != 0) {
                    SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
                    var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
                    drawSettings.perObjectData = PerObjectData.None;
                    if (depthOnlyMaterial == null) {
                        Shader depthOnly = Shader.Find(m_DepthOnlyShader);
                        depthOnlyMaterial = new Material(depthOnly);
                    }
                    drawSettings.overrideMaterial = depthOnlyMaterial;
                    context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);
                }

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(ShaderParams.CustomDepthTexture);
            }

            public void CleanUp() {
                RTHandles.Release(m_Depth);
            }
        }

        [Tooltip("Optionally specify which transparent layers must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
        public LayerMask transparentLayerMask;
        [Tooltip("Optionally specify which semi-transparent (materials using alpha clipping or cut-off) must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
        public LayerMask alphaCutoutLayerMask;
        [Tooltip("Optionally determines the alpha cut off for semitransparent objects.")]
        [Range(0, 1)]
        public float alphaCutOff;

        [Tooltip("If this depth pre-pass render feature can execute on reflection probes.")]
        public bool ignoreReflectionProbes = true;

        DepthRenderPass m_ScriptablePass;
        public static bool installed;

        public override void Create() {
            m_ScriptablePass = new DepthRenderPass(this);
        }

        void OnDestroy() {
            installed = false;
            if (m_ScriptablePass != null) {
                m_ScriptablePass.CleanUp();
            }
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            Camera cam = renderingData.cameraData.camera;
            if (cam.targetTexture != null && cam.targetTexture.format == RenderTextureFormat.Depth) return; // ignore occlusion cams!
            if (ignoreReflectionProbes && cam.cameraType == CameraType.Reflection) return;

            installed = true;
            renderer.EnqueuePass(m_ScriptablePass);
        }

    }



}