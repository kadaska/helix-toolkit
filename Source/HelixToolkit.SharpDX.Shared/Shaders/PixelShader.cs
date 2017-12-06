﻿using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// Pixel Shader
    /// </summary>
    public class PixelShader : ShaderBase
    {
        private readonly global::SharpDX.Direct3D11.PixelShader shader;

        /// <summary>
        /// Pixel Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public PixelShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Pixel)
        {
            shader = Collect(new global::SharpDX.Direct3D11.PixelShader(device, byteCode));
        }
        /// <summary>
        /// <see cref="ShaderBase.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void Bind(DeviceContext context)
        {
            context.PixelShader.Set(shader);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.CBufferMapping)
            {
                context.PixelShader.SetConstantBuffer(buff.Item1, buff.Item2.Buffer);
            }
        }

        /// <summary>
        /// <see cref="ShaderBase.BindTexture(DeviceContext, string, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
            context.VertexShader.SetShaderResource(GetTextureIndex(name), texture);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
            context.VertexShader.SetShaderResource(index, texture);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindTextures(DeviceContext, IEnumerable{Tuple{int, ShaderResourceView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public override void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures)
        {
            foreach (var texture in textures)
            {
                context.VertexShader.SetShaderResource(texture.Item1, texture.Item2);
            }
        }
    }
}
