﻿
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using Utilities;
    using Shaders;
    using global::SharpDX.Direct3D11;

    public interface IConstantBufferPool
    {
        IBufferProxy Register(ConstantBufferDescription description, Device device);
    }

    public class ConstantBufferPool : BufferPool<string, ConstantBufferDescription>, IConstantBufferPool
    {
        protected override IBufferProxy CreateBuffer(ConstantBufferDescription description)
        {
            return description.CreateBuffer();
        }

        protected override string GetKey(ConstantBufferDescription description)
        {
            return description.StructType.Name + description.StructSize;
        }
    }
}
