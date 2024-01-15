/******************************************************************************
 *
 * File: RabbitMqRequestSender.cs
 *
 * Description: RabbitMqRequestSender.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 15.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/


using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqHelper.Interfaces;
using RabbitMqHelper.Settings;
using System.Text;

namespace RabbitMqHelper.Handlers
{
    public class RabbitMqRequestSender: IRabbitMqRequestSender,IDisposable
    {
        #region Private Fields

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _replyQueueName;
        private readonly string _routingKey;

        #endregion Private Fields

        #region Public Constructors

        public RabbitMqRequestSender(IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            _routingKey = rabbitMQSettings.Value.QueueName;

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                Port = rabbitMQSettings.Value.Port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);
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
        /// Sends the request and wait for response async.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <returns>A Task.</returns>
        public async Task<string> SendRequestAndWaitForResponseAsync(object message, string correlationId)
        {
            EventHandler<BasicDeliverEventArgs> responseHandler = null;

            try
            {
                var props = _channel.CreateBasicProperties();
                props.CorrelationId = correlationId;
                props.ReplyTo = _replyQueueName;

                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                _channel.BasicPublish(exchange: "", routingKey: _routingKey, basicProperties: props, body: messageBytes);

                var tcs = new TaskCompletionSource<string>();

                responseHandler = (model, ea) =>
                {
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        var responseBytes = ea.Body.ToArray();
                        var response = Encoding.UTF8.GetString(responseBytes);
                        tcs.SetResult(response);
                    }
                };

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
