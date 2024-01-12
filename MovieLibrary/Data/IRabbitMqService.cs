/******************************************************************************
 *
 * File: IRabbitMqService.cs
 *
 * Description: IRabbitMqService.cs interface
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace MovieLibrary.Data
{
    public interface IRabbitMqService
    {
        string SendRequestAndWaitForResponse(object message, string correlationId);
    }
}