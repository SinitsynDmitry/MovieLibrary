/******************************************************************************
 *
 * File: Program.cs
 *
 * Description: Program.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using MovieData.Interfaces;
using MovieLibraryWorkerService;
using MovieLibraryWorkerService.Data;
using RabbitMqHelper.Handlers;
using RabbitMqHelper.Interfaces;
using RabbitMqHelper.Settings;


internal class Program
{
    /// <summary>
    /// Mains the.
    /// </summary>
    /// <param name="args">The args.</param>
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "Movie Library Service";
        });
        builder.Services.AddHostedService<Worker>();
        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));

        builder.Services.AddSingleton<IDataSource, MovieRepository>();
        builder.Services.AddSingleton<IRabbitMqReplyHandlerFiller, RabbitMqReplyHandlerFiller>();
        builder.Services.AddSingleton<IRabbitMqReplyHandler, RabbitMqReplyHandler>();


       
        var host = builder.Build();
        host.Run();
    }
    
}