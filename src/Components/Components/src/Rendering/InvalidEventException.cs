// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Components.Rendering
{
    /// <summary>
    /// Thrown when the renderer receives an invalid event ID that it can't dispatch.
    /// </summary>
    public class InvalidEventException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InvalidEventException"/>.
        /// </summary>
        /// <param name="eventId">The id of the invalid event.</param>
        public InvalidEventException(ulong eventId)
            : this(eventId, $"There is no event handler associated with this event.")
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidEventException"/>.
        /// </summary>
        /// <param name="eventId">The id of the invalid event.</param>
        /// <param name="message">The message explaining the reason for the error.</param>
        /// <param name="exception">The original exception that caused the issue.</param>
        public InvalidEventException(ulong eventId, string message, Exception exception) : base(message + $" EventId: {eventId}", exception)
        {
            EventId = eventId;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidEventException"/>.
        /// </summary>
        /// <param name="eventId">The id of the invalid event.</param>
        /// <param name="message">The message explaining the reason for the error.</param>
        public InvalidEventException(ulong eventId, string message) : base(message + $" EventId: {eventId}")
        {
            EventId = eventId;
        }

        /// <summary>
        /// The id of the event.
        /// </summary>
        public ulong EventId { get; set; }
    }
}
