﻿using System;
using System.Threading.Tasks;

namespace EasyNetQ
{
    /// <summary>
    /// Provides a simple Publish API for schedule a message on the bus.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Schedule a message to be published at some time in the future.
        /// This required the EasyNetQ.Scheduler service to be running.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="futurePublishDate">The time at which the message should be sent (UTC)</param>
        /// <param name="message">The message to response with</param>
        void FuturePublish<T>(DateTime futurePublishDate, T message) where T : class;

        /// <summary>
        /// Schedule a message to be published at some time in the future.
        /// This required the EasyNetQ.Scheduler service to be running.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="futurePublishDate">The time at which the message should be sent (UTC)</param>
        /// <param name="cancellationKey">An identifier that can be used with CancelFuturePublish to cancel the sending of this message at a later time</param>
        /// <param name="message">The message to response with</param>
        void FuturePublish<T>(DateTime futurePublishDate, string cancellationKey, T message) where T : class;

        /// <summary>
        /// Unschedule all messages matching the cancellationKey.
        /// </summary>
        /// <param name="cancellationKey">The identifier that was used when originally scheduling the message with FuturePublish</param>
        void CancelFuturePublish(string cancellationKey);

        /// <summary>
        /// Schedule a message to be published at some time in the future.
        /// This required the EasyNetQ.Scheduler service to be running.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="futurePublishDate">The time at which the message should be sent (UTC)</param>
        /// <param name="message">The message to response with</param>
        Task FuturePublishAsync<T>(DateTime futurePublishDate, T message) where T : class;

        /// <summary>
        /// Schedule a message to be published at some time in the future.
        /// This required the EasyNetQ.Scheduler service to be running.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="futurePublishDate">The time at which the message should be sent (UTC)</param>
        /// <param name="cancellationKey">An identifier that can be used with CancelFuturePublish to cancel the sending of this message at a later time</param>
        /// <param name="message">The message to response with</param>
        Task FuturePublishAsync<T>(DateTime futurePublishDate, string cancellationKey, T message) where T : class;

        /// <summary>
        /// Unschedule all messages matching the cancellationKey.
        /// </summary>
        /// <param name="cancellationKey">The identifier that was used when originally scheduling the message with FuturePublish</param>
        Task CancelFuturePublishAsync(string cancellationKey);
    }
}
