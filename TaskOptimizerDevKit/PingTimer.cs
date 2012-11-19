using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace TaskOptimizerDevKit
{
    class PingTimer
    {
        Control parent;
        System.Timers.Timer clock = new System.Timers.Timer();
        IPAddress address = null;
        Action<Int32> callback;

        public PingTimer(Control parent, Action<Int32> callback)
        {
            this.parent = parent;
            this.callback = callback;
            this.clock.Interval = 2000;
            this.clock.AutoReset = true;
            this.clock.Enabled = false;
            this.clock.Elapsed += tick;
        }

        private void tick(Object o, ElapsedEventArgs e)
        {
                    Ping p = new Ping();
                    try
                    {
                        PingReply reply = p.Send(address);
                        parent.BeginInvoke(new Action(() => { callback((int)reply.RoundtripTime); }));
                    }
                    catch (Exception x)
                    {
                        parent.BeginInvoke(new Action(() => { callback(-1); }));
                    }
        }

        public IPAddress Address
        {
            get { return address; }
            set
            {
                this.address = value;
                this.clock.Enabled = address != null;
                if (value != null) tick(null, null);
            }
        }
        
    }
}
