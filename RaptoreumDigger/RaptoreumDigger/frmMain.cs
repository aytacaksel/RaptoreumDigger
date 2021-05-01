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

namespace RaptoreumDigger
{

    public partial class frmMain : Form
    {
        string softver = "V1.0";

        private static Digger CDigger;
        private static Queue<Job> IncomingJobs = new Queue<Job>();
        private static Ztratum ztratum;
        private static BackgroundWorker worker;
        public int SharesSubmitted = 0;
        public static int SharesAccepted = 0;
        public static string Server = "";
        public static int Port = 0;
        public static string Username = "";
        public static string Password = "";
        public static int threads = 0;

        private static System.Timers.Timer KeepaliveTimer;

        string settingsFile = Application.StartupPath + "\\Settings.dat";


        public frmMain()
        {
            
            if (IsAdministrator())
            {
                Process.GetCurrentProcess().Kill();
                return;
            }

            InitializeComponent();

            Text += " " + softver;

            cbPriority.SelectedIndex = 2;
            pictureBox1.Image = Properties.Resources.EyeUP;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            nThreadCount.Minimum = 1;
            nThreadCount.Maximum = Environment.ProcessorCount;
            nThreadCount.Value = Environment.ProcessorCount;

            LoadFromFile();

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
            Action a = () => txtLog.AppendText(log + "\r\n"); txtLog.Invoke(a);
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

            GV.StopMining = false;
            GV.ResetMining = false;

            Server = txtPoolUrl.Text.Trim();
            Port = (int)nPoolPort.Value;
            Username = txtUser.Text.Trim();
            Password = txtPassword.Text.Trim();
            threads = (int)nThreadCount.Value;

            txtPoolUrl.Text = Server;

            BackgroundWorker bwMain = new BackgroundWorker();
            bwMain.DoWork += BwMain_DoWork;
            bwMain.RunWorkerAsync();

            while (bwMain.IsBusy)
            {
                Application.DoEvents();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            GV.StopMining = true;
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
            try
            {
                if (GV.Bench)
                {
                    WriteLog("Benchmark starting..");
                    WriteLog("");
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


                CDigger = new Digger(threads);
                StartCDigger();

                Action a = () => btnStop.Enabled = true; btnStop.Invoke(a);

            }
            catch
            {
                Action a = () => btnStart.Enabled = true; btnStart.Invoke(a);
                a = () => gSettings.Enabled = true; gSettings.Invoke(a);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            WriteLog("Keepalive - ");
            ztratum.SendAUTHORIZE();
        }

        private void StartCDigger()
        {
            Job ThisJob = null;

            if (!GV.Bench)
            {
                while (IncomingJobs.Count == 0)
                    Thread.Sleep(500);


                ThisJob = IncomingJobs.Dequeue();

                if (ThisJob.CleanJobs)
                    ztratum.ExtraNonce2 = 0;

                ztratum.ExtraNonce2++;

                string MerkleRoot = Utilities.GenerateMerkleRoot(ThisJob.Coinb1, ThisJob.Coinb2, ztratum.ExtraNonce1, LittleEndian(ztratum.ExtraNonce2), ThisJob.MerkleNumbers);

                uint len = uint.Parse(ThisJob.NetworkDifficulty.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

                double diff = Convert.ToDouble(GV.CurrentDifficulty);
                diff = 65536d / diff;
                GV.CurrentTarget = Convert.ToUInt32(diff);
                ThisJob.Target = Convert.ToUInt32(diff).ToString();
                ThisJob.Data = ThisJob.Version + ThisJob.PreviousHash + MerkleRoot + ThisJob.NetworkTime + ThisJob.NetworkDifficulty;
            }

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(CDigger.Dig);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CDiggerCompleted);
            worker.RunWorkerAsync(new object[] { ThisJob, ztratum, this });
        }

        private string LittleEndian(uint number)
        {

            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        private void CDiggerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GV.ResetMining)
            {
                GV.ResetMining = false;
                GV.StopMining = false;
            }

            if (!GV.StopMining)
            {
                StartCDigger();
            }
            else
            {
                Action a = () => btnStart.Enabled = true; btnStart.Invoke(a);
                a = () => gSettings.Enabled = true; gSettings.Invoke(a);
            }
        }

        private void ztratum_GotResponse(object sender, ZtratumEventArgs e)
        {
            ZtratumResponse Response = (ZtratumResponse)e.MiningEventArg;

            WriteLog("Got Response to " + (string)sender + " - ");

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
                        WriteLog("Share accepted (" + SharesAccepted + " of " + SharesSubmitted + ")");
                    }
                    else
                    {
                        WriteLog("Share rejected. " + Response.error[1]);

                        if (Response.error[1].ToString().Contains("not found"))
                        {
                            GV.ResetMining = true;
                            GV.StopMining = true;
                        }
                    }
                    Action a;
                    a = () => pictureBox1.Image = Properties.Resources.EyeUP; pictureBox1.Invoke(a);
                    a = () => txtLog.BackColor = System.Drawing.Color.White; txtLog.Invoke(a);

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
            ThisJob.PreviousHash = (string)Command.parameters[1];
            ThisJob.Coinb1 = (string)Command.parameters[2];
            ThisJob.Coinb2 = (string)Command.parameters[3];
            Array a = (Array)Command.parameters[4];
            ThisJob.Version = (string)Command.parameters[5];
            ThisJob.NetworkDifficulty = (string)Command.parameters[6];
            ThisJob.NetworkTime = (string)Command.parameters[7];
            ThisJob.CleanJobs = (bool)Command.parameters[8];

            ThisJob.MerkleNumbers = new string[a.Length];

            int i = 0;
            foreach (string s in a)
                ThisJob.MerkleNumbers[i++] = s;


            if (ThisJob.CleanJobs)
            {
                WriteLog("Detected a new block. Stopping old threads.");

                IncomingJobs = new Queue<Job>();
                CDigger.done = true;
            }


            IncomingJobs.Enqueue(ThisJob);
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
