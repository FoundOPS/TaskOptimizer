using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemLib
{
    /// <summary>
    /// Represents a pair of objects.
    /// </summary>
    /// <typeparam name="TFirst">Type of the first object in pair</typeparam>
    /// <typeparam name="TSecond">Type of the second object in pair</typeparam>
    public class Pair<TFirst, TSecond> : ICloneable, IEquatable<Pair<TFirst, TSecond>>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="f">First object in the pair</param>
        /// <param name="s">Second object in the pair</param>
        public Pair(TFirst f, TSecond s)
        {
            this.First = f;
            this.Second = s;
        }
        /// <summary>
        /// First object in the pair
        /// </summary>
        public TFirst First { get; set; }
        /// <summary>
        /// Second object in the pair
        /// </summary>
        public TSecond Second { get; set; }

        /// <summary>
        /// Clones the pair.
        /// </summary>
        /// <returns>A shallow copy of the pair object</returns>
        public object Clone()
        {
            return new Pair<TFirst, TSecond>(this.First, this.Second);
        }
        /// <summary>
        /// Checks whether two pairs are the same
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Pair<TFirst, TSecond> other)
        {
            return this.First.Equals(other.First) && this.Second.Equals(other.Second);
        }
    }
}
