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
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MovieLibraryWorkerService
{
    public class Worker : BackgroundService
    {
        #region Private Fields

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IDataSource _dataSource;
        private readonly ILogger<Worker> _logger;
        private readonly string _queueName = "data_queue";

        #endregion Private Fields

        #region Public Constructors

        public Worker(IDataSource dataSource, IConfiguration configuration, ILogger<Worker> logger)
        {
            _logger = logger;
            _dataSource = dataSource;
            _queueName = configuration.GetSection("RabbitMQ:QueueName").Value;

            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetSection("RabbitMQ:HostName").Value,
                Port = int.Parse(configuration.GetSection("RabbitMQ:Port").Value)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        #endregion Public Constructors


        #region Public Methods

        /// <summary>
        /// Disposes the.
        /// </summary>
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns>A Task.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var response = HandleMessage(message);

                    var replyProperties = _channel.CreateBasicProperties();
                    replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProperties, body: responseBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(), ex, ex.Message);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string.</returns>
        private string HandleMessage(string message)
        {
            try
            {
                dynamic requestData = JsonConvert.DeserializeObject(message);

                string methodName = requestData.MethodName;
                Newtonsoft.Json.Linq.JArray parameters = requestData.Parameters;

                switch (methodName)
                {
                    case "GetCategoriesAsync":
                        var categories = _dataSource.GetCategoriesAsync().Result;
                        return JsonConvert.SerializeObject(categories);

                    case "GetMovieListAsync":
                        var selectAndOrder = parameters.Count > 0 ? parameters[0].ToObject<SelectAndOrder>() : null;
                        var movieList = _dataSource.GetMovieListAsync(selectAndOrder).Result;
                        return JsonConvert.SerializeObject(movieList);

                    case "GetMovieDtoByIdAsync":
                        int movieId = parameters[0].ToObject<int>();
                        var movieDto = _dataSource.GetMovieDtoByIdAsync(movieId).Result;
                        return JsonConvert.SerializeObject(movieDto);

                    default:
                        return "Method not found";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        #endregion Private Methods
    }
}
