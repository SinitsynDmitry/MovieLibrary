/******************************************************************************
 *
 * File: RabbitMqRequestSender.cs
 *
 * Description: RabbitMqRequestSender.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using RabbitMqHelper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqHelper.Settings;
using System.Text;

namespace RabbitMqHelper.Handlers
{
    public class RabbitMqRequestSender: BaseRabbitMqHandler, IRabbitMqRequestSender
    {
        #region Private Fields

        private readonly string _replyQueueName;
        private readonly string _routingKey;

        #endregion Private Fields

        #region Public Constructors

        public RabbitMqRequestSender(IOptions<RabbitMQSettings> rabbitMQSettings, ILogger<RabbitMqRequestSender> logger=null) : base(rabbitMQSettings, logger)
        {
            _routingKey = rabbitMQSettings.Value.QueueName;

            _replyQueueName = _channel.QueueDeclare().QueueName;

            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Sends the request and wait for response async.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A Task.</returns>
        public async Task<string> SendAndWaitAsync(object message)
        {
            EventHandler<BasicDeliverEventArgs> responseHandler = null;
            var correlationId = Guid.NewGuid().ToString();
            try
            {
                var props = _channel.CreateBasicProperties();
                props.CorrelationId = correlationId;
                props.ReplyTo = _replyQueueName;

                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                _channel.BasicPublish(exchange: "", routingKey: _routingKey, basicProperties: props, body: messageBytes);

                var tcs = new TaskCompletionSource<string>();

                responseHandler = GetReceivedHandler(correlationId, (response, correlationId, replyTo) => { tcs.SetResult(response); });

                _consumer.Received += responseHandler;

                var delayTask = Task.Delay(1000);

                // Use Task.WhenAny to wait for either the response or the timeout
                var completedTask = await Task.WhenAny(tcs.Task, delayTask);

                if (completedTask == tcs.Task)
                {
                    // The response arrived within the timeout
                    return await tcs.Task;
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            finally
            {
                // Unregister the event handler after processing the response
                if (_consumer != null && responseHandler!=null)
                {
                    _consumer.Received -= responseHandler;
                }
            }
        }


        #endregion Public Methods
    }
}
