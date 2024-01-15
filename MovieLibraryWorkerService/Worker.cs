/******************************************************************************
 *
 * File: Worker.cs
 *
 * Description: Worker.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using MovieData.Helpers;
using MovieData.Interfaces;
using MovieLibraryWorkerService.Data;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqHelper.Interfaces;
using System.Text;

namespace MovieLibraryWorkerService
{
    public class Worker : BackgroundService
    {
        #region Private Fields

        private readonly ILogger<Worker> _logger;

        private readonly IRabbitMqReplyHandler _rabbitHandler;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IRabbitMqReplyHandler rabbitHandler,  ILogger<Worker> logger)
        {
            _logger = logger;
            _rabbitHandler = rabbitHandler;        
        }

        #endregion Public Constructors


        #region Public Methods

        /// <summary>
        /// Disposes the.
        /// </summary>
        public override void Dispose()
        {
            _rabbitHandler.Dispose();
            base.Dispose();
        }

        #endregion Public Methods

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns>A Task.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitHandler.Start();

            return Task.CompletedTask;
        }

    }
}
