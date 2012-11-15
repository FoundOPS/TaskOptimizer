using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemLib.Preprocessing
{
    /// <summary>
    /// Reporesents various types of cache
    /// </summary>
    public enum CacheType
    {
        NearestNode,
        DistanceTime
    }

    /// <summary>
    /// Factory for creating cache data.
    /// Should also contain all the interfacing with persistent store.
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// Creates a data cache object.
        /// </summary>
        /// <returns></returns>
        PreprocessedDataCache CreateCache();

        /// <summary>
        /// Pushes all additions to the persistent store.
        /// </summary>
        void PushToStore(PreprocessedDataCache cache, CacheType type);
        /// <summary>
        /// Pulls all changes from persistent store.
        /// </summary>
        void PullFromStore(PreprocessedDataCache cache, CacheType type);
    }
}
