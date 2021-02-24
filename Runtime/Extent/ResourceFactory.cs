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
        int BufferRefID;
        List<FResourceRef> BufferRefs;
        Dictionary<int, int> m_BuffersMap;
        Dictionary<int, int> m_TexturesMap;

        public FResourceFactory()
        {
            m_BufferPool = new FBufferPool();
            m_BuffersMap = new Dictionary<int, int>(16);

            m_TexturePool = new FTexturePool();
            m_TexturesMap = new Dictionary<int, int>(128);

            BufferRefs = new List<FResourceRef>(64);
        }

        internal void Reset()
        {
            m_BuffersMap.Clear();
            m_TexturesMap.Clear();

            BufferRefID = 0;
            BufferRefs.Clear();
        }

        public ComputeBuffer PullBuffer(in BufferDescription Description)
        {
            ComputeBuffer Buffer;
            int CacheIndex = Description.GetHashCode();

            if (!m_BufferPool.Pull(CacheIndex, out Buffer))
            {
                Buffer = new ComputeBuffer(Description.count, Description.stride, Description.type);
            }
            m_BuffersMap[Buffer.GetHashCode()] = CacheIndex;

            return Buffer;
        }

        public void PushBuffer(ComputeBuffer Buffer)
        {
            m_BufferPool.Push(m_BuffersMap[Buffer.GetHashCode()], Buffer);
        }

        public ComputeBuffer AllocateBuffer(in BufferDescription Description)
        {
            ComputeBuffer Buffer;
            FResourceRef ResourceRef = new FResourceRef(Description.GetHashCode());
            int HandleID = 

            if (!m_BufferPool.Pull(ResourceRef.handle, out Buffer))
            {
                Buffer = new ComputeBuffer(Description.count, Description.stride, Description.type);
            }
            m_BuffersMap[Buffer.GetHashCode()] = CacheIndex;

            return Buffer;
        }

        public void ReleaseBuffer(ComputeBuffer Buffer)
        {
            m_BufferPool.Push(m_BuffersMap[Buffer.GetHashCode()], Buffer);
        }

        public RenderTexture AllocateTexture(in TextureDescription Description)
        {
            RenderTexture Texture;
            int CacheIndex = Description.GetHashCode();

            if (!m_TexturePool.Pull(CacheIndex, out Texture))
            {
                Texture = RTHandles.Alloc(Description.width, Description.height, Description.slices, (DepthBits)Description.depthBufferBits, Description.colorFormat, Description.filterMode, Description.wrapMode, Description.dimension, Description.enableRandomWrite,
                                          Description.useMipMap, Description.autoGenerateMips, Description.isShadowMap, Description.anisoLevel, Description.mipMapBias, (MSAASamples)Description.msaaSamples, Description.bindTextureMS, false, RenderTextureMemoryless.None, Description.name);
            }
            m_TexturesMap[Texture.GetHashCode()] = CacheIndex;

            return Texture;
        }

        public void ReleaseTexture(RenderTexture Buffer)
        {
            m_TexturePool.Push(m_TexturesMap[Buffer.GetHashCode()], Buffer);
        }

        public void Disposed()
        {
            m_BufferPool.Disposed();
            m_TexturePool.Disposed();
        }
    }
}
