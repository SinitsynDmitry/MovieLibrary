/******************************************************************************
 *
 * File: RabbitMqReplyHandler.cs
 *
 * Description: RabbitMqReplyHandler.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 15.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqHelper.Interfaces;
using RabbitMqHelper.Settings;
using System.Text;

namespace RabbitMqHelper.Handlers
{
    public class RabbitMqReplyHandler : IDisposable, IRabbitMqReplyHandler
    {
        #region Private Fields

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IDictionary<string, Func<JArray, Task<string>>> _functions;
        private readonly ILogger<RabbitMqReplyHandler> _logger;
        private readonly string _queueName = "data_queue";

        #endregion Private Fields

        #region Public Constructors

        public RabbitMqReplyHandler(
            IOptions<RabbitMQSettings> rabbitMQSettings,
            IRabbitMqReplyHandlerFiller filler,
            ILogger<RabbitMqReplyHandler> logger)
        {
            _logger = logger;
            _queueName = rabbitMQSettings.Value.QueueName;

            ArgumentNullException.ThrowIfNull(filler);
            _functions = filler.GetFunctions();

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                Port = rabbitMQSettings.Value.Port
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
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        /// <summary>
        /// Starts the.
        /// </summary>
        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var response = await HandleMessage(message);

                    var correlationId = ea.BasicProperties.CorrelationId;
                    var replyTo = ea.BasicProperties.ReplyTo;

                    Reply(correlationId, replyTo, response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(), ex, ex.Message);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A Task.</returns>
        private async Task<string> HandleMessage(string message)
        {
            try
            {
                dynamic requestData = JsonConvert.DeserializeObject(message);

                string methodName = requestData.MethodName;
                JArray parameters = requestData.Parameters;

                if (!_functions.ContainsKey(methodName))
                {
                    return "Method not found";
                }

                return await _functions[methodName](parameters);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Replies the.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <param name="response">The response.</param>
        private void Reply(string correlationId, string replyTo, string response)
        {
            var replyProperties = _channel.CreateBasicProperties();
            replyProperties.CorrelationId = correlationId;

            var responseBytes = Encoding.UTF8.GetBytes(response);
            _channel.BasicPublish(exchange: "", routingKey: replyTo, basicProperties: replyProperties, body: responseBytes);
        }

        #endregion Private Methods
    }
}
