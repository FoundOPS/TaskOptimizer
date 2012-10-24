using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace ProblemLib.DataModel
{
    public class DistributionConfiguration
    {
        public Guid ProblemID { get; set; }

        public IPAddress ControllerServer { get; set; }
        public UInt16 ControllerServerPort { get; set; }
        public IPAddress RedisServer { get; set; }
        public UInt16 RedisServerPort { get; set; }
        public IPAddress OsrmServer { get; set; }
        public UInt16 OsrmServerPort { get; set; }

        public Int32 RandomSeed { get; set; }
        public Task[] Tasks { get; set; }
        public Worker[] Workers { get; set; }

        public void WriteToStream(BinaryWriter s)
        {
            // Write problem id
            Byte[] problemid = ProblemID.ToByteArray();
            s.Write(problemid);

            // Serialize IP Addresses
            Byte[] cb = ControllerServer.GetAddressBytes();
            Byte[] rb = RedisServer.GetAddressBytes();
            Byte[] ob = RedisServer.GetAddressBytes();

            // Write address information
            Byte addrtype = 0;
            if (cb.Length == 16) addrtype |= 1;
            if (rb.Length == 16) addrtype |= 2;
            if (ob.Length == 16) addrtype |= 4;
            s.Write(addrtype);

            // Write controller ip information (4/16 + 2)
            s.Write(cb);
            s.Write(ControllerServerPort);

            // Write redis server ip information (4/16 + 2)
            s.Write(rb);
            s.Write(RedisServerPort);

            // Write osrm server ip information (4/16 + 2)
            s.Write(ob);
            s.Write(OsrmServerPort);
            
            // Write random seed (4)
            s.Write(RandomSeed);

            // Write Workers
            s.Write(Workers.Length);
            for (int i = 0; i < Workers.Length; i++)
                Workers[i].WriteToStream(s);

            // Write Clusters
            s.Write(Tasks.Length);
            for (int i = 0; i < Tasks.Length; i++)
            { Tasks[i].WriteToStream(s); }
        }

        public static DistributionConfiguration ReadFromStream(TcpClient client, BinaryReader br)
        {
            DistributionConfiguration cfg = new DistributionConfiguration();

            while (client.Available < 17) ; // wait for data
            Byte[] problemid = br.ReadBytes(16);
            cfg.ProblemID = new Guid(problemid);

            Byte addrtype = br.ReadByte();

            // Read Controller Server Information
            Int32 cblen = 6 + ((addrtype & 1) == 0 ? 0 : 12);
            while (client.Available < cblen) ; // Wait for data
            Byte[] cb = br.ReadBytes(cblen - 2);
            cfg.ControllerServer = new IPAddress(cb);
            cfg.ControllerServerPort = br.ReadUInt16();
            
            // Read Redis Server Information
            Int32 rblen = 6 + ((addrtype & 2) == 0 ? 0 : 12);
            while (client.Available < rblen) ; // wait for data
            Byte[] rb = br.ReadBytes(rblen - 2);
            cfg.RedisServer = new IPAddress(rb);
            cfg.RedisServerPort = br.ReadUInt16();

            // Read OSRM Server Information
            Int32 oblen = 6 + ((addrtype & 4) == 0 ? 0 : 12);
            while (client.Available < oblen) ; // wait for data
            Byte[] ob = br.ReadBytes(oblen - 2);
            cfg.OsrmServer = new IPAddress(ob);
            cfg.OsrmServerPort = br.ReadUInt16();

            while (client.Available < 8) ; // wait for data

            // Read Random Seed
            cfg.RandomSeed = br.ReadInt32();

            // Read Workers
            Int32 wcount = br.ReadInt32(); 
            while (client.Available < Worker.SerializedLength * wcount) ; // wait for data
            cfg.Workers = new Worker[wcount];
            for (int i = 0; i < wcount; i++)
                cfg.Workers[i] = Worker.ReadFromStream(br);

            // Read Clusters
            while (client.Available < 4) ; // wait for data
            Int32 tcount = br.ReadInt32();
            cfg.Tasks = new Task[tcount];
            for (int i = 0; i < tcount; i++)
            {
                while (client.Available < Task.SerializedLength) ; // wait for data
                cfg.Tasks[i] = Task.ReadFromStream(br);
            }

            return cfg;
        }
    }
}
