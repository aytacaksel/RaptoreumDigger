using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Runtime;

namespace RaptoreumDigger
{

    public partial class frmMain : Form
    {

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            private uint lp;
            private int hp;

            public uint LowPart
            {
                get { return lp; }
                set { lp = value; }
            }

            public int HighPart
            {
                get { return hp; }
                set { hp = value; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            private LUID luid;
            private uint attributes;

            public LUID LUID
            {
                get { return luid; }
                set { luid = value; }
            }

            public uint Attributes
            {
                get { return attributes; }
                set { attributes = value; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            private uint prvct;
            [MarshalAs(UnmanagedType.SafeArray)]
            private LUID_AND_ATTRIBUTES[] privileges;

            public uint PrivilegeCount
            {
                get { return prvct; }
                set { prvct = value; }
            }

            public LUID_AND_ATTRIBUTES[] Privileges
            {
                get { return privileges; }
                set { privileges = value; }
            }
        }

        [DllImport("GhostriderAVX2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderInit")]
        private static extern void GhostriderInitAVX2();

        [DllImport("GhostriderAVX2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderInit2")]
        private static extern void GhostriderInit2AVX2(IntPtr buffer);

        [DllImport("GhostriderAVX2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderFree")]
        private static extern void GhostriderFreeAVX2();

        [DllImport("GhostriderAVX2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderWork")]
        private static extern void GhostriderWorkAVX2(byte[] input, byte[] output);

        [DllImport("GhostriderSSE2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderInit")]
        private static extern void GhostriderInitSSE2();

        [DllImport("GhostriderSSE2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderInit2")]
        private static extern void GhostriderInit2SSE2(IntPtr buffer);

        [DllImport("GhostriderSSE2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderFree")]
        private static extern void GhostriderFreeSSE2();

        [DllImport("GhostriderSSE2.dll", CharSet = CharSet.Auto, EntryPoint = "GhostriderWork")]
        private static extern void GhostriderWorkSSE2(byte[] input, byte[] output);

        [DllImport("kernel32")]
        static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType lAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("advapi32", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, TokenAccessLevels DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint Bufferlength, IntPtr PreviousState, IntPtr ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        string softver = "V1.4";

        private static Ztratum ztratum;
        public int SharesSubmitted = 0;
        public static int SharesAccepted = 0;
        public static string Server = "";
        public static int Port = 0;
        public static string Username = "";
        public static string Password = "";
        public static int threads = 0;

        public int cpuMode = 0;

        private static System.Timers.Timer KeepaliveTimer;

        string settingsFile = Application.StartupPath + "\\Settings.dat";


        public int[] totalHashFoundList;
        public int totalShareSubmited = 0;
        public int totalShareAccepted = 0;
        public int totalShareRejected = 0;
        public DateTime workStartTime;
        public bool statsReset = false;

        public string lastJob = "";

        public bool[] runList;

        byte[] databyte = new byte[76];

        Job ThisJob;

        int runThreadCount = 1;

        List<uint> submitList = new List<uint>();
        List<string> submitListThread = new List<string>();
        List<double> hashStatList = new List<double>();

        int[] hashCountList;
        DateTime[] hashStartList;

        bool largeMemAccess = false;

        public frmMain()
        {
            if (IsAdministrator())
            {
                Process.GetCurrentProcess().Kill();
                return;
            }

            InitializeComponent();

            Text += " " + softver;

            cbPriority.SelectedIndex = 1;
            pictureBox1.Image = Properties.Resources.EyeUP;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                UAC.SetPrivileges();
                largeMemAccess = true;
            }
            catch
            {
                largeMemAccess = false;
            }

            nThreadCount.Minimum = 1;
            nThreadCount.Maximum = Environment.ProcessorCount;
            nThreadCount.Value = Environment.ProcessorCount;

            GV.threads = new Thread[Environment.ProcessorCount];
            GV.threadStateList = new bool[Environment.ProcessorCount];
            GV.NonceList = new uint[Environment.ProcessorCount];
            GV.MaxNonceList = new uint[Environment.ProcessorCount];

            runList = new bool[Environment.ProcessorCount];

            for (int i = 0; i < GV.threads.Length; i++)
            {
                GV.threads[i] = new Thread(new ParameterizedThreadStart(doGR));

                GV.threads[i].IsBackground = false;
                GV.threads[i].Priority = ThreadPriority.BelowNormal;
                GV.threads[i].Start(i);
            }

            LoadFromFile();
        }

        string HexConverted(string strBinary)
        {
            string strHex = Convert.ToInt32(strBinary, 2).ToString("X16");
            return strHex;
        }

        public void doGR(object o)
        {
            int threadId = (int)o;

            //int cpuid = Environment.ProcessorCount - (threadId + 1);

            //char[] tcpu = new char[64];
            //for (int i = 0; i < tcpu.Length; i++)
            //    tcpu[i] = '0';

            //tcpu[tcpu.Length - 1 - cpuid] = '1';

            //string ocpu = new string(tcpu);
            //string aff = HexConverted(ocpu);

            //Process process = Process.GetCurrentProcess();

            //int tid = GetCurrentThreadId();
            //foreach (ProcessThread pt in process.Threads)
            //{
            //    if (pt.Id == tid)
            //    {
            //        long AffinityMask = long.Parse(aff, System.Globalization.NumberStyles.HexNumber);
            //        pt.ProcessorAffinity = (IntPtr)AffinityMask;
            //        pt.IdealProcessor = cpuid;
            //        break;
            //    }
            //}


            int memSize = 1048576 * 2;

            IntPtr hp_state_AVX2 = new IntPtr();
            IntPtr hp_state_SSE2 = new IntPtr();

            if (largeMemAccess)
            {
                hp_state_AVX2 = VirtualAllocEx(Process.GetCurrentProcess().Handle, IntPtr.Zero, new IntPtr(memSize), AllocationType.Commit | AllocationType.Reserve | AllocationType.LargePages, MemoryProtection.ReadWrite);

                if (hp_state_AVX2 == IntPtr.Zero)
                {
                    hp_state_AVX2 = Marshal.AllocHGlobal(memSize);
                    var error = Marshal.GetLastWin32Error();
                }

                hp_state_SSE2 = VirtualAllocEx(Process.GetCurrentProcess().Handle, IntPtr.Zero, new IntPtr(memSize), AllocationType.Commit | AllocationType.Reserve | AllocationType.LargePages, MemoryProtection.ReadWrite);

                if (hp_state_SSE2 == IntPtr.Zero)
                {
                    hp_state_SSE2 = Marshal.AllocHGlobal(memSize);
                    var error = Marshal.GetLastWin32Error();
                }
            }
            else
            {
                hp_state_AVX2 = Marshal.AllocHGlobal(memSize);
                hp_state_SSE2 = Marshal.AllocHGlobal(memSize);
            }

            GhostriderInit2AVX2(hp_state_AVX2);
            GhostriderInit2SSE2(hp_state_SSE2);


            while (true)
            {
                if (!runList[threadId])
                {
                    GV.threadStateList[threadId] = false;
                    Thread.Sleep(50);
                    continue;
                }

                GV.threadStateList[threadId] = true;


                Action a;

                uint Nonce = GV.NonceList[threadId];
                uint MaxNonce = GV.MaxNonceList[threadId];

                if (GV.Bench)
                    GV.CurrentTarget = 0x00ff;

                int Hashcount = 0;

                byte[] input = new byte[80];


                Array.Copy(databyte, input, 76);

                byte[] n = be32enc(Nonce);

                Array.Copy(n, 0, input, 76, 4);

                byte[] output = new byte[32];

                DateTime StartTime = DateTime.Now;

                DateTime reportTimeStart = DateTime.Now;

                hashStartList[threadId] = StartTime;

                try
                {
                    while (runList[threadId])
                    {
                        if (GV.StopMining)
                        {
                            for (int i = 0; i < runList.Length; i++)
                            {
                                runList[i] = false;
                            }

                            break;
                        }

                        n = be32enc(Nonce);

                        Array.Copy(n, 0, input, 76, 4);

                        if (cpuMode == 0)
                            GhostriderWorkAVX2(input, output);

                        if (cpuMode == 1)
                            GhostriderWorkSSE2(input, output);

                        uint ou = BitConverter.ToUInt32(output, 28);

                        Hashcount++;
                        totalHashFoundList[threadId]++;
                        hashCountList[threadId] = Hashcount;

                        if (ou <= GV.CurrentTarget)
                        {
                            if (!GV.Bench)
                            {
                                if (!submitList.Contains(Nonce))
                                {
                                    if (lastJob == ThisJob.JobID)
                                    {
                                        submitList.Add(Nonce);
                                        submitListThread.Add(Nonce.ToString() + "_" + threadId.ToString());

                                        SharesSubmitted++;
                                        totalShareSubmited++;

                                        ztratum.SendSUBMIT(ThisJob.JobID, ThisJob.Data.Substring(68 * 2, 8), Nonce, GV.CurrentDifficulty);
                                        a = () => pictureBox1.Image = Properties.Resources.EyeDown; pictureBox1.Invoke(a);
                                        //a = () => main.txtLog.BackColor = System.Drawing.Color.WhiteSmoke; main.txtLog.Invoke(a);
                                    }
                                }

                            }

                            List<double> hashStatListSub = new List<double>();
                            for (int i = 0; i < hashStartList.Length; i++)
                            {
                                double ElapsedtimeSub = (DateTime.Now - hashStartList[i]).TotalSeconds;
                                //main.WriteLog("Thread " + (i + 1).ToString() + " running - " + hashCountList[i].ToString() + " hashes in " + ElapsedtimeSub.ToString("0.00") + " s. Speed: " + (hashCountList[i] / ElapsedtimeSub).ToString("0.00") + " Hash/s");
                                hashStatListSub.Add(hashCountList[i] / ElapsedtimeSub);
                            }

                            WriteLog("Current Hashrate: " + hashStatListSub.Sum().ToString("0.00") + " Hash/s");
                            UpdateStats(hashStatListSub.Sum().ToString("0.00"));
                        }

                        if ((threadId == 0) && (Nonce > 1) && ((DateTime.Now - reportTimeStart).TotalSeconds > 10))
                        {
                            reportTimeStart = DateTime.Now;
                            List<double> hashStatListSub = new List<double>();
                            for (int i = 0; i < hashStartList.Length; i++)
                            {
                                double ElapsedtimeSub = (DateTime.Now - hashStartList[i]).TotalSeconds;
                                //main.WriteLog("Thread " + (i + 1).ToString() + " running - " + hashCountList[i].ToString() + " hashes in " + ElapsedtimeSub.ToString("0.00") + " s. Speed: " + (hashCountList[i] / ElapsedtimeSub).ToString("0.00") + " Hash/s");
                                hashStatListSub.Add(hashCountList[i] / ElapsedtimeSub);
                            }

                            WriteLog("Current Hashrate: " + hashStatListSub.Sum().ToString("0.00") + " Hash/s");

                            UpdateStats(hashStatListSub.Sum().ToString("0.00"));

                        }

                        Nonce++;

                        if (Nonce >= MaxNonce)
                        {
                            break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                    //FinalNonce = 0;
                }


                double Elapsedtime = (DateTime.Now - StartTime).TotalSeconds;

                WriteLog("Thread finished - " + Hashcount.ToString() + " hashes in " + Elapsedtime.ToString("0.00") + " s. Speed: " + (Hashcount / Elapsedtime).ToString("0.00") + " Hash/s");

                hashStatList.Add(Hashcount / Elapsedtime);
            }
        }

        static byte[] be32enc(uint x)
        {
            byte[] p = new byte[4];
            p[3] = (byte)(x & 0xff);
            p[2] = (byte)((x >> 8) & 0xff);
            p[1] = (byte)((x >> 16) & 0xff);
            p[0] = (byte)((x >> 24) & 0xff);
            return p;
        }

        private void UpdateStats(string currentHashRate)
        {
            Action a;

            double Elapsedtime = (DateTime.Now - workStartTime).TotalSeconds;

            double avgHash = totalHashFoundList.ToList().Sum() / Elapsedtime;

            int totalShare = totalShareSubmited;
            int acShare = totalShareAccepted;
            int rejShare = totalShareRejected;

            a = () => lblCurrentHashrate.Text = currentHashRate + " Hash/s"; lblCurrentHashrate.Invoke(a);
            a = () => lblAverageHashrate.Text = avgHash.ToString("0.00") + " Hash/s"; lblAverageHashrate.Invoke(a);
            a = () => lblTotalShares.Text = totalShare.ToString(); lblTotalShares.Invoke(a);
            a = () => lblAcceptedShares.Text = acShare.ToString(); lblAcceptedShares.Invoke(a);
            a = () => lblRejectedShares.Text = rejShare.ToString(); lblRejectedShares.Invoke(a);
            a = () => lblCurrentDifficulty.Text = GV.CurrentDifficulty.ToString("0.00000"); lblCurrentDifficulty.Invoke(a);
            a = () => lblUptime.Text = Elapsedtime.ToString("0.00") + " s"; lblUptime.Invoke(a);
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public void WriteLog(string log)
        {
            DateTime dtNow = DateTime.Now;

            string timeStamp = dtNow.Hour.ToString("00") + ":" + dtNow.Minute.ToString("00") + ":" + dtNow.Second.ToString("00") + "." + dtNow.Millisecond.ToString("000") + "> ";

            if (log == "")
            {
                timeStamp = "";
            }

            Action a = () => txtLog.AppendText(timeStamp + log + "\r\n"); txtLog.Invoke(a);
        }

        private void LoadFromFile()
        {
            if (File.Exists(settingsFile))
            {
                using (StreamReader sr = new StreamReader(settingsFile, Encoding.Default))
                {
                    string s1 = sr.ReadLine().Trim();
                    string s2 = sr.ReadLine().Trim();
                    string s3 = sr.ReadLine().Trim();
                    string s4 = sr.ReadLine().Trim();

                    Server = s1;
                    Port = Convert.ToInt32(s2);
                    Username = s3;
                    Password = s4;
                }

                txtPoolUrl.Text = Server;
                nPoolPort.Value = Port;
                txtUser.Text = Username;
                txtPassword.Text = Password;
            }
        }

        private void SaveToFile()
        {
            if (File.Exists(settingsFile))
            {
                File.Delete(settingsFile);
            }

            using (StreamWriter sw = new StreamWriter(settingsFile, false, Encoding.Default))
            {
                sw.WriteLine(txtPoolUrl.Text.Trim());
                sw.WriteLine(nPoolPort.Value.ToString());
                sw.WriteLine(txtUser.Text.Trim());
                sw.WriteLine(txtPassword.Text.Trim());
            }
        }

        private void txtPoolUrl_Leave(object sender, EventArgs e)
        {
            string[] serverport = txtPoolUrl.Text.Replace("s" + "t" + "ratum+", "").Replace("http://", "").Replace("tcp://", "").Trim().Split(':');
            txtPoolUrl.Text = serverport[0].Trim();

            if (serverport.Length > 1)
            {
                int port = 0;
                try { port = Convert.ToInt32(serverport[1]); } catch { }

                if ((port > 0) && (port < 65535))
                {
                    nPoolPort.Value = port;
                }
            }
            SaveToFile();
        }

        private void nPoolPort_Leave(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void txtUser_Leave(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void nThreadCount_Leave(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            gSettings.Enabled = false;

            lblCurrentHashrate.Text = "";
            lblAverageHashrate.Text = "";
            lblTotalShares.Text = "";
            lblAcceptedShares.Text = "";
            lblRejectedShares.Text = "";
            lblCurrentDifficulty.Text = "";
            lblUptime.Text = "";

            statsReset = true;

            GV.StopMining = false;
            GV.ResetMining = false;

            Server = txtPoolUrl.Text.Trim();
            Port = (int)nPoolPort.Value;
            Username = txtUser.Text.Trim();
            Password = txtPassword.Text.Trim();
            threads = (int)nThreadCount.Value;

            txtPoolUrl.Text = Server;

            SharesSubmitted = 0;
            SharesAccepted = 0;

            BackgroundWorker bwMain = new BackgroundWorker();
            bwMain.DoWork += BwMain_DoWork;
            bwMain.RunWorkerAsync();

            while (bwMain.IsBusy)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (KeepaliveTimer != null)
            {
                if (KeepaliveTimer.Enabled)
                {
                    KeepaliveTimer.Stop();
                }
            }

            GV.StopMining = true;

            if (ztratum != null)
            {
                ztratum.Close();
            }

            for (int i = 0; i < runList.Length; i++)
            {
                runList[i] = false;
            }

            btnStop.Enabled = false;

        }

        private string InvokeMethod(string method, string paramString = null)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Server);
            webRequest.Credentials = new NetworkCredential(Username, Password);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            string jsonParam = (paramString != null) ? "\"" + paramString + "\"" : "";
            string request = "{\"id\": 0, \"method\": \"" + method + "\", \"params\": [" + jsonParam + "]}";

            // serialize json for the request
            byte[] byteArray = Encoding.UTF8.GetBytes(request);
            webRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = webRequest.GetRequestStream())
                dataStream.Write(byteArray, 0, byteArray.Length);

            string reply = "";
            using (WebResponse webResponse = webRequest.GetResponse())
            using (Stream str = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(str))
                reply = reader.ReadToEnd();

            return reply;
        }

        public Work GetWork()
        {
            Work x = new Work(ParseData(InvokeMethod("getwork")));
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Done\n");
            Console.ForegroundColor = ConsoleColor.White;
            return x;
        }

        private byte[] ParseData(string json)
        {
            Match match = Regex.Match(json, "\"data\": \"([A-Fa-f0-9]+)");
            if (match.Success)
            {
                string data = Utils.RemovePadding(match.Groups[1].Value);
                data = Utils.EndianFlip32BitChunks(data);
                return Utils.ToBytes(data);
            }
            throw new Exception("Didn't find valid 'data' in Server Response");
        }

        public bool SendShare(byte[] share)
        {
            string data = Utils.EndianFlip32BitChunks(Utils.ToString(share));
            string paddedData = Utils.AddPadding(data);
            string jsonReply = InvokeMethod("getwork", paddedData);
            Match match = Regex.Match(jsonReply, "\"result\": true");
            return match.Success;
        }

        private void BwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            Action a;

            try
            {
                runThreadCount = (int)nThreadCount.Value;

                if (GV.Bench)
                {
                    WriteLog("Benchmark starting..");
                    WriteLog("");


                    InitWork();
                }
                else
                {
                    if (Server == "")
                    {
                        WriteLog("Missing server URL. URL should be in the format of rtm.suprnova.cc");
                        throw new Exception();
                    }
                    else if (Port == 0)
                    {
                        WriteLog("Missing server port.");
                        return;
                    }
                    else if (Username == "")
                    {
                        WriteLog("Missing username");
                        throw new Exception();
                    }
                    else if (Password == "")
                    {
                        WriteLog("Missing password");
                        throw new Exception();
                    }

                    WriteLog("Connecting to '" + Server + "' on port '" + Port + "' with username '" + Username + "' and password '" + Password + "'");
                    WriteLog("");

                    ztratum = new Ztratum(this);
                    KeepaliveTimer = new System.Timers.Timer(45000);
                    KeepaliveTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    KeepaliveTimer.Start();

                    ztratum.GotResponse += ztratum_GotResponse;
                    ztratum.GotSetDifficulty += ztratum_GotSetDifficulty;
                    ztratum.GotNotify += ztratum_GotNotify;

                    ztratum.ConnectToServer(Server, Port, Username, Password);
                }



                a = () => btnStop.Enabled = true; btnStop.Invoke(a);

                while (!GV.StopMining)
                {
                    Thread.Sleep(100);
                }

                while (true)
                {
                    bool stateResult = false;

                    for (int i = 0; i < GV.threadStateList.Length; i++)
                    {
                        if (GV.threadStateList[i])
                        {
                            stateResult = true;
                        }
                    }

                    if (!stateResult)
                    {
                        break;
                    }

                    Thread.Sleep(50);
                }

                if (hashStatList.Count > 0)
                {
                    if (hashStatList.Sum() > 0)
                    {
                        WriteLog("Current Hashrate: " + hashStatList.Sum().ToString("0.00") + " Hash/s");
                        UpdateStats(hashStatList.Sum().ToString("0.00"));
                    }
                }
            }
            catch
            {

            }

            a = () => btnStart.Enabled = true; btnStart.Invoke(a);
            a = () => gSettings.Enabled = true; gSettings.Invoke(a);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (GV.StopMining)
            {
                return;
            }

            if (!GV.Bench)
            {
                WriteLog("Keepalive");
                ztratum.SendAUTHORIZE();
            }
        }

        private string LittleEndian(uint number)
        {

            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        private void ztratum_GotResponse(object sender, ZtratumEventArgs e)
        {
            ZtratumResponse Response = (ZtratumResponse)e.MiningEventArg;

            WriteLog("Got Response to " + (string)sender);

            switch ((string)sender)
            {
                case "mining.authorize":
                    if ((bool)Response.result)
                        WriteLog("Worker authorized");
                    else
                    {
                        WriteLog("Worker rejected");
                        return;
                    }
                    break;

                case "mining.subscribe":
                    ztratum.ExtraNonce1 = (string)((object[])Response.result)[1];
                    WriteLog("Subscribed. ExtraNonce1 set to " + ztratum.ExtraNonce1);
                    break;

                case "mining.submit":
                    if (Response.result != null && (bool)Response.result)
                    {
                        SharesAccepted++;
                        totalShareAccepted++;
                        WriteLog("Share accepted (" + SharesAccepted + " of " + SharesSubmitted + ")");
                    }
                    else
                    {
                        totalShareRejected++;
                        WriteLog("Share rejected. " + Response.error[1]);

                        //if (Response.error[1].ToString().Contains("not found"))
                        //{
                        //    for (int i = 0; i < runList.Length; i++)
                        //    {
                        //        runList[i] = false;
                        //    }
                        //}
                    }
                    Action a;
                    a = () => pictureBox1.Image = Properties.Resources.EyeUP; pictureBox1.Invoke(a);
                    //a = () => txtLog.BackColor = System.Drawing.Color.White; txtLog.Invoke(a);

                    break;
                default:
                    break;
            }
        }

        private void ztratum_GotSetDifficulty(object sender, ZtratumEventArgs e)
        {
            ZtratumCommand Command = (ZtratumCommand)e.MiningEventArg;
            GV.CurrentDifficulty = Convert.ToDouble(Command.parameters[0]);
            double diff = Convert.ToDouble(GV.CurrentDifficulty);
            diff = 65536d / diff;
            GV.CurrentTarget = Convert.ToUInt32(diff);

            WriteLog("Got Set_Difficulty " + GV.CurrentDifficulty);
        }

        private void ztratum_GotNotify(object sender, ZtratumEventArgs e)
        {
            Job ThisJob = new Job();
            ZtratumCommand Command = (ZtratumCommand)e.MiningEventArg;

            ThisJob.JobID = (string)Command.parameters[0];

            lastJob = ThisJob.JobID;

            ThisJob.PreviousHash = (string)Command.parameters[1];
            ThisJob.Coinb1 = (string)Command.parameters[2];
            ThisJob.Coinb2 = (string)Command.parameters[3];
            Array a = (Array)Command.parameters[4];
            ThisJob.Version = (string)Command.parameters[5];
            ThisJob.NetworkDifficulty = (string)Command.parameters[6];
            ThisJob.NetworkTime = (string)Command.parameters[7];
            ThisJob.CleanJobs = (bool)Command.parameters[8];

            ThisJob.MerkleNumbers = new string[a.Length];

            int index = 0;
            foreach (string s in a)
            {
                ThisJob.MerkleNumbers[index++] = s;
            }

            if (ThisJob.CleanJobs)
            {
                WriteLog("Detected a new block. Stopping old threads.");
                WriteLog("Job: " + ThisJob.JobID);

                ztratum.ExtraNonce2 = 0;
            }
            else
            {
                WriteLog("Detected a new job. Stopping old threads.");
                WriteLog("Job: " + ThisJob.JobID);
            }

            for (int i = 0; i < runList.Length; i++)
            {
                runList[i] = false;
            }


            while (true)
            {
                bool stateResult = false;

                for (int i = 0; i < GV.threadStateList.Length; i++)
                {
                    if (GV.threadStateList[i])
                    {
                        stateResult = true;
                    }
                }

                if (!stateResult)
                {
                    break;
                }

                Thread.Sleep(50);
            }

            if (hashStatList.Count > 0)
            {
                if (hashStatList.Sum() > 0)
                {
                    WriteLog("Current Hashrate: " + hashStatList.Sum().ToString("0.00") + " Hash/s");
                    UpdateStats(hashStatList.Sum().ToString("0.00"));
                }
            }

            ztratum.ExtraNonce2++;

            string MerkleRoot = Utilities.GenerateMerkleRoot(ThisJob.Coinb1, ThisJob.Coinb2, ztratum.ExtraNonce1, LittleEndian(ztratum.ExtraNonce2), ThisJob.MerkleNumbers);

            // uint len = uint.Parse(ThisJob.NetworkDifficulty.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

            double diff = Convert.ToDouble(GV.CurrentDifficulty);
            diff = 65536d / diff;
            GV.CurrentTarget = Convert.ToUInt32(diff);
            ThisJob.Target = Convert.ToUInt32(diff).ToString();
            ThisJob.Data = ThisJob.Version + ThisJob.PreviousHash + MerkleRoot + ThisJob.NetworkTime + ThisJob.NetworkDifficulty;

            WriteLog("Starting " + runThreadCount + " threads for new job...");

            InitWork(ThisJob);
        }

        private void InitWork(Job ThisJob = null)
        {
            databyte = new byte[76];

            if (GV.Bench)
            {
                for (byte n = 0; n < 74; n++)
                    databyte[n] = n;

                databyte[17] = BitConverter.GetBytes(1619664393)[0];
                databyte[18] = BitConverter.GetBytes(1619664393)[1];
                databyte[19] = BitConverter.GetBytes(1619664393)[2];
                databyte[20] = BitConverter.GetBytes(1619664393)[3];

                databyte[20] = BitConverter.GetBytes(0x80000000)[0];
                databyte[21] = BitConverter.GetBytes(0x80000000)[1];
                databyte[22] = BitConverter.GetBytes(0x80000000)[2];
                databyte[23] = BitConverter.GetBytes(0x80000000)[3];

                databyte[31] = BitConverter.GetBytes(0x00000280)[0];
                databyte[32] = BitConverter.GetBytes(0x00000280)[1];
                databyte[33] = BitConverter.GetBytes(0x00000280)[2];
                databyte[34] = BitConverter.GetBytes(0x00000280)[3];

            }
            else
            {
                databyte = Utilities.ReverseByteArrayByFours(Utilities.HexStringToByteArray(ThisJob.Data));
            }

            submitList = new List<uint>();
            hashCountList = new int[GV.threads.Length];
            hashStartList = new DateTime[GV.threads.Length];
            hashStatList = new List<double>();

            uint workSize = (uint)(uint.MaxValue / GV.threads.Length);

            GV.NonceList = new uint[GV.threads.Length];
            GV.MaxNonceList = new uint[GV.threads.Length];

            uint start = 0;

            for (int i = 0; i < GV.threads.Length; i++)
            {
                GV.NonceList[i] = (uint)((i * workSize) + start);
                GV.MaxNonceList[i] = (uint)((i + 1) * workSize);
            }

            this.ThisJob = ThisJob;

            if (statsReset)
            {
                statsReset = false;

                totalHashFoundList = new int[GV.threads.Length];
                totalShareSubmited = 0;
                totalShareAccepted = 0;
                totalShareRejected = 0;
                workStartTime = DateTime.Now;
            }

            for (int i = 0; i < runThreadCount; i++)
            {
                runList[i] = true;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void chkBench_CheckedChanged(object sender, EventArgs e)
        {
            GV.Bench = chkBench.Checked;
        }

        private void cbPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPriority.SelectedIndex == 0)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            }

            if (cbPriority.SelectedIndex == 1)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            }

            if (cbPriority.SelectedIndex == 2)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
            }

            if (cbPriority.SelectedIndex == 3)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;
            }


        }

        private void tTheme_Scroll(object sender, EventArgs e)
        {


            if (tTheme.Value == 0)
            {
                GV.DarkTheme = false;
            }
            else
            {
                GV.DarkTheme = true;

                Color light = Color.Turquoise;
                Color dark = Color.Black;

                BackColor = dark;

                label1.ForeColor = light;
                label2.ForeColor = light;
                label3.ForeColor = light;
                label4.ForeColor = light;
                label5.ForeColor = light;
                label6.ForeColor = light;
                label7.ForeColor = light;
                label8.ForeColor = light;

                gSettings.ForeColor = light;
                gControl.ForeColor = light;
                gLog.ForeColor = light;


                txtPoolUrl.BackColor = dark;
                nPoolPort.BackColor = dark;
                txtUser.BackColor = dark;
                txtPassword.BackColor = dark;
                nThreadCount.BackColor = dark;
                chkBench.BackColor = dark;
                txtLog.BackColor = dark;


                txtPoolUrl.ForeColor = light;
                nPoolPort.ForeColor = light;
                txtUser.ForeColor = light;
                txtPassword.ForeColor = light;
                nThreadCount.ForeColor = light;
                chkBench.ForeColor = light;
                txtLog.ForeColor = light;
                txtLog.ForeColor = light;




            }
        }

        private void rAVX2_CheckedChanged(object sender, EventArgs e)
        {
            CPUMode_Changed();
        }

        private void rSSE2_CheckedChanged(object sender, EventArgs e)
        {
            CPUMode_Changed();
        }

        private void CPUMode_Changed()
        {
            if (rAVX2.Checked)
            {
                cpuMode = 0;
            }
            else
            {
                cpuMode = 1;
            }
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            if (txtLog.Lines.Length > 25)
            {
                List<string> logLines = txtLog.Lines.ToList();

                while (logLines.Count > 25)
                {
                    logLines.RemoveAt(0);
                }

                txtLog.Lines = logLines.ToArray();

                txtLog.SelectionStart = txtLog.TextLength;
                txtLog.ScrollToCaret();
            }
        }
    }

