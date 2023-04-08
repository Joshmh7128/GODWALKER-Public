using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DesaturateFeature : ScriptableRendererFeature
{
    class RenderPass : ScriptableRenderPass
    {

        private Material material;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public RenderPass(Material material) : base()
        {
            this.material = material;
            tempTexture.Init("_TempDesaturateTexture");
        }

        public void SetSource(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("DesaturateFeature");

            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDesc, FilterMode.Bilinear);

            Blit(cmd, source, tempTexture.Identifier(), material, 0);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    private RenderPass renderPass;

    public override void Create()
    {
        var material = new Material(Shader.Find("Shader Graphs/Desaturate"));
        this.renderPass = new RenderPass(material);

        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderPass.SetSource(renderer.cameraColorTarget);
        renderer.EnqueuePass(renderPass);
    }
}