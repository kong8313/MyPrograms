using System;
using System.Threading;
using ConfirmitDialerInterface;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage
{
    public class RedisClient
    {
        private readonly ILogger _logger;
        private IConnectionMultiplexer _connection;
        private readonly string _redisConnString;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1,1);
        private const string KeyPrefix = "AwsConnectDialer_";
        
        public RedisClient(string redisConnString, ILogger logger)
        {
            _redisConnString = redisConnString;
            _logger = logger;
        }
        
        private IConnectionMultiplexer GetConnection()
        {
            if (_connection != null)
                return _connection;
            
            _connectionLock.Wait();

            try
            {
                if (_connection != null)
                    return _connection;

                _logger.Info(nameof(RedisClient),"Establish redis connection");
                _connection = ConnectionMultiplexer.Connect(_redisConnString);
                _connection.ConnectionFailed += RedisConnectionFailed;
                _connection.ConnectionRestored += RedisConnectionRestored;

                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }
        
        private IDatabase Database => GetConnection().GetDatabase();

        private void RedisConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.Info(nameof(RedisClient),"Redis connection restored.");
        }

        private void RedisConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.Info(nameof(RedisClient),"Redis connection failed.");
        }

        public T Get<T>(string cacheKey) where T : IStorageModel
        {
            var value = Database.StringGet(KeyPrefix + cacheKey);
            return JsonConvert.DeserializeObject<T>(value);
        }
        
        public bool Set<T>(string cacheKey, T value, TimeSpan? expiry = null) where T : IStorageModel
        {
            var stringValue = JsonConvert.SerializeObject(value);
            return Database.StringSet(KeyPrefix + cacheKey, stringValue, expiry);
        }

        public bool Remove(string cacheKey)
        {
            return Database.KeyDelete(KeyPrefix + cacheKey);
        }
    }
}