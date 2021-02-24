using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.Rendering.Universal.Extent
{
    public class FResourceFactory
    {
        FBufferPool m_BufferPool;
        FTexturePool m_TexturePool;

        public FResourceFactory()
        {
            m_BufferPool = new FBufferPool();
            m_TexturePool = new FTexturePool();
        }

        internal void Reset()
        {

        }

        public BufferRef PullBuffer(in BufferDescription Description)
        {
            ComputeBuffer Buffer;
            int Handle = Description.GetHashCode();

            if (!m_BufferPool.Pull(Handle, out Buffer))
            {
                Buffer = new ComputeBuffer(Description.count, Description.stride, Description.type);
            }

            return new BufferRef(Handle, Buffer);
        }

        internal void PushBuffer(in BufferRef BufferHandle)
        {
            m_BufferPool.Push(BufferHandle.Handle, BufferHandle.Buffer);
        }

        public TextureRef PullTexture(in TextureDescription Description)
        {
            RTHandle Texture;
            int Handle = Description.GetHashCode();

            if (!m_TexturePool.Pull(Handle, out Texture))
            {
                Texture = RTHandles.Alloc(Description.width, Description.height, Description.slices, (DepthBits)Description.depthBufferBits, Description.colorFormat, Description.filterMode, Description.wrapMode, Description.dimension, Description.enableRandomWrite,
                                          Description.useMipMap, Description.autoGenerateMips, Description.isShadowMap, Description.anisoLevel, Description.mipMapBias, (MSAASamples)Description.msaaSamples, Description.bindTextureMS, false, RenderTextureMemoryless.None, Description.name);
            }

            return new TextureRef(Handle, Texture);
        }

        public void PushTexture(in TextureRef TextureHandle)
        {
            m_TexturePool.Push(TextureHandle.Handle, TextureHandle.Texture);
        }

        public void Disposed()
        {
            m_BufferPool.Disposed();
            m_TexturePool.Disposed();
        }
    }
}
