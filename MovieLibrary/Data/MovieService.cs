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
    public class MovieService : IDataSource
    {
        #region Private Fields

        private readonly IRabbitMqService _rabbitMqService;

        #endregion Private Fields

        #region Public Constructors

        public MovieService(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService=rabbitMqService;
        }

        #endregion Public Constructors

        #region Public Methods



        /// <summary>
        /// Gets the categories async.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task<List<Category>> GetCategoriesAsync()
        {
            var correlationId = Guid.NewGuid().ToString();
            var message = new { MethodName = "GetCategoriesAsync", Parameters = new object[] { } };

            var response = _rabbitMqService.SendRequestAndWaitForResponse(message, correlationId);

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

            var response = _rabbitMqService.SendRequestAndWaitForResponse(message, correlationId);

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

            var response = _rabbitMqService.SendRequestAndWaitForResponse(message, correlationId);

            return Task.FromResult(JsonConvert.DeserializeObject<List<MovieLightDto>>(response));
        }

        #endregion Public Methods

    }
}
