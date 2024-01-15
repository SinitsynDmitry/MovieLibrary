/******************************************************************************
 *
 * File: IRabbitMqReplyHandler.cs
 *
 * Description: IRabbitMqReplyHandler.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 15.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace RabbitMqHelper.Interfaces
{
    public interface IRabbitMqReplyHandler
    {
        /// <summary>
        /// Disposes the.
        /// </summary>
        void Dispose();
        /// <summary>
        /// Starts the.
        /// </summary>
        void Start();
    }
}