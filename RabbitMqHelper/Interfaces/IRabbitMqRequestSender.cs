/******************************************************************************
 *
 * File: IRabbitMqRequestSender.cs
 *
 * Description: IRabbitMqRequestSender.cs interface.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/


namespace RabbitMqHelper.Interfaces
{
    public interface IRabbitMqRequestSender
    {
        /// <summary>
        /// Sends the request and wait for response async.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A Task.</returns>
        Task<string> SendAndWaitAsync(object message);
    }
}