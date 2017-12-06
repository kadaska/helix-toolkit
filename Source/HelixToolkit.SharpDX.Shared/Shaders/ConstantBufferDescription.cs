﻿using SharpDX.Direct3D11;
using System;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Utilities;

    [DataContract]
    public class ConstantBufferDescription
    {
        [DataMember]
        public int Slot { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public int StructSize { set; get; }
        [DataMember]
        public BindFlags BindFlags { set; get; } = BindFlags.ConstantBuffer;
        [DataMember]
        public CpuAccessFlags CpuAccessFlags { set; get; } = CpuAccessFlags.Write;
        [DataMember]
        public ResourceOptionFlags OptionFlags { set; get; } = ResourceOptionFlags.None;
        [DataMember]
        public ResourceUsage Usage { set; get; } = ResourceUsage.Dynamic;
        [DataMember]
        public Type StructType { set; get; }

        public ConstantBufferDescription() { }

        public ConstantBufferDescription(string name, int structSize, Type structType, int slot)
        {
            Name = name;
            StructSize = structSize;
            StructType = structType;
            Slot = slot;
        }

        public IBufferProxy CreateBuffer()
        {
            Type genericClass = typeof(ConstantBufferProxy<>);
            Type constructed = genericClass.MakeGenericType(StructType);
            var obj = Activator.CreateInstance(constructed, this);
            return (IBufferProxy)obj;
        }
    }
}
