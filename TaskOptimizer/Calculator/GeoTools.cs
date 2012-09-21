using System;

namespace TaskOptimizer.Calculator
{
    public static class GeoTools
    {
        #region Othodromic Distance from https://github.com/lmaslanka/Orthodromic-Distance-Calculator

        /// <summary>
        /// Calculate the straight distance between two points
        /// </summary>
        public static int StraightDistance(double aLatRad, double aLonRad, double bLatRad, double bLonRad)
        {
            return (int)VincentyDistanceFormula(aLatRad, aLonRad, bLatRad, bLonRad);
        }

        /// <summary>
        /// Calculate the arc length distance between two locations using the Vincenty formula
        /// </summary>
        /// <returns>
        /// The distance in meters
        /// </returns>
        private static double VincentyDistanceFormula(double aLatRad, double aLonRad, double bLatRad, double bLonRad)
        {
            return Math.Atan(Math.Sqrt(
                ((Math.Pow(Math.Cos(bLatRad) * Math.Sin(Diff(aLonRad, bLonRad)), 2)) +
                (Math.Pow((Math.Cos(aLatRad) * Math.Sin(bLatRad)) - (Math.Sin(aLatRad) * Math.Cos(bLatRad) * Math.Cos(Diff(aLonRad, bLonRad))), 2)))
                /
                ((Math.Sin(aLatRad) * Math.Sin(bLatRad)) +
                (Math.Cos(aLatRad) * Math.Cos(bLatRad) * Math.Cos(Diff(aLonRad, bLonRad))))));
        }

        private static double Diff(double a, double b)
        {
            return Math.Abs(b - a);
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        #endregion
    }
}
