using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nightingale;
using ProblemLib;
using ProblemLib.API;
using ProblemLib.DataModel;
using ProblemLib.Preprocessing;

namespace TaskOptimizerDevKit
{
/* This part of the main form is used for prototyping, developing
 * and testing preprocessing functions of DistributionServer.
 */

    public partial class MainForm : Form
    {
        WizardStateManager pprocStateMan = new WizardStateManager();
        PingTimer pprocRedisPing;
        PingTimer pprocOsrmPing;
        DistributionConfiguration config;

        private void InitializePreprocessing()
        {
            pprocStateMan.AddState(wsPprocConfig);
            pprocStateMan.AddState(wsPprocReview);
            pprocStateMan.AddState(wsPprocConnect);
            pprocStateMan.SetCurrentState("wsPprocConfig");

            btnPpCfgGenId_Click(null, null);

            Random r = new Random();
            txtPpCfgRnd.Text = ((UInt32)r.Next()).ToString();

            pprocOsrmPing = new PingTimer(this, (int i) =>
                {
                    if (i >= 0)
                    {
                        lblPpCfgOPingResult.ForeColor = Color.DarkGreen;
                        lblPpCfgOPingResult.Text = String.Format("Delay: {0}ms", i);
                    }
					else
                    {
                        lblPpCfgOPingResult.ForeColor = Color.DarkRed;
                        lblPpCfgOPingResult.Text = "Failed!";
                    }
                });
            pprocRedisPing = new PingTimer(this, (int i) =>
            {
                if (i >= 0)
                {
                    lblPpCfgRPingResult.ForeColor = Color.DarkGreen;
                    lblPpCfgRPingResult.Text = String.Format("Delay: {0}ms", i);
                }
                else
                {
                    lblPpCfgRPingResult.ForeColor = Color.DarkRed;
                    lblPpCfgRPingResult.Text = "Failed!";
                }
            });
        }

        private void btnPpCfgGenId_Click(object sender, EventArgs e)
        {
            Guid id = Guid.NewGuid();
            txtPpCfgId.Text = id.ToString();
        }

        private void btnPpCfgRedisPing_Click(object sender, EventArgs e)
        {
                erp.Clear();
            if (!Utils.IsValidIp(txtPpCfgRedisAddr.Text))
            {
                erp.SetError(txtPpCfgRedisAddr, "Value not a valid IP!");
                pprocRedisPing.Address = null;
                return;
            }

            pprocRedisPing.Address = Utils.ToIpAddress(txtPpCfgRedisAddr.Text);
        }

        private void btnPpCfgOsrmPing_Click(object sender, EventArgs e)
        {
                erp.Clear();
            if (!Utils.IsValidIp(txtPpCfgOsrmAddr.Text))
            {
                erp.SetError(txtPpCfgOsrmAddr, "Value not a valid IP!");
                pprocOsrmPing.Address = null;
                return;
            }

            pprocOsrmPing.Address = Utils.ToIpAddress(txtPpCfgOsrmAddr.Text);
        }

