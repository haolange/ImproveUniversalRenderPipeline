﻿using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal.Extent
{
    public enum EDepthBits
    {
        None = 0,
        Depth8 = 8,
        Depth16 = 16,
        Depth24 = 24,
        Depth32 = 32
    }

    public enum EMSAASamples
    {
        None = 1,
        MSAA2x = 2,
        MSAA4x = 4,
        MSAA8x = 8
    }

    public struct BufferDescription
    {
        public int count;
        public int stride;
        public ComputeBufferType type;
        public string name;

        public BufferDescription(int count, int stride) : this()
        {
            this.count = count;
            this.stride = stride;
            type = ComputeBufferType.Default;
        }

        public BufferDescription(int count, int stride, ComputeBufferType type) : this()
        {
            this.type = type;
            this.count = count;
            this.stride = stride;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode += count;
            hashCode += stride;
            hashCode += (int)type;

            return hashCode;
        }
    }

    public struct TextureDescription
    {
        public string name;

        public int width;
        public int height;
        public int slices;
        public EDepthBits depthBufferBits;
        public GraphicsFormat colorFormat;
        public FilterMode filterMode;
        public TextureWrapMode wrapMode;
        public TextureDimension dimension;
        public bool enableRandomWrite;
        public bool useMipMap;
        public bool autoGenerateMips;
        public bool isShadowMap;
        public int anisoLevel;
        public float mipMapBias;
        public bool enableMSAA;
        public bool bindTextureMS;
        public EMSAASamples msaaSamples;
        public bool clearBuffer;
        public Color clearColor;

        public TextureDescription(int Width, int Height) : this()
        {
            width = Width;
            height = Height;
            slices = 1;

            isShadowMap = false;
            enableRandomWrite = false;

            msaaSamples = EMSAASamples.None;
            depthBufferBits = EDepthBits.None;
            wrapMode = TextureWrapMode.Repeat;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode += width;
            hashCode += height;
            hashCode += slices;
            hashCode += mipMapBias.GetHashCode();
            hashCode += (int)depthBufferBits;
            hashCode += (int)colorFormat;
            hashCode += (int)filterMode;
            hashCode += (int)wrapMode;
            hashCode += (int)dimension;
            hashCode += anisoLevel;
            hashCode += (enableRandomWrite ? 1 : 0);
            hashCode += (useMipMap ? 1 : 0);
            hashCode += (autoGenerateMips ? 1 : 0);
            hashCode += (isShadowMap ? 1 : 0);
            hashCode += (bindTextureMS ? 1 : 0);

            return hashCode;
        }
    }

    public struct BufferRef
    {
        internal int Handle;
        public ComputeBuffer Buffer;

        internal BufferRef(int InHandle, ComputeBuffer InBuffer) { Handle = InHandle; Buffer = InBuffer; }
        public static implicit operator ComputeBuffer(BufferRef BufferHandle) => BufferHandle.Buffer;
    }

    public struct TextureRef
    {
        internal int Handle;
        public RTHandle Texture;

        internal TextureRef(int InHandle, RTHandle InTexture) { Handle = InHandle; Texture = InTexture; }
        public static implicit operator RTHandle(TextureRef TextureHandle) => TextureHandle.Texture;
    }

    public abstract class FGPUResourcePool<Type> where Type : class
    {
        protected Dictionary<int, List<Type>> m_ResourcePool = new Dictionary<int, List<Type>>(64);

        abstract protected void ReleaseInternalResource(Type res);
        abstract protected string GetResourceName(Type res);
        abstract protected string GetResourceTypeName();

        public bool Pull(int hashCode, out Type resource)
        {
            if (m_ResourcePool.TryGetValue(hashCode, out var list) && list.Count > 0)
            {
                resource = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return true;
            }

            resource = null;
            return false;
        }

        public void Push(int hash, Type resource)
        {
            if (!m_ResourcePool.TryGetValue(hash, out var list))
            {
                list = new List<Type>();
                m_ResourcePool.Add(hash, list);
            }

            list.Add(resource);
        }

        public void Disposed()
        {
            foreach (var kvp in m_ResourcePool)
            {
                foreach (Type resource in kvp.Value)
                {
                    ReleaseInternalResource(resource);
                }
            }
        }
    }

    public class FTexturePool : FGPUResourcePool<RTHandle>
    {
        protected override void ReleaseInternalResource(RTHandle res)
        {
            res.Release();
        }

        protected override string GetResourceName(RTHandle res)
        {
            return res.name;
        }

        override protected string GetResourceTypeName()
        {
            return "Texture";
        }
    }

    public class FBufferPool : FGPUResourcePool<ComputeBuffer>
    {
        protected override void ReleaseInternalResource(ComputeBuffer res)
        {
            res.Release();
        }

        protected override string GetResourceName(ComputeBuffer res)
        {
            return "BufferNameNotAvailable";
        }

        override protected string GetResourceTypeName()
        {
            return "Buffer";
        }
    }
}
