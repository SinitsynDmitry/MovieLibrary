/******************************************************************************
 *
 * File: IRabbitMqReplyHandlerFiller.cs
 *
 * Description: IRabbitMqReplyHandlerFiller.cs interface.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 17.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Newtonsoft.Json.Linq;

namespace RabbitMqHelper.Interfaces
{
    public interface IRabbitMqReplyHandlerFiller
    {
        /// <summary>
        /// Gets the functions.
        /// </summary>
        /// <returns>An IDictionary.</returns>
        IDictionary<string, Func<JArray, Task<string>>> GetFunctions();
    }
}