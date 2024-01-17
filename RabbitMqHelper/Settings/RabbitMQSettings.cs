/******************************************************************************
 *
 * File: RabbitMQSettings.cs
 *
 * Description: RabbitMQSettings.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace RabbitMqHelper.Settings
{
    public record RabbitMQSettings
    {
        public string HostName { get; init; }
        public int Port { get; init; }
        public string QueueName { get; init; }
    }
}