    public class Job
    {

        public string JobID;
        public string PreviousHash;
        public string Coinb1;
        public string Coinb2;
        public string[] MerkleNumbers;
        public string Version;
        public string NetworkDifficulty;
        public string NetworkTime;
        public bool CleanJobs;


        public string Target;
        public string Data;


        public uint Answer;
    }

    public class Work
    {
        public Work(byte[] data)
        {
            Data = data;
            Current = (byte[])data.Clone();
            _nonceOffset = Data.Length - 4;
            _ticks = DateTime.Now.Ticks;
            _hasher = new SHA256Managed();

        }
        private SHA256Managed _hasher;
        private long _ticks;
        private long _nonceOffset;
        public byte[] Data;
        public byte[] Current;

        internal bool FindShare(ref uint nonce, uint batchSize)
        {
            for (; batchSize > 0; batchSize--)
            {
                BitConverter.GetBytes(nonce).CopyTo(Current, _nonceOffset);
                byte[] doubleHash = Sha256(Sha256(Current));

                //count trailing bytes that are zero
                int zeroBytes = 0;
                for (int i = 31; i >= 28; i--, zeroBytes++)
                    if (doubleHash[i] > 0)
                        break;

                //standard share difficulty matched! (target:ffffffffffffffffffffffffffffffffffffffffffffffffffffffff00000000)
                if (zeroBytes == 4)
                    return true;

                //increase
                if (++nonce == uint.MaxValue)
                    nonce = 0;
            }
            return false;
        }

