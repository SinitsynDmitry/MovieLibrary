/******************************************************************************
 *
 * File: MovieService.cs
 *
 * Description: MovieService.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Microsoft.Extensions.Configuration;
using MovieData.Dtos;
using MovieData.Entitys;
using MovieData.Helpers;
using MovieData.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary.Data
{
    public class MovieService : IDataSource,IDisposable
    {
        #region Private Fields

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _replyQueueName;
        private readonly string _routingKey;

        #endregion Private Fields

        #region Public Constructors

        public MovieService(IConfiguration configuration)
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
        /// Gets the categories async.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task<List<Category>> GetCategoriesAsync()
        {
            var correlationId = Guid.NewGuid().ToString();
            var message = new { MethodName = "GetCategoriesAsync", Parameters = new object[] { } };

            var response = SendRequestAndWaitForResponse(message, correlationId);

            return Task.FromResult(JsonConvert.DeserializeObject<List<Category>>(response));
        }


        /// <summary>
        /// Gets the movie dto by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A Task.</returns>
        public Task<MovieDto> GetMovieDtoByIdAsync(int id)
        {
            var correlationId = Guid.NewGuid().ToString();
            var message = new { MethodName = "GetMovieDtoByIdAsync", Parameters = new object[] { id } };

            var response = SendRequestAndWaitForResponse(message, correlationId);

            return Task.FromResult(JsonConvert.DeserializeObject<MovieDto>(response));
        }

        /// <summary>
        /// Gets the movie list async.
        /// </summary>
        /// <param name="selectAndOrder">The select and order.</param>
        /// <returns>A Task.</returns>
        public Task<List<MovieLightDto>> GetMovieListAsync(SelectAndOrder? selectAndOrder = null)
        {
            var correlationId = Guid.NewGuid().ToString();
            var message = new { MethodName = "GetMovieListAsync", Parameters = new object[] { selectAndOrder } };

            var response = SendRequestAndWaitForResponse(message, correlationId);

            return Task.FromResult(JsonConvert.DeserializeObject<List<MovieLightDto>>(response));
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Sends the request and wait for response.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <returns>A string.</returns>
        private string SendRequestAndWaitForResponse(object message, string correlationId)
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

        #endregion Private Methods
    }
}
