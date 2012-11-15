using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ProblemLib.Utilities;
using ServiceStack.Redis;
using ProblemLib.ErrorHandling;
using ProblemLib.API;

namespace ProblemLib.Preprocessing
{
    /// <summary>
    /// Creates PreprocessedDataCache object from a remote Redis source.
    /// </summary>
    public class RedisDataCacheFactory : ICacheFactory
    {
        const String DISTANCE_TIME_POSTFIX = ":dt";
        const String NEAREST_NODE_POSTFIX = ":nl";

        IRedisClient redisServer;
        String cacheId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="redisServerHost">Address of the redis server host</param>
        /// <param name="port">Port of the Redis service</param>
        /// <param name="cacheId">ID of the cache page (?)</param>
        public RedisDataCacheFactory(String redisServerHost, Int32 port, Guid cacheId)
        {
            redisServer = new RedisClient(redisServerHost, port);
            this.cacheId = cacheId.ToString();
        }
        
        /// <summary>
        /// Creates a new cache object and pulls cache data from Redis server.
        /// </summary>
        /// <returns></returns>
        public PreprocessedDataCache CreateCache()
        {
            PreprocessedDataCache cache = new PreprocessedDataCache(this);
            PullFromStore(cache, CacheType.NearestNode);
            PullFromStore(cache, CacheType.DistanceTime);
            return cache;
        }

        /// <summary>
        /// Pushes changes to Redis server.
        /// Before calling this method, seriously consider pulling changes first.
        /// </summary>
        /// <param name="cache">PreprocessedDataCache object</param>
        /// <param name="type">Type of cache</param>
        public void PushToStore(PreprocessedDataCache cache, CacheType type)
        {
            switch (type)
            {
                case CacheType.DistanceTime: // push distance-time entries to redis
                    {
                        String setId = cacheId + DISTANCE_TIME_POSTFIX; // set id

                        try
                        {
                            foreach (var kvp in cache.CachedDistanceTimeDelta)
                            {
                                String encoded = RedisNumberEncoder.EncodeDistanceTime(kvp.Key.First, kvp.Key.Second, kvp.Value.First, kvp.Value.Second);
                                redisServer.AddItemToSet(setId, encoded);
                            }

                            redisServer.Save();     // write redis data to disk
                            cache.CachedDistanceTimeDelta.Clear();  // clear unsynced data
                        }
                        catch (Exception ex)
                        {   // oops.. an error..
                            throw new ProblemLibException(ErrorCodes.RedisConnectionFailed, ex);
                        }
                    }
                    break;
                case CacheType.NearestNode:
                    {
                        String setId = cacheId + NEAREST_NODE_POSTFIX; // set id

                        try
                        {
                            foreach (var kvp in cache.CachedNearestNodeDelta)
                            {
                                String encoded = RedisNumberEncoder.EncodeCoordinatePair(kvp.Key, kvp.Value);
                                redisServer.AddItemToSet(setId, encoded);
                            }

                            redisServer.Save();     // write redis data to disk
                            cache.CachedNearestNodeDelta.Clear();  // clear unsynced data
                        }
                        catch (Exception ex)
                        {   // oops.. an error..
                            throw new ProblemLibException(ErrorCodes.RedisConnectionFailed, ex);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Pulls changes from Redis server.
        /// </summary>
        /// <param name="cache">PreprocessedDataCache object</param>
        /// <param name="type">Type of cache</param>
        public void PullFromStore(PreprocessedDataCache cache, CacheType type)
        {
            switch (type)
            {
                case CacheType.DistanceTime:
                    {
                        String setId = cacheId + DISTANCE_TIME_POSTFIX;

                        HashSet<String> encodedEntries = redisServer.GetAllItemsFromSet(setId);
                        foreach (String s in encodedEntries)
                        {
                            Coordinate c0, c1;
                            Int32 d, t;
                            RedisNumberEncoder.DecodeDistanceTime(s, out c0, out c1, out d, out t);

                            Pair<Coordinate, Coordinate> key = new Pair<Coordinate, Coordinate>(c0, c1);
                            cache.CachedDistanceTime[key] = new Pair<int, int>(d, t);
                        }
                    }
                    break;
                case CacheType.NearestNode:
                    {
                        String setId = cacheId + NEAREST_NODE_POSTFIX;

                        HashSet<String> encodedEntries = redisServer.GetAllItemsFromSet(setId);
                        foreach (String s in encodedEntries)
                        {
                            Coordinate c0, c1;
                            RedisNumberEncoder.DecodeCoordinatePair(s, out c0, out c1);

                            cache.CachedNearestNode[c0] = c1;
                        }
                    }
                    break;
            }
        }
    }
}
