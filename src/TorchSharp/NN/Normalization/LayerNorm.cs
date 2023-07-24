// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.
using System;
using static TorchSharp.torch;
using static TorchSharp.PInvoke.LibTorchSharp;

#nullable enable
namespace TorchSharp
{
    using Modules;

    namespace Modules
    {

        /// <summary>
        /// This class is used to represent a LayerNorm module.
        /// </summary>
        public sealed class LayerNorm : torch.nn.Module<Tensor, Tensor>
        {
            internal LayerNorm(IntPtr handle, IntPtr boxedHandle) : base(handle, boxedHandle)
            {
            }

            public override Tensor forward(Tensor tensor)
            {
                var res = THSNN_LayerNorm_forward(handle.DangerousGetHandle(), tensor.Handle);
                if (res == IntPtr.Zero) { torch.CheckForErrors(); }
                return new Tensor(res);
            }

            public Parameter? bias {
                get {
                    var res = THSNN_LayerNorm_bias(handle);
                    if (res == IntPtr.Zero) { torch.CheckForErrors(); }
                    return (res == IntPtr.Zero) ? null : new Parameter(res);
                }
                set {
                    THSNN_LayerNorm_set_bias(handle, (value is null ? IntPtr.Zero : value.Handle));
                    torch.CheckForErrors();
                    ConditionallyRegisterParameter("bias", value);
                }
            }

            public Parameter? weight {
                get {
                    var res = THSNN_LayerNorm_weight(handle);
                    if (res == IntPtr.Zero) { torch.CheckForErrors(); }
                    return (res == IntPtr.Zero) ? null : new Parameter(res);
                }
                set {
                    THSNN_LayerNorm_set_weight(handle, value is null ? IntPtr.Zero : value.Handle);
                    torch.CheckForErrors();
                    ConditionallyRegisterParameter("weight", value);
                }
            }

            protected internal override Module _to(Device device, ScalarType dtype)
            {
                if (device.type != DeviceType.DIRECTML) return base._to(device, dtype);

                if (bias is not null) {
                    bias = bias.to(dtype, device).AsParameter();
                }
                if (weight is not null) {
                    weight = weight.to(dtype, device).AsParameter();
                }
                _toEpilog(device, dtype);
                return this;
            }

            protected internal override Module _to(DeviceType deviceType, int deviceIndex = -1)
            {
                if (deviceType != DeviceType.DIRECTML) return base._to(deviceType, deviceIndex);

                if (bias is not null) {
                    bias = bias.to(deviceType, deviceIndex).AsParameter();
                }
                if (weight is not null) {
                    weight = weight.to(deviceType, deviceIndex).AsParameter();
                }
                _toEpilog(deviceType, deviceIndex);
                return this;
            }
        }
    }

    public static partial class torch
    {
        public static partial class nn
        {
            /// <summary>
            /// Applies Layer Normalization over a mini-batch of inputs as described in the paper Layer Normalization
            /// </summary>
            /// <param name="normalized_shape">Input shape from an expected input.</param>
            /// <param name="eps">A value added to the denominator for numerical stability. Default: 1e-5</param>
            /// <param name="elementwise_affine">a boolean value that when set to true, this module has learnable per-element affine parameters initialized to ones (for weights) and zeros (for biases).</param>
            /// <param name="device">The desired device of the parameters and buffers in this module</param>
            /// <param name="dtype">The desired floating point or complex dtype of the parameters and buffers in this module</param>
            /// <returns></returns>
            public static LayerNorm LayerNorm(long[] normalized_shape, double eps = 1e-05, bool elementwise_affine = true, Device? device = null, ScalarType? dtype = null)
            {
                unsafe {
                    fixed (long* pNormShape = normalized_shape) {
                        var handle = THSNN_LayerNorm_ctor((IntPtr)pNormShape, normalized_shape.Length, eps, elementwise_affine, out var boxedHandle);
                        if (handle == IntPtr.Zero) { torch.CheckForErrors(); }
                        return new LayerNorm(handle, boxedHandle).MoveModule<LayerNorm>(device, dtype);
                    }
                }
            }

            /// <summary>
            /// Applies Layer Normalization over a mini-batch of inputs as described in the paper Layer Normalization
            /// </summary>
            /// <param name="normalized_shape">Input shape from an expected input.</param>
            /// <param name="eps">A value added to the denominator for numerical stability. Default: 1e-5</param>
            /// <param name="elementwise_affine">a boolean value that when set to true, this module has learnable per-element affine parameters initialized to ones (for weights) and zeros (for biases).</param>
            /// <param name="device">The desired device of the parameters and buffers in this module</param>
            /// <param name="dtype">The desired floating point or complex dtype of the parameters and buffers in this module</param>
            /// <returns></returns>
            public static LayerNorm LayerNorm(long normalized_shape, double eps = 1e-05, bool elementwise_affine = true, Device? device = null, ScalarType? dtype = null)
            {
                return LayerNorm(new[] { normalized_shape }, eps, elementwise_affine, device, dtype);
            }
        }
    }
}
