/******************************************************************************
 *
 * File: IRabbitMqReplyHandlerFiller.cs
 *
 * Description: IRabbitMqReplyHandlerFiller.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 15.1.2024	 Authors:  Dmitry Sinitsyn
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