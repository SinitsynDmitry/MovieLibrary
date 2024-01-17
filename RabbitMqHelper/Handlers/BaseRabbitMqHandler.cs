/******************************************************************************
 *
 * File: BaseRabbitMqHandler.cs
 *
 * Description: BaseRabbitMqHandler.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqHelper.Settings;
using System.Text;

namespace RabbitMqHelper.Handlers
{
    public class BaseRabbitMqHandler: IDisposable
    {
        protected readonly IModel _channel;
        protected readonly IConnection _connection;
        protected readonly EventingBasicConsumer _consumer;
        protected readonly ILogger<BaseRabbitMqHandler> _logger;

        public BaseRabbitMqHandler(IOptions<RabbitMQSettings> rabbitMQSettings, ILogger<BaseRabbitMqHandler> logger = null)
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                Port = rabbitMQSettings.Value.Port
            };
            _logger = logger;
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);
        }

        /// <summary>
        /// Disposes the.
        /// </summary>
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }


        protected EventHandler<BasicDeliverEventArgs> GetReceivedHandler(string receivedCorrelationId, Action<string, string, string>? callBack = null)
        {
            return async (model, ea) =>
            {
                try
                {
                    var isCatched = true;
                    if (!string.IsNullOrEmpty(receivedCorrelationId))
                    {
                        isCatched = ea.BasicProperties.CorrelationId == receivedCorrelationId;
                    }
                    if (isCatched)
                    {
                        var responseBytes = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(responseBytes);

                        var correlationId = ea.BasicProperties.CorrelationId;
                        var replyTo = ea.BasicProperties.ReplyTo;

                        if (callBack != null)
                        {
                            callBack(message, correlationId, replyTo);
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger?.LogError(new EventId(), ex, ex.Message);
                }
            };
        }

    }
}
