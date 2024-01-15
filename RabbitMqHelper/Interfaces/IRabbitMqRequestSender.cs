/******************************************************************************
 *
 * File: IRabbitMqRequestSender.cs
 *
 * Description: IRabbitMqRequestSender.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 15.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using System.Threading.Tasks;

namespace RabbitMqHelper.Interfaces
{
    public interface IRabbitMqRequestSender
    {
        /// <summary>
        /// Sends the request and wait for response async.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <returns>A Task.</returns>
        Task<string> SendRequestAndWaitForResponseAsync(object message, string correlationId);
    }
}