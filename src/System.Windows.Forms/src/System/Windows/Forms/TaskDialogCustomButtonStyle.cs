﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace System.Windows.Forms
{
    /// <summary>
    /// Contains constants that defines how <see cref="TaskDialogCustomButton"/>s are to be
    /// displayed in a task dialog.
    /// </summary>
    public enum TaskDialogCustomButtonStyle
    {
        /// <summary>
        /// Custom buttons should be displayed as normal buttons.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Custom buttons should be displayed as command links.
        /// </summary>
        CommandLinks = 1,

        /// <summary>
        /// Custom buttons should be displayed as command links, but without an icon.
        /// </summary>
        CommandLinksNoIcon = 2
    }
}