        private byte[] Sha256(byte[] input)
        {
            byte[] crypto = _hasher.ComputeHash(input, 0, input.Length);
            return crypto;
        }

        public byte[] Hash
        {
            get { return Sha256(Sha256(Current)); }
        }

        public long Age
        {
            get { return DateTime.Now.Ticks - _ticks; }
        }
    }

    class Utils
    {
        public static byte[] ToBytes(string input)
        {
            byte[] bytes = new byte[input.Length / 2];
            for (int i = 0, j = 0; i < input.Length; j++, i += 2)
                bytes[j] = byte.Parse(input.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);

            return bytes;
        }

        public static string ToString(byte[] input)
        {
            string result = "";
            foreach (byte b in input)
                result += b.ToString("x2");

            return result;
        }

        public static string ToString(uint value)
        {
            string result = "";
            foreach (byte b in BitConverter.GetBytes(value))
                result += b.ToString("x2");

            return result;
        }

        public static string EndianFlip32BitChunks(string input)
        {
            //32 bits = 4*4 bytes = 4*4*2 chars
            string result = "";
            for (int i = 0; i < input.Length; i += 8)
                for (int j = 0; j < 8; j += 2)
                {
                    //append byte (2 chars)
                    result += input[i - j + 6];
                    result += input[i - j + 7];
                }
            return result;
        }

        public static string RemovePadding(string input)
        {
            //payload length: final 64 bits in big-endian - 0x0000000000000280 = 640 bits = 80 bytes = 160 chars
            return input.Substring(0, 160);
        }

        public static string AddPadding(string input)
        {
            //add the padding to the payload. It never changes.
            return input + "000000800000000000000000000000000000000000000000000000000000000000000000000000000000000080020000";
        }
    }

}
