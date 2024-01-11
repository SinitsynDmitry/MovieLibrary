/******************************************************************************
 *
 * File: MultipleSelectionListBox.cs
 *
 * Description: MultipleSelectionListBox.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MovieLibrary.Controls
{
    public class MultipleSelectionListBox : ListBox
    {
        #region Public Fields

        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.Register("BindableSelectedItems",
                typeof(object), typeof(MultipleSelectionListBox),
                new FrameworkPropertyMetadata(default(ICollection<object>),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBindableSelectedItemsChanged));

        #endregion Public Fields

        #region Internal Fields

        internal bool processSelectionChanges = false;

        #endregion Internal Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the bindable selected items.
        /// </summary>
        public dynamic BindableSelectedItems
        {
            get => GetValue(BindableSelectedItemsProperty);
            set => SetValue(BindableSelectedItemsProperty, value);
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Ons the selection changed.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (BindableSelectedItems == null || !this.IsInitialized) return; //Handle pre initilized calls

            if (e.AddedItems.Count > 0)
                if (!string.IsNullOrWhiteSpace(SelectedValuePath))
                {
                    foreach (var item in e.AddedItems)
                        if (!BindableSelectedItems.Contains((dynamic)item.GetType().GetProperty(SelectedValuePath).GetValue(item, null)))
                            BindableSelectedItems.Add((dynamic)item.GetType().GetProperty(SelectedValuePath).GetValue(item, null));
                }
                else
                {
                    foreach (var item in e.AddedItems)
                        if (!BindableSelectedItems.Contains((dynamic)item))
                            BindableSelectedItems.Add((dynamic)item);
                }

            if (e.RemovedItems.Count > 0)
                if (!string.IsNullOrWhiteSpace(SelectedValuePath))
                {
                    foreach (var item in e.RemovedItems)
                        if (BindableSelectedItems.Contains((dynamic)item.GetType().GetProperty(SelectedValuePath).GetValue(item, null)))
                            BindableSelectedItems.Remove((dynamic)item.GetType().GetProperty(SelectedValuePath).GetValue(item, null));
                }
                else
                {
                    foreach (var item in e.RemovedItems)
                        if (BindableSelectedItems.Contains((dynamic)item))
                            BindableSelectedItems.Remove((dynamic)item);
                }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Ons the bindable selected items changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultipleSelectionListBox listBox)
            {
                List<dynamic> newSelection = new List<dynamic>();
                if (!string.IsNullOrWhiteSpace(listBox.SelectedValuePath))
                    foreach (var item in listBox.BindableSelectedItems)
                    {
                        foreach (var lbItem in listBox.Items)
                        {
                            var lbItemValue = lbItem.GetType().GetProperty(listBox.SelectedValuePath).GetValue(lbItem, null);
                            if ((dynamic)lbItemValue == (dynamic)item)
                                newSelection.Add(lbItem);
                        }
                    }
                else
                    newSelection = listBox.BindableSelectedItems as List<dynamic>;

                listBox.SetSelectedItems(newSelection);
            }
        }

        #endregion Private Methods
    }
}
