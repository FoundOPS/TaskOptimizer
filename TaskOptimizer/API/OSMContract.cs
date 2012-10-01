using System;
using System.Runtime.Serialization;
using TaskOptimizer.Calculator;

namespace TaskOptimizer.API
{
    [DataContract]
    public class OSMResponse
    {
        public AlternativeInstructions[] Alternative_Instructions;
        public OSMInstruction[] Route_Instructions;
        public Coordinate[] Via_Points;

        [DataMember(Name = "version")]
        public double Version { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "status_message")]
        public String Status_Message { get; set; }

        [DataMember(Name = "route_geometry")]
        public String Route_Geometry { get; set; }

        [DataMember(Name = "route_instructions")]
        public Object[][] Raw_Route { get; set; }

        [DataMember(Name = "route_summary")]
        public RouteSummary Route_Summary { get; set; }

        [DataMember(Name = "alternative_geometries")]
        public String[] Alternative_Geometries { get; set; }

        [DataMember(Name = "alternative_instructions")]
        public Object[][][] Raw_Alternatives { get; set; }

        [DataMember(Name = "alternative_summaries")]
        public RouteSummary[] Alternative_Summaries { get; set; }

        [DataMember(Name = "via_points")]
        public Object[][] Raw_Via { get; set; }

        [DataMember(Name = "hint_data")]
        public HintData Hint_Data { get; set; }

        [DataMember(Name = "transactionId")]
        public String Transaction_Id { get; set; }
    }

    [DataContract]
    internal class LocResponse
    {
        [DataMember(Name = "version")]
        public double Version { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "mapped_coordinate")]
        public double[] Mapped_Coordinate { get; set; }

        [DataMember(Name = "tansactionId")]
        public String Transaction_Id { get; set; }
    }

    [DataContract]
    public class OSMInstruction
    {
        public OSMInstruction(Object[] objs)
        {
            Instruction_Type = objs[0] as String;
            Road_Name = objs[1] as String;
            Instruction_Duration = (int)objs[2];
            Position = (int)objs[3];
            Time_Elapsed = (int)objs[4];
            Distance_Elapsed = objs[5] as String;
            Direction = objs[6] as String;
            Azimuth = Convert.ToDouble(objs[7]);
        }

        [DataMember]
        public String Instruction_Type { get; set; }

        [DataMember]
        public String Road_Name { get; set; }

        [DataMember]
        public int Instruction_Duration { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public int Time_Elapsed { get; set; }

        [DataMember]
        public String Distance_Elapsed { get; set; }

        [DataMember]
        public String Direction { get; set; }

        [DataMember]
        public double Azimuth { get; set; }
    }

    [DataContract]
    public class RouteSummary
    {
        [DataMember(Name = "total_distance")]
        public int Total_Distance { get; set; }

        [DataMember(Name = "total_time")]
        public int Total_Time { get; set; }

        [DataMember(Name = "start_point")]
        public String Start_Point { get; set; }

        [DataMember(Name = "end_point")]
        public String End_Point { get; set; }
    }

    [DataContract]
    public class AlternativeInstructions
    {
        [DataMember]
        public OSMInstruction[] Instructions { get; set; }
    }

    [DataContract]
    public class Coordinate : IComparable, IComparable<Coordinate>, IEquatable<Coordinate>
    {
        public Coordinate(double x, double y)
        {
            lat = x;
            lon = y;
        }

        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }

        public double latRad { get { return GeoTools.DegreeToRadian(lat); } }
        public double lonRad { get { return GeoTools.DegreeToRadian(lon); } }

        #region IComparable/IEquatable<Coordinate> Members

        public int CompareTo(object o)
        {
            var other = o as Coordinate;
            if (other == null)
                throw new Exception("Not a coordinate!");

            var latDifference = lat - other.lat;
            var lonDifference = lon - other.lon;

            if (Math.Abs(latDifference) > 0.0000001 || Math.Abs(lonDifference) > 0.0000001)
            {
                if (latDifference > 0 || lonDifference > 0)
                    return 1;

                return -1;
            }

            return 0;
        }

        public int CompareTo(Coordinate other)
        {
            return CompareTo((object)other);
        }

        public bool Equals(Coordinate other)
        {
            return CompareTo(other) == 0;
        }

        #endregion

        public override String ToString()
        {
            return lat + "," + lon;
        }
    }

    [DataContract]
    public class HintData
    {
        [DataMember(Name = "checksum")]
        public int Checksum { get; set; }

        [DataMember(Name = "locations")]
        public String[] Locations { get; set; }
    }
}