using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;

namespace SalesAnalysis.RabbitMQ.Helpers
{
    public class PolicyHelper
    {
        public static RetryPolicy CreateRabbitMqPolicy<T>(ILogger<T> logger, int retryCount) where T : class =>
            Policy.Handle<SocketException>().Or<BrokerUnreachableException>().WaitAndRetry(retryCount
                , retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                , (ex, time) =>
                {
                    logger.LogWarning(ex, "RabbitMQ Client could not connect atver {TimeOut}s ({ExceptionMessage})"
                        , $"{time.TotalSeconds:n1}", ex.Message);
                });

        public static RetryPolicy CreateSqlPolicy<T>(ILogger<T> logger, int retryCount) where T: class =>
            Policy.Handle<SqlException>()
                .WaitAndRetry(3, retry => TimeSpan.FromSeconds(10)
                    , (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception
                            , "Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}"
                            , exception.GetType().Name, exception.Message, retry, 3);
                    });
    }
}