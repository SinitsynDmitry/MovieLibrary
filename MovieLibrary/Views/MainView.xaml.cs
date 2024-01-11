/******************************************************************************
 *
 * File: MainView.xaml.cs
 *
 * Description: MainView.xaml.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Microsoft.Extensions.DependencyInjection;
using MovieLibrary.ViewModels;
using System.ComponentModel;
using System.Data.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MovieLibrary.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<MainViewModel>();
            this.ContentRendered += MainView_ContentRendered;
        }

        #region Private

        /// <summary>
        /// Mains the view_ content rendered.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void MainView_ContentRendered(object? sender, System.EventArgs e)
        {
            var style = (Style)Application.Current.Resources["AccentButtonStyle"];

            btn_cleanFilters.Style = style;
        }

        #endregion Private
    }
}