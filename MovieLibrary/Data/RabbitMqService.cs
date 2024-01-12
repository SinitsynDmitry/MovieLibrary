using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary.Data
{
    public class RabbitMqService: IRabbitMqService,IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _replyQueueName;
        private readonly string _routingKey;

        public RabbitMqService(IConfiguration configuration)
        {
            _routingKey = configuration.GetSection("RabbitMQ:QueueName").Value;

            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetSection("RabbitMQ:HostName").Value,
                Port = int.Parse(configuration.GetSection("RabbitMQ:Port").Value)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);
        }

        /// <summary>
        /// Disposes the.
        /// </summary>
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        /// <summary>
        /// Sends the request and wait for response.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <returns>A string.</returns>
        public string SendRequestAndWaitForResponse(object message, string correlationId)
        {
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.BasicPublish(exchange: "", routingKey: _routingKey, basicProperties: props, body: messageBytes);

            var tcs = new TaskCompletionSource<string>();

            _consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var responseBytes = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(responseBytes);
                    tcs.SetResult(response);
                }
            };

            var delayTask = Task.Delay(1000);

            // Use Task.WhenAny to wait for either the response or the timeout
            var completedTask = Task.WhenAny(tcs.Task, delayTask).Result;

            if (completedTask == tcs.Task)
            {
                // The response arrived within the timeout
                return tcs.Task.Result;
            }
            else
            {
                throw new TimeoutException();
            }
        }

    }
}
