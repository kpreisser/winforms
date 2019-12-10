﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a collection of <see cref="TaskDialogStandardButton"/> objects.
    /// </summary>
    public class TaskDialogStandardButtonCollection : KeyedCollection<TaskDialogResult, TaskDialogStandardButton>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogStandardButtonCollection"/> class.
        /// </summary>
        public TaskDialogStandardButtonCollection()
        {
        }

        /// <summary>
        ///   Converts the specified <see cref="TaskDialogButtons"/> flags into a new
        ///   <see cref="TaskDialogStandardButtonCollection"/> containing an
        ///   <see cref="TaskDialogStandardButton"/> instance for each flag.
        /// </summary>
        /// <param name="buttons">The flags to convert.</param>
        public static implicit operator TaskDialogStandardButtonCollection(TaskDialogButtons buttons)
        {
            var collection = new TaskDialogStandardButtonCollection();

            // Get the button results for the flags.
            foreach (TaskDialogResult result in GetResultsForButtonFlags(buttons))
            {
                collection.Add(new TaskDialogStandardButton(result));
            }

            return collection;
        }

        // Note: This indexer is not needed for functionality; it only exists to avoid an overload
        // resolution behavior in the C# compiler which would otherwise use the TaskDialogResult
        // indexer from class KeyedCollection instead of the int indexer from class Collection
        // for an expression like "page.StandardButtons[0]" (using the int literal 0).
        // See: https://github.com/dotnet/winforms/pull/1133#issuecomment-557483834
        /// <summary>
        ///   Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is equal to or greater than <see cref="Count"/>.</exception>
        public new TaskDialogStandardButton this[int index]
        {
            get => base[index];
            set => base[index] = value;
        }

        internal TaskDialogPage? BoundPage
        {
            get;
            set;
        }

        internal static IEnumerable<TaskDialogResult> GetResultsForButtonFlags(TaskDialogButtons buttons)
        {
            // Note: The order in which we yield the results is the order in which
            // the task dialog actually displays the buttons.
            if ((buttons & TaskDialogButtons.OK) == TaskDialogButtons.OK)
                yield return TaskDialogResult.OK;
            if ((buttons & TaskDialogButtons.Yes) == TaskDialogButtons.Yes)
                yield return TaskDialogResult.Yes;
            if ((buttons & TaskDialogButtons.No) == TaskDialogButtons.No)
                yield return TaskDialogResult.No;
            if ((buttons & TaskDialogButtons.Abort) == TaskDialogButtons.Abort)
                yield return TaskDialogResult.Abort;
            if ((buttons & TaskDialogButtons.Retry) == TaskDialogButtons.Retry)
                yield return TaskDialogResult.Retry;
            if ((buttons & TaskDialogButtons.Cancel) == TaskDialogButtons.Cancel)
                yield return TaskDialogResult.Cancel;
            if ((buttons & TaskDialogButtons.Ignore) == TaskDialogButtons.Ignore)
                yield return TaskDialogResult.Ignore;
            if ((buttons & TaskDialogButtons.TryAgain) == TaskDialogButtons.TryAgain)
                yield return TaskDialogResult.TryAgain;
            if ((buttons & TaskDialogButtons.Continue) == TaskDialogButtons.Continue)
                yield return TaskDialogResult.Continue;
            if ((buttons & TaskDialogButtons.Close) == TaskDialogButtons.Close)
                yield return TaskDialogResult.Close;
            if ((buttons & TaskDialogButtons.Help) == TaskDialogButtons.Help)
                yield return TaskDialogResult.Help;
        }

        /// <summary>
        ///   Creates and adds a <see cref="TaskDialogStandardButton"/> to the collection.
        /// </summary>
        /// <param name="result">The <see cref="TaskDialogResult"/> that is represented by the
        /// <see cref="TaskDialogStandardButton"/>.</param>
        /// <returns>The created <see cref="TaskDialogStandardButton"/>.</returns>
        public TaskDialogStandardButton Add(TaskDialogResult result)
        {
            var button = new TaskDialogStandardButton(result);
            Add(button);

            return button;
        }

        internal void HandleKeyChange(TaskDialogStandardButton button, TaskDialogResult newKey) => 
            ChangeItemKey(button, newKey);

        /// <inheritdoc/>
        protected override TaskDialogResult GetKeyForItem(TaskDialogStandardButton item) => item.Result;

        /// <inheritdoc/>
        protected override void SetItem(int index, TaskDialogStandardButton item)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            TaskDialogStandardButton oldItem = this[index];

            // Call the base method first, as it will throw if we would insert a
            // duplicate item.
            base.SetItem(index, item);

            oldItem.Collection = null;
            item.Collection = this;
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, TaskDialogStandardButton item)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            // Call the base method first, as it will throw if we would insert a
            // duplicate item.
            base.InsertItem(index, item);

            item.Collection = this;
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();

            TaskDialogStandardButton oldItem = this[index];
            oldItem.Collection = null;
            base.RemoveItem(index);
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();

            foreach (TaskDialogStandardButton button in this)
            {
                button.Collection = null;
            }

            base.ClearItems();
        }

        private void DenyIfHasOtherCollection(TaskDialogStandardButton item)
        {
            if (item.Collection != null && item.Collection != this)
            {
                throw new InvalidOperationException(SR.TaskDialogControlIsPartOfOtherCollection);
            }
        }
    }
}
