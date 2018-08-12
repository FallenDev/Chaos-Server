// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChaosLauncher
{
    internal partial class Launcher : Form
    {
        private IPAddress ServerIP;
        private IPAddress ClientIP;
        private bool IsLoopback;
        private IPAddress IpToUse;
        private PrivateFontCollection pfc = new PrivateFontCollection();

        Ping ping = new Ping();
        PingOptions options = new PingOptions(128, true);
        byte[] buffer = new byte[32];
        AutoResetEvent token = new AutoResetEvent(false);

        internal Launcher()
        {
            InitializeComponent();

            using (MemoryStream fontStream = new MemoryStream(Properties.Resources.SWTORTrajan))
            {
                byte[] pfcData = fontStream.ToArray();
                unsafe
                {
                    fixed (byte* pfcDataPtr = pfcData)
                        pfc.AddMemoryFont((IntPtr)pfcDataPtr, pfcData.Length);
                }
            }

            launchBtn.Font = new System.Drawing.Font(pfc.Families[0], launchBtn.Font.Size, launchBtn.Font.Style);
            serverStatusLbl.Font = new System.Drawing.Font(pfc.Families[0], serverStatusLbl.Font.Size, serverStatusLbl.Font.Style);
            
#if DEBUG
            ServerIP = IPAddress.Loopback;
            IsLoopback = true;
#else

            ServerIP = Dns.GetHostEntry(Paths.HostName).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            WebRequest request = WebRequest.Create("https://checkip.amazonaws.com");
            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                ClientIP = IPAddress.Parse(reader.ReadToEnd().Trim('\n'));
            }

            IsLoopback = ServerIP.Equals(ClientIP);
#endif

            IpToUse = IsLoopback ? IPAddress.Loopback : ServerIP;

            ping.PingCompleted += new PingCompletedEventHandler(ServerStatus);
            ping.SendAsync(IpToUse, 5000, buffer, options, token);
        }

        private async void ServerStatus(object o, PingCompletedEventArgs a)
        {
            if (a.Reply.Status == IPStatus.Success)
            {
                if (!InvokeRequired)
                {
                    serverStatusImg.Image = Properties.Resources.serverUp;
                    launchBtn.Enabled = true;
                }
                else
                    Invoke((Action)(() =>
                    {
                        serverStatusImg.Image = Properties.Resources.serverUp;
                        launchBtn.Enabled = true;
                    }));
            }
            else
            {
                if (!InvokeRequired)
                {
                    serverStatusImg.Image = Properties.Resources.serverDown;
                    launchBtn.Enabled = false;

                }
                else
                    Invoke((Action)(() =>
                    {
                        serverStatusImg.Image = Properties.Resources.serverDown;
                        launchBtn.Enabled = false;
                    }));
            }

            await Task.Delay(5000);
            ping.SendAsync(IpToUse, 5000, buffer, options, token);
        }

        private void launchBtn_Click(object sender, EventArgs e)
        {
            StartInfo startInfo = new StartInfo();
            ProcInfo procInfo = new ProcInfo();
            startInfo.Size = Marshal.SizeOf(startInfo);


                //create the process
                SafeNativeMethods.CreateProcess(
#if DEBUG
                Paths.DarkAgesExe
#else
                @"Darkages.exe"
#endif
                , null, IntPtr.Zero, IntPtr.Zero, false, ProcessCreationFlags.Suspended, IntPtr.Zero, null, ref startInfo, out procInfo);

            //grab the process we created
            Process proc = Process.GetProcessById(procInfo.ProcessId);

            using (ProcMemoryStream memory = new ProcMemoryStream(procInfo, ProcessAccess.VmOperation | ProcessAccess.VmRead | ProcessAccess.VmWrite))
            {
                //force "socket" - call for direct ip
                memory.Position = 0x4333A2;
                memory.WriteByte(0xEB);

                //edit the direct ip to the server ip
                byte[] address = IpToUse.GetAddressBytes();
                memory.Position = 0x4333C2;
                memory.WriteByte(106);
                memory.WriteByte(address[3]);
                memory.WriteByte(106);
                memory.WriteByte(address[2]);
                memory.WriteByte(106);
                memory.WriteByte(address[1]);
                memory.WriteByte(106);
                memory.WriteByte(address[0]);

                //change port
                memory.Position = 0x4333E4;
                memory.WriteByte(2554 % 256);
                memory.WriteByte(2554 / 256);

                //skip intro
                memory.Position = 0x42E61F;
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);

                //allow multiple instances
                memory.Position = 0x57A7D9;
                memory.WriteByte(0xEB);

                //resume process
                memory.Position = 0x6F3CA4;
                SafeNativeMethods.ResumeThread(procInfo.ThreadHandle);
            }

            //let process render it's window before we change the title
            while (proc.MainWindowHandle == IntPtr.Zero) { }
            SafeNativeMethods.SetWindowText(proc.MainWindowHandle, "Chaos");
        }

        private void infoImg_Click(object sender, EventArgs e)
        {
            MessageDialog.Show(this, this);
        }
    }
}
