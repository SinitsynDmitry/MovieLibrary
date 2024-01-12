/******************************************************************************
 *
 * File: App.xaml.cs
 *
 * Description: App.xaml.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieData.Interfaces;
using MovieLibrary.Data;
using MovieLibrary.ViewModels;
using MovieLibrary.Views;
using System;
using System.Windows;

namespace MovieLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        /// <summary>
        /// Application Entry for MovieLibrary
        /// </summary>
        public App()
        {
            Services = ConfigureServices();

            var view = new MainView();
            view.Show();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            Configuration = new ConfigurationBuilder()
           .SetBasePath(System.IO.Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();

            services.AddSingleton<IConfiguration>(re =>
            {
                return Configuration;
            });

            services.AddSingleton<IDataSource, MovieService>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();

            // Viewmodels
            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }
    }
}