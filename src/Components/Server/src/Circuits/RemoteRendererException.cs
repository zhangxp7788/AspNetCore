// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Components.Web.Rendering
{
    internal class RemoteRendererException : Exception
    {
        public RemoteRendererException(string message) : base(message)
        {
        }

        public RemoteRendererException(bool badInput, string message) : this (message)
        {
            BadInput = badInput;
        }

        public bool BadInput { get; }
    }
}
