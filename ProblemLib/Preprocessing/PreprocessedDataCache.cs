using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemLib.API;

namespace ProblemLib.Preprocessing
{
    /// <summary>
    /// Holds all cached data
    /// </summary>
    public class PreprocessedDataCache
    {
        private ICacheFactory factory;

        // cached distance-time entries; coordinates rounded to 5 decimals
        private readonly Dictionary<Pair<Coordinate, Coordinate>, Pair<Int32, Int32>> distanceTimeCache;
        // keeps track of additions to the cache
        private readonly Dictionary<Pair<Coordinate, Coordinate>, Pair<Int32, Int32>> distanceTimeDelta;

        // cached nearest location entries; coordinates rounded to 5 decimals
        private readonly Dictionary<Coordinate, Coordinate> nearestLocationCache;
        // keeps track of additions to the cache
        private readonly Dictionary<Coordinate, Coordinate> nearestLocationDelta;

        public PreprocessedDataCache(ICacheFactory factory)
        {

            if (factory == null)
                throw new ArgumentNullException("PreprocessedDataCache.ctor(): factory cannot be null!");
            this.factory = factory;

            distanceTimeCache = new Dictionary<Pair<Coordinate, Coordinate>, Pair<int, int>>();
            distanceTimeDelta = new Dictionary<Pair<Coordinate, Coordinate>, Pair<int, int>>();

            nearestLocationCache = new Dictionary<Coordinate, Coordinate>();
            nearestLocationDelta = new Dictionary<Coordinate, Coordinate>();
        }

        /// <summary>
        /// Provides access to underlying factory class
        /// </summary>
        public ICacheFactory Factory
        { get { return factory; } }

        /// <summary>
        /// Underlying dictionary for distance-time entries.
        /// For use in ICacheFactory only, do not use unless absolutely necessary!!!
        /// </summary>
        public Dictionary<Pair<Coordinate, Coordinate>, Pair<Int32, Int32>> CachedDistanceTime
        { get { return distanceTimeCache; } }
        /// <summary>
        /// Underlying dictionary for nearest node entries.
        /// For use in ICacheFactory only, do not use unless absolutely necessary!!!
        /// </summary>
        public Dictionary<Coordinate, Coordinate> CachedNearestNode
        { get { return nearestLocationCache; } }

        public Dictionary<Pair<Coordinate, Coordinate>, Pair<Int32, Int32>> CachedDistanceTimeDelta
        { get { return distanceTimeDelta; } }

        public Dictionary<Coordinate, Coordinate> CachedNearestNodeDelta
        { get { return nearestLocationDelta; } }

        #region Distance-Time Cache

        /// <summary>
        /// Gets a cached distance-time entry from cache.
        /// </summary>
        /// <param name="a">Coordinate A</param>
        /// <param name="b">Coordinate B</param>
        /// <returns>Distance-Time pair; null if operation fails</returns>
        public Pair<Int32, Int32> GetCachedDistanceTime(Coordinate a, Coordinate b)
        {
            // sort coordinates
            if (a.CompareTo(b) > 0)
            {
                Coordinate tmp = a;
                a = b;
                b = tmp;
            }

            // create pair
            Pair<Coordinate, Coordinate> key = new Pair<Coordinate, Coordinate>(a, b);

            // look for entry
            if (distanceTimeCache.ContainsKey(key))
                return distanceTimeCache[key];
            else
                return null;
        }
        /// <summary>
        /// Adds a new entry to cache, and marks it as unsynced so that next push operation
        /// will send it to the redis server.
        /// </summary>
        /// <param name="a">First coordinate</param>
        /// <param name="b">Second coordinate</param>
        /// <param name="dt">Distance-Time pair</param>
        public void AddCachedDistanceTimeEntry(Coordinate a, Coordinate b, Pair<Int32, Int32> dt)
        {
            // sort coordinates
            if (a.CompareTo(b) > 0)
            {
                Coordinate tmp = a;
                a = b;
                b = tmp;
            }

            // create pair
            Pair<Coordinate, Coordinate> key = new Pair<Coordinate, Coordinate>(a, b);

            AddCachedDistanceTimeEntry(key, dt);
        }
        /// <summary>
        /// Adds a new entry to cache, and marks it as unsynced so that next push operation
        /// will send it to the redis server.
        /// </summary>
        /// <param name="key">SORTED!! coordinate pair</param>
        /// <param name="dt">Date-Time </param>
        public void AddCachedDistanceTimeEntry(Pair<Coordinate, Coordinate> key, Pair<Int32, Int32> dt)
        {
            // check if the entry is already there
            if (distanceTimeCache.ContainsKey(key))
                throw new ProblemLib.ErrorHandling.ProblemLibException(
                    ErrorCodes.AttemptToAddRedundantCacheEntry,
                    new Exception(String.Format("Attempt to add a redundant cache entry! Key=({0}, {1});", key.First, key.Second)));

            // add entry
            distanceTimeCache.Add(key, dt);
            distanceTimeDelta.Add(key, dt);
        }
        /// <summary>
        /// Checks if the specified key exists within the cache
        /// </summary>
        /// <param name="a">Coordinate A</param>
        /// <param name="b">Coordinate B</param>
        /// <returns>Boolean indicating whether the key is found</returns>
        public Boolean IsIstanceTimeEntryCached(Coordinate a, Coordinate b)
        {
            // sort coordinates
            if (a.CompareTo(b) > 0)
            {
                Coordinate tmp = a;
                a = b;
                b = tmp;
            }

            // create pair
            Pair<Coordinate, Coordinate> key = new Pair<Coordinate, Coordinate>(a, b);

            return distanceTimeCache.ContainsKey(key);
        }
        /// <summary>
        /// Checks if the specified key exists within the cache
        /// </summary>
        /// <param name="key">SORTED!! pair of coordinates</param>
        /// <returns>Boolean indicating whether the key is found</returns>
        public Boolean IsIstanceTimeEntryCached(Pair<Coordinate, Coordinate> key)
        {
            return distanceTimeCache.ContainsKey(key);
        }

        #endregion

        #region Nearest-Node Cache

        /// <summary>
        /// Gets a cached nearest node entry from cache.
        /// </summary>
        /// <param name="a">Coordinate</param>
        /// <returns>Nearest node coordinate; null if operation fails</returns>
        public Coordinate GetCachedNearestNode(Coordinate a)
        {
            if (nearestLocationCache.ContainsKey(a))
                return nearestLocationCache[a];
            else
                return null;
        }
        /// <summary>
        /// Adds a new entry to cache, and marks it as unsynced so that next push operation
        /// will send it to the redis server.
        /// </summary>
        /// <param name="key">SORTED!! coordinate pair</param>
        /// <param name="value">Nearest Node</param>
        public void AddCachedNearestNodeEntry(Coordinate key, Coordinate value)
        {
            // check if entry already exists
            if (nearestLocationCache.ContainsKey(key))
               throw new ProblemLib.ErrorHandling.ProblemLibException(
                   ErrorCodes.AttemptToAddRedundantCacheEntry,
                   new Exception(String.Format("Attempt to add a redundant cache entry! Key=({0}, {1});", key.First, key.Second)));

            // add entry
            nearestLocationCache.Add(key, value);
            nearestLocationDelta.Add(key, value);
        }
        /// <summary>
        /// Checks if the specified key exists within the cache
        /// </summary>
        /// <param name="a">Coordinate</param>
        /// <returns>Boolean indicating whether the key is found</returns>
        public Boolean IsNearestNodeEntryCached(Coordinate key)
        {
            return nearestLocationCache.ContainsKey(key);
        }

        #endregion
    }
}
