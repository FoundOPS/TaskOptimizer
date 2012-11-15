using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemLib.Preprocessing
{
    /// <summary>
    /// Represents a partition of preprocessing data.
    /// </summary>
    /// <typeparam name="T">Type of preprocessing pairs</typeparam>
    public class PreprocessingPartition<T> : IEnumerable<Pair<T, T>>, IEnumerator<Pair<T, T>>
    {
        
        T[] arr;            // One dimension of the preprocessing "grid"
        Int32 startX;       // Start X position of current partition
        Int32 startY;       // Start Y position of current partition
        Int32 endX;         // Ending X position of current partition
        Int32 endY;         // Ending Y position of current partition
        Int32 length;       // length of the partition (in # of elements)

        // variables used for iteration
        Int32 y;            // Current X position of the iterator
        Int32 x;            // Current Y position of the iterator
        Int32 cIndex;       // Current Linear Index

        /// <summary>
        /// Constructor.
        /// Creates a new partition that contains the entire preprocessing queue.
        /// </summary>
        /// <param name="arr"></param>
        public PreprocessingPartition(T[] arr)
            : this(arr, 0) { }

        /// <summary>
        /// Constructor.
        /// Creates a new partition that contains all the elements starting from a specified index.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        public PreprocessingPartition(T[] arr, Int64 start)
            : this(arr, start, (int)(RowSum(arr.Length, arr.Length) - start)) { }

        /// <summary>
        /// Constructor.
        /// Creates a new partition starting from a specified index and spanning specified number of elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        public PreprocessingPartition(T[] arr, Int64 start, Int32 len)
        {
            this.arr = arr;

            // Verify
            Int64 rsum = RowSum(arr.Length, arr.Length);
            if (start < 0 || start >= rsum)
                throw new ArgumentOutOfRangeException("Start position cannot be negative or greater than/equal row sum!");
            if (len < 0 || start + len > rsum)
                throw new ArgumentOutOfRangeException("Length cannot be negative or exceed row sum!");

            // Calculate starting position
            BoxIndex(start, out startX, out startY);

            // Calculate ending position
            BoxIndex(start + len - 1, out endX, out endY);

            length = len;

            // Set Start
            Reset();
        }

        #region Utilities

        /// <summary>
        /// Gets the index of the same element if the array were reversed.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Int32 ReverseIndex(Int32 index)
        {
            return arr.Length - index - 1;
        }
        /// <summary>
        /// Calculates the sum of elements in n rows
        /// </summary>
        /// <param name="width"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static Int64 RowSum(Int32 width, Int32 rowCount)
        {
            return (Int64)((width * 2 - rowCount - 1) * rowCount / 2);
        }
        /// <summary>
        /// Maps a linear index to x and y positions in the preprocessing frid
        /// </summary>
        /// <param name="linearIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void BoxIndex(Int64 linearIndex, out Int32 x, out Int32 y)
        {
            Int32 ty = 0;
            if (linearIndex < RowSum(arr.Length, 1))
            {
                y = 0;
                x = (Int32)linearIndex;
                return;
            }

            while (RowSum(arr.Length, ty) < linearIndex) ty++;

            Int32 rsum = (Int32)RowSum(arr.Length, ty - 1);
            x = (Int32)(linearIndex - rsum);
            y = ty;
        }

        public Int32 Size
        { get { return length; } }
        public Int32 CurrentIndex
        { get { return cIndex; } }

        #endregion

        #region IEnumerable Members

        public IEnumerator<Pair<T, T>> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator Members

        public Pair<T, T> Current
        {
            get
            {
                T lhs = arr[y];
                T rhs = arr[ReverseIndex(x)];
                return new Pair<T, T>(lhs, rhs);
            }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            // advance line counter
            x++;
            cIndex++;

            // calculate row change
            Int32 rowLen = ReverseIndex(y);
            if (x >= rowLen)
            { x = 0; y++; }

            // check if new value is valid and return
            return y <= endY && (y == endY ? x <= endX : true);
        }

        public void Reset()
        {
            x = startX - 1;
            y = startY;
        }

        #endregion

    }
}