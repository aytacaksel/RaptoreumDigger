using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace RaptoreumDigger
{
    class Digger
    {
        ReaderWriterLock thLock = new ReaderWriterLock();

        [Flags]
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }


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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);

        frmMain main;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        delegate void GhostriderInit(IntPtr buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GhostriderWork
        (
          byte[] input,
          byte[] output
        );

        internal static class UnsafeNativeMethods
        {
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        }

        public volatile bool done = false;

        Thread[] threads;
        int[] hashCountList;
        DateTime[] hashStartList;

        public Digger(int threadCount)
        {
            threads = new Thread[threadCount];
            hashCountList = new int[threadCount];
            hashStartList = new DateTime[threadCount];
        }

        List<double> hashStatList = new List<double>();

        IntPtr hModule;

        public void Dig(object sender, DoWorkEventArgs e)
        {
            Job ThisJob = (Job)((object[])e.Argument)[0];
            Ztratum ztratum = (Ztratum)((object[])e.Argument)[1];
            main = (frmMain)((object[])e.Argument)[2];

            main.WriteLog("Starting " + threads.Length + " threads for new job...");

            byte[] databyte = new byte[76];
            uint targetbyte = 0;


            //200000007c6e5530a551ca1496969d0909412315cdefd569e0c64db09207663680af4278c303fb0ff0ad69b0dea42307b075b246822f444ee444450af3b786f7beb377256094edac1d0f1d61

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

                //databyte = Utilities.ReverseByteArrayByFours(Utilities.HexStringToByteArray("200000007c6e5530a551ca1496969d0909412315cdefd569e0c64db09207663680af4278c303fb0ff0ad69b0dea42307b075b246822f444ee444450af3b786f7beb377256094edac1d0f1d61"));
            }
            else
            {
                databyte = Utilities.ReverseByteArrayByFours(Utilities.HexStringToByteArray(ThisJob.Data));
                targetbyte = Convert.ToUInt32(ThisJob.Target);
            }

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

            hashCountList = new int[threads.Length];
            hashStartList = new DateTime[threads.Length];
            hashStatList = new List<double>();

            if (main.submitList.Count == 0)
            {
                GV.lastNonceList = new uint[threads.Length];
            }

            uint workSize = (uint)(uint.MaxValue / threads.Length);

            hModule = LoadLibrary("Ghostrider.dll");

            int start = 0;

            for (int i = 0; i < threads.Length; i++)
            {
                ArrayList args = new ArrayList();
                args.Add(ThisJob);
                args.Add(ztratum);
                args.Add(i);
                args.Add(databyte);
                args.Add(targetbyte);

                if (GV.lastNonceList[i] > 0)
                {
                    args.Add(GV.lastNonceList[i]);
                }
                else
                {
                    args.Add((uint)((i * workSize) + start));
                }

                args.Add((uint)((i + 1) * workSize));

                threads[i] = new Thread(new ParameterizedThreadStart(doGR));

                threads[i].IsBackground = true;
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

            FreeLibrary(hModule);
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
            IntPtr processHandle = Process.GetCurrentProcess().Handle;

            Action a;

            ArrayList args = (ArrayList)o;

            Job ThisJob = (Job)args[0];
            Ztratum ztratum = (Ztratum)args[1];
            int threadId = (int)args[2];
            byte[] Tempdata = (byte[])args[3];
            uint Target = (uint)args[4];
            uint Nonce = (uint)args[5];
            uint MaxNonce = (uint)args[6];

            IntPtr intPtr_GhostriderInit;
            GhostriderInit _GhostriderInit;

            IntPtr intPtr_GhostriderWork;
            GhostriderWork _GhostriderWork;

            intPtr_GhostriderInit = UnsafeNativeMethods.GetProcAddress(hModule, "GhostriderInit");
            _GhostriderInit = (GhostriderInit)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderInit, typeof(GhostriderInit));

            intPtr_GhostriderWork = GetProcAddress(hModule, "GhostriderWork");
            _GhostriderWork = (GhostriderWork)Marshal.GetDelegateForFunctionPointer(intPtr_GhostriderWork, typeof(GhostriderWork));

            IntPtr hp_state = new IntPtr();

            int memSize = (int)(GV.largePageMinimum * 1);

            bool usinglargeMem = false;

            thLock.AcquireWriterLock(Timeout.Infinite);

            try
            {
                if (GV.largeMemAccess)
                {
                    hp_state = VirtualAllocEx(processHandle, IntPtr.Zero, new IntPtr(memSize), AllocationType.Commit | AllocationType.Reserve | AllocationType.LargePages, MemoryProtection.ReadWrite);

                    if (hp_state == IntPtr.Zero)
                    {
                        hp_state = Marshal.AllocHGlobal(memSize);
                        usinglargeMem = false;
                    }
                    else
                    {
                        usinglargeMem = true;
                    }
                }
                else
                {
                    hp_state = Marshal.AllocHGlobal(memSize);
                    usinglargeMem = false;
                }
            }
            catch { }

            thLock.ReleaseLock();

            _GhostriderInit(hp_state);

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


            while (!done)
            {
                if (GV.StopMining)
                {
                    break;
                }

                n = be32enc(Nonce);

                Array.Copy(n, 0, input, 76, 4);

                _GhostriderWork(input, output);

                uint ou = BitConverter.ToUInt32(output, 28);

                Hashcount++;
                main.totalHashFoundList[threadId]++;
                hashCountList[threadId] = Hashcount;

                if (ou <= GV.CurrentTarget)
                {
                    if (!GV.Bench)
                    {
                        if (!main.submitList.Contains(Nonce))
                        {
                            if (main.lastJob == ThisJob.JobID)
                            {
                                main.submitList.Add(Nonce);
                                main.submitListThread.Add(Nonce.ToString() + "_" + threadId.ToString() + "_" + ThisJob.JobID.ToString());

                                main.SharesSubmitted++;
                                main.totalShareSubmited++;

                                ztratum.SendSUBMIT(ThisJob.JobID, ThisJob.Data.Substring(68 * 2, 8), Nonce, GV.CurrentDifficulty);
                                a = () => main.pictureBox1.Image = GV.eyeDown; main.pictureBox1.Invoke(a);
                            }
                            else
                            {
                                done = true;
                            }
                        }
                    }

                    List<double> hashStatListSub = new List<double>();
                    for (int i = 0; i < hashStartList.Length; i++)
                    {
                        double ElapsedtimeSub = (DateTime.Now - hashStartList[i]).TotalSeconds;
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
                        hashStatListSub.Add(hashCountList[i] / ElapsedtimeSub);
                    }

                    main.WriteLog("Current Hashrate: " + hashStatListSub.Sum().ToString("0.00") + " Hash/s");

                    UpdateStats(hashStatListSub.Sum().ToString("0.00"));
                }

                Nonce++;

                GV.lastNonceList[threadId] = Nonce;

                if (Nonce >= MaxNonce)
                {
                    break;
                }

            }

            thLock.AcquireWriterLock(Timeout.Infinite);

            try
            {
                if (usinglargeMem)
                {
                    VirtualFreeEx(processHandle, hp_state, 0, FreeType.Release);
                }
                else
                {
                    Marshal.FreeHGlobal(hp_state);
                }
            }
            catch { }

            thLock.ReleaseLock();

            double Elapsedtime = (DateTime.Now - StartTime).TotalSeconds;

            string largeMemMsg = "";

            if (usinglargeMem)
            {
                largeMemMsg = "LP ON";
            }
            else
            {
                largeMemMsg = "LP OFF";
            }

            main.WriteLog("Thread finished - " + Hashcount.ToString() + " hashes in " + Elapsedtime.ToString("0.00") + " s. Speed: " + (Hashcount / Elapsedtime).ToString("0.00") + " Hash/s" + " - " + largeMemMsg);

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
