﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Render;

        public class InstancingMeshRenderCore : MeshRenderCore
        {
            private IElementsBufferModel parameterBufferModel;
            public IElementsBufferModel ParameterBuffer
            {
                set
                {
                    var old = parameterBufferModel;
                    if(SetAffectsCanRenderFlag(ref parameterBufferModel, value))
                    {
                        if (old != null)
                        {
                            old.ElementChanged -= OnElementChanged;
                        }
                        if (parameterBufferModel != null)
                        {
                            parameterBufferModel.ElementChanged += OnElementChanged;
                        }
                    }
                }
                get { return parameterBufferModel; }
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                return base.OnAttach(technique);
            }
            protected override bool OnUpdateCanRenderFlag()
            {
                return base.OnUpdateCanRenderFlag() && InstanceBuffer != null && InstanceBuffer.HasElements;
            }

            protected override void OnUpdatePerModelStruct(RenderContext context)
            {
                base.OnUpdatePerModelStruct(context);
                modelStruct.HasInstanceParams = ParameterBuffer != null && ParameterBuffer.HasElements ? 1 : 0;
            }

            protected override bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
            {
                if(base.OnAttachBuffers(context, ref vertStartSlot))
                {
                    ParameterBuffer?.AttachBuffer(context, ref vertStartSlot);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

}
