using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace RaptoreumDigger
{
    class Digger
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        static extern bool FreeLibrary(int hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        //[DllImport("Ghostrider.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        //private static extern void GhostriderInit();

        //[DllImport("Ghostrider.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        //private static extern void GhostriderWork(byte[] input, byte[] output);

        frmMain main;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GhostriderInit();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GhostriderFree();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GhostriderWork
        (
          byte[] input,
          byte[] output
        );



        public volatile bool done = false;
        public volatile uint FinalNonce = 0;

        Thread[] threads;
        int[] hashCountList;
        DateTime[] hashStartList;

        public Digger(int threadCount)
        {
            threads = new Thread[threadCount];
            hashCountList = new int[threadCount];
            hashStartList = new DateTime[threadCount];
        }

        List<uint> submitList = new List<uint>();
        List<string> submitListThread = new List<string>();
        List<double> hashStatList = new List<double>();

        public void Dig(object sender, DoWorkEventArgs e)
        {
            Job ThisJob = (Job)((object[])e.Argument)[0];
            Ztratum ztratum = (Ztratum)((object[])e.Argument)[1];
            main = (frmMain)((object[])e.Argument)[2];

            main.WriteLog("Starting " + threads.Length + " threads for new job...");

            byte[] databyte = new byte[76];
            uint targetbyte = 0;


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
                targetbyte = Convert.ToUInt32(ThisJob.Target);
            }

            //if (main.cpuMode == 0)
            //{
            //    hModule = LoadLibrary("GhostriderAVX2.dll");
            //}

            //if (main.cpuMode == 1)
            //{
            //    hModule = LoadLibrary("GhostriderSSE2.dll");
            //}

            //intPtr_GhostriderInit = GetProcAddress(hModule, "GhostriderInit");
            //_GhostriderInit = (GhostriderInit)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderInit, typeof(GhostriderInit));

            //intPtr_GhostriderFree = GetProcAddress(hModule, "GhostriderFree");
            //_GhostriderFree = (GhostriderFree)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderFree, typeof(GhostriderFree));

            //intPtr_GhostriderWork = GetProcAddress(hModule, "GhostriderWork");
            //_GhostriderWork = (GhostriderWork)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderWork, typeof(GhostriderWork));

            if (main.statsReset)
            {
                main.statsReset = false;

                main.totalHashFoundList = new int[threads.Length];
                main.totalShareSubmited = 0;
                main.totalShareAccepted = 0;
                main.totalShareRejected = 0;
                main.workStartTime = DateTime.Now;
            }

            done = false;
            FinalNonce = 0;

            submitList = new List<uint>();
            hashCountList = new int[threads.Length];
            hashStartList = new DateTime[threads.Length];
            hashStatList = new List<double>();

            uint workSize = (uint)(uint.MaxValue / threads.Length);

            //_GhostriderInit();

            //Random rnd = new Random();
            int start = 0;

            for (int i = 0; i < threads.Length; i++)
            {
                //start = rnd.Next(0, (int)(workSize / 2));

                ArrayList args = new ArrayList();
                args.Add(ThisJob);
                args.Add(ztratum);
                args.Add(i);
                args.Add(databyte);
                args.Add(targetbyte);
                args.Add((uint)((i * workSize) + start));
                args.Add((uint)((i + 1) * workSize));

                threads[i] = new Thread(new ParameterizedThreadStart(doGR));


                threads[i].IsBackground = false;
                threads[i].Priority = ThreadPriority.BelowNormal;
                threads[i].Start(args);
            }


            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }


            e.Result = null;

            main.WriteLog("Current Hashrate: " + hashStatList.Sum().ToString("0.00") + " Hash/s");
            UpdateStats(hashStatList.Sum().ToString("0.00"));
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

        public void doGR(object o)
        {
            int hModule = 0;

            IntPtr intPtr_GhostriderInit;
            GhostriderInit _GhostriderInit;

            IntPtr intPtr_GhostriderFree;
            GhostriderFree _GhostriderFree;

            IntPtr intPtr_GhostriderWork;
            GhostriderWork _GhostriderWork;

            if (main.cpuMode == 0)
            {
                hModule = LoadLibrary("GhostriderAVX2.dll");
            }

            if (main.cpuMode == 1)
            {
                hModule = LoadLibrary("GhostriderSSE2.dll");
            }

            intPtr_GhostriderInit = GetProcAddress(hModule, "GhostriderInit");
            _GhostriderInit = (GhostriderInit)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderInit, typeof(GhostriderInit));

            intPtr_GhostriderFree = GetProcAddress(hModule, "GhostriderFree");
            _GhostriderFree = (GhostriderFree)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderFree, typeof(GhostriderFree));


            intPtr_GhostriderWork = GetProcAddress(hModule, "GhostriderWork");
            _GhostriderWork = (GhostriderWork)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderWork, typeof(GhostriderWork));


            _GhostriderInit();

            Action a;

            ArrayList args = (ArrayList)o;

            Job ThisJob = (Job)args[0];
            Ztratum ztratum = (Ztratum)args[1];
            int threadId = (int)args[2];
            byte[] Tempdata = (byte[])args[3];
            uint Target = (uint)args[4];
            uint Nonce = (uint)args[5];
            uint MaxNonce = (uint)args[6];

            if (GV.Bench)
                Target = 0x00ff;

            int Hashcount = 0;

            byte[] input = new byte[80];


            Array.Copy(Tempdata, input, 76);

            byte[] n = be32enc(Nonce);

            Array.Copy(n, 0, input, 76, 4);

            byte[] output = new byte[32];

            DateTime StartTime = DateTime.Now;

            DateTime reportTimeStart = DateTime.Now;

            hashStartList[threadId] = StartTime;

            try
            {
                while (!done)
                {
                    if (GV.StopMining)
                    {
                        break;
                    }

                    n = be32enc(Nonce);

                    Array.Copy(n, 0, input, 76, 4);

                    _GhostriderWork(input, output);

                    // GhostriderWork(input, output);

                    uint ou = BitConverter.ToUInt32(output, 28);

                    Hashcount++;
                    main.totalHashFoundList[threadId]++;
                    hashCountList[threadId] = Hashcount;

                    if (ou <= GV.CurrentTarget)
                    {
                        if (!GV.Bench)
                        {
                            if (!submitList.Contains(Nonce))
                            {
                                if (main.lastJob == ThisJob.JobID)
                                {
                                    submitList.Add(Nonce);
                                    submitListThread.Add(Nonce.ToString() + "_" + threadId.ToString());

                                    main.SharesSubmitted++;
                                    main.totalShareSubmited++;

                                    ztratum.SendSUBMIT(ThisJob.JobID, ThisJob.Data.Substring(68 * 2, 8), Nonce, GV.CurrentDifficulty);
                                    a = () => main.pictureBox1.Image = Properties.Resources.EyeDown; main.pictureBox1.Invoke(a);
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

                        main.WriteLog("Current Hashrate: " + hashStatListSub.Sum().ToString("0.00") + " Hash/s");
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

                        main.WriteLog("Current Hashrate: " + hashStatListSub.Sum().ToString("0.00") + " Hash/s");

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
                main.WriteLog(ex.Message);
                FinalNonce = 0;
            }


            _GhostriderFree();



            Marshal.FreeCoTaskMem(intPtr_GhostriderInit);
            Marshal.FreeHGlobal(intPtr_GhostriderFree);
            Marshal.FreeHGlobal(intPtr_GhostriderWork);

            GC.WaitForPendingFinalizers();
            GC.SuppressFinalize(_GhostriderInit);
            GC.SuppressFinalize(_GhostriderFree);
            GC.SuppressFinalize(_GhostriderWork);
            GC.Collect();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            FreeLibrary(hModule);




            double Elapsedtime = (DateTime.Now - StartTime).TotalSeconds;

            main.WriteLog("Thread finished - " + Hashcount.ToString() + " hashes in " + Elapsedtime.ToString("0.00") + " s. Speed: " + (Hashcount / Elapsedtime).ToString("0.00") + " Hash/s");

            hashStatList.Add(Hashcount / Elapsedtime);
        }

        private void UpdateStats(string currentHashRate)
        {
            Action a;

            double Elapsedtime = (DateTime.Now - main.workStartTime).TotalSeconds;

            double avgHash = main.totalHashFoundList.ToList().Sum() / Elapsedtime;

            int totalShare = main.totalShareSubmited;
            int acShare = main.totalShareAccepted;
            int rejShare = main.totalShareRejected;

            a = () => main.lblCurrentHashrate.Text = currentHashRate + " Hash/s"; main.lblCurrentHashrate.Invoke(a);
            a = () => main.lblAverageHashrate.Text = avgHash.ToString("0.00") + " Hash/s"; main.lblAverageHashrate.Invoke(a);
            a = () => main.lblTotalShares.Text = totalShare.ToString(); main.lblTotalShares.Invoke(a);
            a = () => main.lblAcceptedShares.Text = acShare.ToString(); main.lblAcceptedShares.Invoke(a);
            a = () => main.lblRejectedShares.Text = rejShare.ToString(); main.lblRejectedShares.Invoke(a);
            a = () => main.lblCurrentDifficulty.Text = GV.CurrentDifficulty.ToString("0.00000"); main.lblCurrentDifficulty.Invoke(a);
            a = () => main.lblUptime.Text = Elapsedtime.ToString("0.00") + " s"; main.lblUptime.Invoke(a);
        }
    }

}
