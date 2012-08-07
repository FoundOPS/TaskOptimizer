using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Net;



namespace TaskOptimizer.API
{
    [DataContract]
    class OSMResponse
    {
        [DataMember(Name = "version")]
        public double Version { get; set; }
        [DataMember(Name = "status")]
        public int Status {get; set;}
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
        public OSMInstruction[] Route_Instructions;
        public AlternativeInstructions[] Alternative_Instructions;
        public Coordinate[] Via_Points;
    }
    [DataContract]
    class LocResponse
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
    class OSMInstruction{
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
    }
    [DataContract]
    class RouteSummary{
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
    class AlternativeInstructions
    {
        [DataMember]
        public OSMInstruction[] Instructions { get; set; }
    }
    [DataContract]
    public class Coordinate : IComparable, IEquatable<Coordinate>
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lon { get; set; }
        public Coordinate(double x, double y)
        {
            this.lat = x;
            this.lon = y;
        }
        public int CompareTo(object o)
        {
            if (!(o.GetType() == typeof(Coordinate)))
            {
                throw new Exception("Not a coordinate!");
            }
            else if (((Coordinate)o).lat != this.lat)
            {
                if (((Coordinate)o).lat > this.lat) return 1;
                return -1;
            }
            else if (((Coordinate)o).lon != this.lon)
            {
                if (((Coordinate)o).lon > this.lon) return 1;
                else return -1;
            }
            return 0;
        }
        public override String ToString()
        {
            return this.lat + "," + this.lon;
        }
        public bool Equals(Coordinate other)
        {
            return (this.lat==other.lat && this.lon==other.lon);
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
