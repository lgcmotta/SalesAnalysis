using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;

namespace SalesAnalysis.RabbitMQ
{
    public class PolicyHelper
    {
        public static RetryPolicy CreatePolicy<T>(ILogger<T> logger, int retryCount) where T : class
        {
            return Policy.Handle<SocketException>().Or<BrokerUnreachableException>().WaitAndRetry(retryCount
                , retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                , (ex, time) =>
                {
                    logger.LogWarning(ex, "RabbitMQ Client could not connect atver {TimeOut}s ({ExceptionMessage})"
                        , $"{time.TotalSeconds:n1}", ex.Message);
                });
        }
    }
}