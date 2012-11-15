using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemLib.API;

namespace ProblemLib.Utilities
{
    /// <summary>
    /// Encodes/decodes data stored in Redis server
    /// </summary>
    static class RedisNumberEncoder
    {
        /// <summary>
        /// Number of decimals to leave after decoding Single/Double numbers
        /// </summary>
        const Int32 ROUND_DECIMALS = 5;

        #region Utility Methods

        /// <summary>
        /// Encodes a 32-bit integer and copies the result into a byte array
        /// </summary>
        /// <param name="v">32-bit Integer to encode</param>
        /// <param name="arr">Destination array</param>
        /// <param name="start">Position where to start copying</param>
        private static void EncodeInt32(Int32 v, Byte[] arr, Int32 start)
        {
            Byte[] tmp = BitConverter.GetBytes(v);
            Array.Copy(tmp, 0, arr, start, 4);
        }
        /// <summary>
        /// Encodes a 32-bit floating point number and copies the result into a byte array
        /// </summary>
        /// <param name="v">32-bit Single to encode</param>
        /// <param name="arr">Destination array</param>
        /// <param name="start">Position where to start copying</param>
        private static void EncodeSingle(Single v, Byte[] arr, Int32 start)
        {
            Byte[] tmp = BitConverter.GetBytes(v);
            Array.Copy(tmp, 0, arr, start, 4);
        }

        #endregion

        #region Encode/Decode

        /// <summary>
        /// Encodes a coordinate-distance-time entry into a compact string representation
        /// that can be stored in Redis. This method does not sort coordinates!
        /// </summary>
        /// <param name="c0">First Location</param>
        /// <param name="c1">Second Location</param>
        /// <param name="dist">Distance between location</param>
        /// <param name="time">Time between locations</param>
        /// <returns></returns>
        public static String EncodeDistanceTime(Coordinate c0, Coordinate c1, Int32 dist, Int32 time)
        {
            Byte[] payload = new Byte[24];

            EncodeSingle((Single)c0.lon, payload, 0);
            EncodeSingle((Single)c0.lat, payload, 4);
            EncodeSingle((Single)c1.lon, payload, 8);
            EncodeSingle((Single)c1.lat, payload, 12);

            EncodeInt32(dist, payload, 16);
            EncodeInt32(time, payload, 20);

            return Convert.ToBase64String(payload);
        }
        /// <summary>
        /// Decodes coordinate-distance-time entry from its compact representation
        /// </summary>
        /// <param name="data">Encoded data</param>
        /// <param name="c0">First Location</param>
        /// <param name="c1">Second Location</param>
        /// <param name="distance">Distance between locations</param>
        /// <param name="time">Time between locations</param>
        public static void DecodeDistanceTime(String data, out Coordinate c0, out Coordinate c1, out Int32 distance, out Int32 time)
        {
            try
            {
                Byte[] payload = Convert.FromBase64String(data);

                Single c0lon = BitConverter.ToSingle(payload, 0);
                Single c0lat = BitConverter.ToSingle(payload, 4);
                Single c1lon = BitConverter.ToSingle(payload, 8);
                Single c1lat = BitConverter.ToSingle(payload, 12);
                distance = BitConverter.ToInt32(payload, 16);
                time = BitConverter.ToInt32(payload, 20);
                c0 = new Coordinate(Math.Round(c0lat, ROUND_DECIMALS), Math.Round(c0lon, ROUND_DECIMALS));
                c1 = new Coordinate(Math.Round(c1lat, ROUND_DECIMALS), Math.Round(c1lon, ROUND_DECIMALS));
            }
            catch (FormatException ex)
            {
                throw new ProblemLib.ErrorHandling.ProblemLibException(ErrorCodes.RedisCacheCorrupted, ex);
            }
        }
        /// <summary>
        /// Encodes a coordinate pair entry into a compact string representation
        /// that can be stored in Redis. This method does not sort coordinates!
        /// </summary>
        /// <param name="c0">First Location</param>
        /// <param name="c1">Second Location</param>
        /// <returns></returns>
        public static String EncodeCoordinatePair(Coordinate c0, Coordinate c1)
        {
            Byte[] payload = new Byte[16];

            EncodeSingle((Single)c0.lon, payload, 0);
            EncodeSingle((Single)c0.lat, payload, 4);
            EncodeSingle((Single)c1.lon, payload, 8);
            EncodeSingle((Single)c1.lat, payload, 12);
            
            return Convert.ToBase64String(payload);
        }
        /// <summary>
        /// Decodes coordinate pair entry from its compact representation
        /// </summary>
        /// <param name="data">Encoded data</param>
        /// <param name="c0">First Location</param>
        /// <param name="c1">Second Location</param>
        public static void DecodeCoordinatePair(String data, out Coordinate c0, out Coordinate c1)
        {
            try
            {
                Byte[] payload = Convert.FromBase64String(data);

                Single c0lon = BitConverter.ToSingle(payload, 0);
                Single c0lat = BitConverter.ToSingle(payload, 4);
                Single c1lon = BitConverter.ToSingle(payload, 8);
                Single c1lat = BitConverter.ToSingle(payload, 12);
                c0 = new Coordinate(Math.Round(c0lat, ROUND_DECIMALS), Math.Round(c0lon, ROUND_DECIMALS));
                c1 = new Coordinate(Math.Round(c1lat, ROUND_DECIMALS), Math.Round(c1lon, ROUND_DECIMALS));
            }
            catch (FormatException ex)
            {
                throw new ProblemLib.ErrorHandling.ProblemLibException(ErrorCodes.RedisCacheCorrupted, ex);
            }
        }

        #endregion
    }
}
