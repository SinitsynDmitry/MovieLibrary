/******************************************************************************
 *
 * File: IRabbitMqReplyHandler.cs
 *
 * Description: IRabbitMqReplyHandler.cs interface.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
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
        void Start_ReceiveAndReply();
    }
}