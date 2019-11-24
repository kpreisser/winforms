﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a button control of a task dialog that has a custom text.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   A custom button can either be displayed as regular button or as command link,
    ///   depending on the value of <see cref="TaskDialogPage.CustomButtonStyle"/>.
    /// </para>
    /// </remarks>
    public sealed class TaskDialogCustomButton : TaskDialogButton
    {
        private string? _text;
        private string? _descriptionText;
        private int _buttonID;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCustomButton"/> class.
        /// </summary>
        public TaskDialogCustomButton()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogCustomButton"/> class
        ///   using the given text and, optionally, a description text.
        /// </summary>
        /// <param name="text">The text of the control.</param>
        /// <param name="descriptionText">An additional description text that will be displayed in
        /// a separate line when the <see cref="TaskDialogCustomButton"/>s of the task dialog are
        /// shown as command links (see <see cref="DescriptionText"/>).</param>
        public TaskDialogCustomButton(string? text, string? descriptionText = null)
            : this()
        {
            _text = text;
            _descriptionText = descriptionText;
        }

        /// <summary>
        ///   Gets or sets the text associated with this control.
        /// </summary>
        /// <value>
        ///   The text associated with this control. The default value is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property must not be <see langword="null"/> or an empty string when showing or navigating
        ///   the dialog; otherwise, the operation will fail.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">This control is currently bound to a task dialog.</exception>
        public string? Text
        {
            get => _text;

            set
            {
                DenyIfBound();

                _text = value;
            }
        }

        /// <summary>
        ///   Gets or sets an additional description text that will be displayed in a separate
        ///   line when the <see cref="TaskDialogCustomButton"/>s of the task dialog are shown
        ///   as command links (when
        /// <see cref="TaskDialogPage.CustomButtonStyle"/> is set to
        /// <see cref="TaskDialogCustomButtonStyle.CommandLinks"/> or
        /// <see cref="TaskDialogCustomButtonStyle.CommandLinksNoIcon"/>).
        /// </summary>
        public string? DescriptionText
        {
            get => _descriptionText;

            set
            {
                DenyIfBound();

                _descriptionText = value;
            }
        }

        internal override bool IsCreatable => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);

        internal override int ButtonID => _buttonID;

        internal new TaskDialogCustomButtonCollection? Collection
        {
            get => (TaskDialogCustomButtonCollection?)base.Collection;
            set => base.Collection = value;
        }

        private protected override bool IsShownAsCommandLink
        {
            get => BoundPage?.CustomButtonStyle == TaskDialogCustomButtonStyle.CommandLinks ||
                BoundPage?.CustomButtonStyle == TaskDialogCustomButtonStyle.CommandLinksNoIcon;
        }

        /// <summary>
        ///   Returns a string that represents the current <see cref="TaskDialogCustomButton"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString() => _text ?? base.ToString() ?? string.Empty;

        internal ComCtl32.TDF Bind(TaskDialogPage page, int buttonID)
        {
            ComCtl32.TDF result = Bind(page);
            _buttonID = buttonID;

            return result;
        }

        internal string? GetResultingText()
        {
            Debug.Assert(BoundPage != null);

            // Remove LFs from the text. Otherwise, the dialog would display the
            // part of the text after the LF in the command link note, but for
            // this we have the "DescriptionText" property, so we should ensure that
            // there is not an discrepancy here and that the contents of the "Text"
            // property are not displayed in the command link note.
            // Therefore, we replace a combined CR+LF with CR, and then also single
            // LFs with CR, because CR is treated as a line break.
            string? text = _text?.Replace("\r\n", "\r").Replace("\n", "\r");

            TaskDialogCustomButtonStyle customButtonStyle = BoundPage.CustomButtonStyle;
            if ((customButtonStyle == TaskDialogCustomButtonStyle.CommandLinks ||
                customButtonStyle == TaskDialogCustomButtonStyle.CommandLinksNoIcon) &&
                text != null && _descriptionText != null)
            {
                text += '\n' + _descriptionText;
            }

            return text;
        }

        private protected override void UnbindCore()
        {
            _buttonID = 0;

            base.UnbindCore();
        }
    }
}
