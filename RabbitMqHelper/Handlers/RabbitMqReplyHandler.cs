/******************************************************************************
 *
 * File: RabbitMqReplyHandler.cs
 *
 * Description: RabbitMqReplyHandler.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
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
    public class RabbitMqReplyHandler : BaseRabbitMqHandler , IRabbitMqReplyHandler
    {
        #region Private Fields

        private IDictionary<string, Func<JArray, Task<string>>> _functions;
        private readonly string _queueName = "data_queue";

        #endregion Private Fields

        #region Public Constructors

        public RabbitMqReplyHandler(
            IOptions<RabbitMQSettings> rabbitMQSettings,
            ILogger<RabbitMqReplyHandler> logger,
            IRabbitMqReplyHandlerFiller filler):base(rabbitMQSettings, logger)
        {
            ArgumentNullException.ThrowIfNull(filler);
            _functions = filler.GetFunctions();
            _queueName = rabbitMQSettings.Value.QueueName;

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        #endregion Public Constructors

        #region Public Methods



        /// <summary>
        /// Start receive and reply.
        /// </summary>
        public void Start_ReceiveAndReply()
        {           
            EventHandler<BasicDeliverEventArgs> responseHandler = null;
            responseHandler = GetReceivedHandler("", async (message, correlationId, replyTo) =>
            {
                var response = await HandleMessage(message);

                Reply(response, correlationId, replyTo);
            });

            _consumer.Received += responseHandler;

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: _consumer);
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
        /// <param name="response">The response.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="replyTo">The reply to.</param>
        private void Reply(string response, string correlationId, string replyTo)
        {
            var replyProperties = _channel.CreateBasicProperties();
            replyProperties.CorrelationId = correlationId;

            var responseBytes = Encoding.UTF8.GetBytes(response);
            _channel.BasicPublish(exchange: "", routingKey: replyTo, basicProperties: replyProperties, body: responseBytes);
        }

        #endregion Private Methods
    }
}