        private void btnPpCgfNext_Click(object sender, EventArgs e)
        {
            // Construct config object
            config = new DistributionConfiguration();

            // tree view 
            TreeNode rootNode = new TreeNode("DictributionConfiguration (#" + config.GetHashCode().ToString() + ")");

            // misc
            config.ControllerServer = IPAddress.Parse("127.0.0.1");
            config.ControllerServerPort = 0;
            config.OsrmServerPort = 5000;
            config.RedisServerPort = 6379;

            // guid
            try { config.ProblemID = Guid.Parse(txtPpCfgId.Text); }
            catch { erp.SetError(txtPpCfgId, "Cannot parse GUID"); return; }
            rootNode.Nodes.Add(String.Format("ProblemID: {0}", config.ProblemID));


            rootNode.Nodes.Add(String.Format("ControllerServer: {0}:{1}", config.ControllerServer, config.ControllerServerPort));

            // redis ip
            try { config.RedisServer = IPAddress.Parse(txtPpCfgRedisAddr.Text); }
            catch { erp.SetError(txtPpCfgRedisAddr, "Invalid IP Address"); return; }
            rootNode.Nodes.Add(String.Format("RedisServer: {0}:{1}", config.RedisServer, config.RedisServerPort));

            // osrm ip
            try { config.OsrmServer = IPAddress.Parse(txtPpCfgOsrmAddr.Text); }
            catch { erp.SetError(txtPpCfgOsrmAddr, "Invalid IP Address"); return; }
            rootNode.Nodes.Add(String.Format("OsrmServer: {0}:{1}", config.OsrmServer, config.OsrmServerPort));

            // random seed
            try { config.RandomSeed = Int32.Parse(txtPpCfgRnd.Text); }
            catch { erp.SetError(txtPpCfgRnd, "Cannot parse number"); return; }
            rootNode.Nodes.Add(String.Format("RandomSeed: {0}", config.RandomSeed));

            // generate workers
            TreeNode workersNode = new TreeNode("Workers");
            List<Worker> workers = new List<Worker>();
            for (int i = 0; i < nudPpCfgWorkerC.Value; i++)
            {
                Worker w = new Worker() { WorkerID = (uint)i };
                workers.Add(w);
                workersNode.Nodes.Add(String.Format("Worker {{ID: {0}}}", w.WorkerID));
            }
            config.Workers = workers.ToArray();
            rootNode.Nodes.Add(workersNode);

            // generate tasks
            TreeNode tasksNode = new TreeNode("Tasks");
            List<ProblemLib.DataModel.Task> tasks = new List<ProblemLib.DataModel.Task>();
            for (int i = 0; i < nudPpCfgTaskC.Value; i++)
            {
                Coordinate c = locations[i];
                ProblemLib.DataModel.Task t = new ProblemLib.DataModel.Task((uint)(100000 + i), (float)c.lon, (float)c.lat);
                tasks.Add(t);
                tasksNode.Nodes.Add(String.Format("Task {{ID: {0}; Location: ({1}, {2})}}", t.TaskID, t.Latitude, t.Longitude));
            }
            config.Tasks = tasks.ToArray();
            rootNode.Nodes.Add(tasksNode);

            // partition stuff
            Int32 dtPprocC = (int)nudPpCfgPprocC.Value;
            Int64 totalLen = ProblemLib.Preprocessing.PreprocessingPartition<int>.RowSum(config.Tasks.Length, config.Tasks.Length);
            Int32 distribution = (int)(totalLen / dtPprocC);

            TreeNode distributionNode = new TreeNode("Preprocessing Distribution");
            for (int i = 0; i < dtPprocC - 1; i++)
            {
                distributionNode.Nodes.Add(String.Format("Distribution {{Start: {0}; Length: {1}}}", i * distribution, distribution));
            }
            distributionNode.Nodes.Add(String.Format("Distribution {{Start: {0}; Length: {1}}}", (dtPprocC - 1) * distribution, totalLen - (dtPprocC - 1) * distribution));
            rootNode.Nodes.Add(distributionNode);

            treeView1.Nodes.Add(rootNode);

            pprocStateMan.SetCurrentState("wsPprocReview");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            erp.Clear();
            if (!Utils.IsValidIp(textBox1.Text))
            {
                erp.SetError(textBox1, "Value not a valid IP!");
                return;
            }

            int clientCount = (int)nudPpCfgPprocC.Value;
            Int32 distribution = (int)(config.Tasks.Length / clientCount);

            Thread[] threads = new Thread[clientCount];
            Pair<Int32, Int32>[] distributions = new Pair<int, int>[clientCount];
            for (int i = 0; i < clientCount - 1; i++)
                distributions[i] = new Pair<Int32, Int32>(distribution * i, distribution);
            distributions[clientCount - 1] = new Pair<int, int>(distribution * (clientCount - 1), config.Tasks.Length - (distribution * (clientCount - 1)));

            for (int i = 0; i < clientCount; i++)
            {
                ParameterizedThreadStart ts = new ParameterizedThreadStart((object o) =>
                    {
                        Int32 index = (int)o;

                        WriteMessage(index, "Connecting to {0}...\n", textBox1.Text);
                        using (TcpClient client = new TcpClient(textBox1.Text, 17924))
                        {
                            client.ReceiveTimeout = Int32.MaxValue;

                            WriteMessage(index, "Opening network streams...\n");
                            // create reader/writer
                            BinaryWriter bw = new BinaryWriter(client.GetStream());
                            BinaryReader br = new BinaryReader(client.GetStream());

                            WriteMessage(index, "Sending config data to distribution...\n");
                            // send config we just created
                            bw.Write(ControlCodes.SendingConfiguration);
                            config.WriteToStream(bw);

                            WriteMessage(index, "Waiting for ACK signal...\n");
                            while (client.Available < 2) ;
                            UInt32 code = br.ReadUInt16();
                            if (code == ControlCodes.Acknowledge)
                                WriteMessage(index, "Gotcha!\n");
                            else if (code == ControlCodes.Error)
                            {
                                while (client.Available < 4) ;
                                UInt32 errc = br.ReadUInt32();
                                WriteMessage(index, "Error!\nError Code: {0:X}\n", errc);
                                return;
                            }


                            WriteMessage(index, "Sending start phase 1 preprocessing signal..\n");
                            WriteMessage(index, "Start: {0}; Length: {1}\n", distributions[index].First, distributions[index].Second);
                            bw.Write(ControlCodes.StartPreprocessing);
                            bw.Write((byte)CacheType.NearestNode);
                            bw.Write((Int64)(distributions[index].First));
                            bw.Write(distributions[index].Second);

                            WriteMessage(index, "Waiting for ACK signal...\n");
                            while (client.Available < 2) ;
                            code = br.ReadUInt16();
                            if (code == ControlCodes.Acknowledge)
                                WriteMessage(index, "Gotcha!\n");
                            else if (code == ControlCodes.Error)
                            {
                                while (client.Available < 4) ;
                                UInt32 errc = br.ReadUInt32();
                                WriteMessage(index, "Error!\nError Code: {0:X}\n", errc);
                                return;
                            }

                            // preprocessing in progress
                            while (true)
                            {
                                // wait for report data
                                while (client.Available < 10) ;

                                UInt16 ctrlCode = br.ReadUInt16();
                                Int32 entry = br.ReadInt32();
                                Int32 total = br.ReadInt32();

                                if (ctrlCode == ControlCodes.Preprocessing)
                                {
                                    WriteMessage(index, "Progress: {0:X} - {1}/{2}\n", ctrlCode, entry, total);
                                    bw.Write(ControlCodes.Acknowledge);
                                }
                                else if (ctrlCode == ControlCodes.WaitingForSync)
                                {
                                    WriteMessage(index, "Progress: Waiting for sync permission..\n", ctrlCode, entry, total);
                                    bw.Write(ControlCodes.Acknowledge);
                                    break;
                                }
                            }

                            WriteMessage(index, "Thread finished, sending disconnect command..\n");
                            bw.Write(ControlCodes.TerminateConnection);

                            while (client.Available < 2) ;
                            
                            code = br.ReadUInt16();
                            if (code == ControlCodes.Acknowledge)
                                WriteMessage(index, "Disconnected from distribution!\n");
                            else
                                WriteMessage(index, "Unrecognized code {0:X}\n", code);

                            while (client.Connected) ; // wait for disconnect
                            WriteMessage(index, "Thread finished, releasing resources!\n");
                        }
                    });
                threads[i] = new Thread(ts);
            }

            for (int i = 0; i < clientCount; i++) threads[i].Start(i);
        }

        private void WriteMessage(Int32 threadIndex, String format, params object[] args)
        {
            txtPpConsole.BeginInvoke(new Action(
                () =>{
                    txtPpConsole.AppendText(String.Format("D#{0}  {1}", threadIndex.ToString().PadLeft(3, '0'), String.Format(format, args)));
                }));
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            pprocStateMan.SetCurrentState("wsPprocConnect");
        }
    }
}
