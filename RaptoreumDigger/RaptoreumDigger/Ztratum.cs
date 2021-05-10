using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace RaptoreumDigger
{
    class Ztratum
    {
        public event EventHandler<ZtratumEventArgs> GotSetDifficulty;
        public event EventHandler<ZtratumEventArgs> GotNotify;
        public event EventHandler<ZtratumEventArgs> GotResponse;

        frmMain main;

        public static List<HT> PendingACKs = new List<HT>();
        public TcpClient tcpClient;
        private string page = "";
        public string ExtraNonce1 = "";
        public uint ExtraNonce2 = 0;
        private string Server;
        private int Port;
        private string Username;
        private string Password;
        public int ID;

        public Ztratum(object o)
        {
            main = (frmMain)o;
        }

        public void ConnectToServer(string DigServer, int DigPort, string DigUser, string DdigPassword)
        {
            try
            {
                ID = 1;
                Server = DigServer;
                Port = DigPort;
                Username = DigUser;
                Password = DdigPassword;
                tcpClient = new TcpClient();



                tcpClient.BeginConnect(Server, Port, new AsyncCallback(ConnectCallback), tcpClient);
            }
            catch (Exception ex)
            {
                if (!GV.StopMining)
                {
                    main.WriteLog("Socket error:" + ex.Message);
                }
            }
        }

        public void Close()
        {
            if (tcpClient != null)
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                }
            }

        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (tcpClient.Connected)
                main.WriteLog("Connected");
            else
            {
                main.WriteLog("Unable to connect to server " + Port + " on port " + Port);
                return;
            }


            try
            {
                SendSUBSCRIBE();
                SendAUTHORIZE();

                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];


                networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            }
            catch (Exception ex)
            {
                if (!GV.StopMining)
                {
                    main.WriteLog("Socket error:" + ex.Message);
                }
            }
        }

        public void SendSUBSCRIBE()
        {
            Byte[] bytesSent;
            ZtratumCommand Command = new ZtratumCommand();

            Command.id = ID++;
            Command.method = "mining.subscribe";
            Command.parameters = new ArrayList();

            string request = Utilities.JsonSerialize(Command) + "\n";

            bytesSent = Encoding.ASCII.GetBytes(request);

            try
            {
                tcpClient.GetStream().Write(bytesSent, 0, bytesSent.Length);
                HT ht = new HT(Command.id, Command.method);
                PendingACKs.Add(ht);
            }
            catch (Exception ex)
            {
                if (!GV.StopMining)
                {
                    main.WriteLog("Socket error:" + ex.Message);
                    ConnectToServer(Server, Port, Username, Password);
                }

                return; //
            }

            main.WriteLog("Sent mining.subscribe");
        }

        public void SendAUTHORIZE()
        {
            Byte[] bytesSent;
            ZtratumCommand Command = new ZtratumCommand();

            Command.id = ID++;
            Command.method = "mining.authorize";
            Command.parameters = new ArrayList();
            Command.parameters.Add(Username);
            Command.parameters.Add(Password);

            string request = Utilities.JsonSerialize(Command) + "\n";

            bytesSent = Encoding.ASCII.GetBytes(request);

            try
            {
                tcpClient.GetStream().Write(bytesSent, 0, bytesSent.Length);
                HT ht = new HT(Command.id, Command.method);
                PendingACKs.Add(ht);
            }
            catch (Exception ex)
            {
                if (!GV.StopMining)
                {
                    main.WriteLog("Socket error:" + ex.Message);
                    ConnectToServer(Server, Port, Username, Password);
                }

                return; //
            }

            main.WriteLog("Sent mining.authorize");
        }

        static string LittleEndian(uint number)
        {

            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        public void SendSUBMIT(string JobID, string nTime, uint Nonce, double Difficulty)
        {
            ZtratumCommand Command = new ZtratumCommand();

            Command.id = ID++;
            Command.method = "mining.submit";
            Command.parameters = new ArrayList();
            Command.parameters.Add(Username);
            Command.parameters.Add(JobID.ToLower());
            Command.parameters.Add(LittleEndian(ExtraNonce2).ToLower());
            Command.parameters.Add(nTime.ToLower());
            Command.parameters.Add(LittleEndian(Nonce).ToLower());

            string SubmitString = Utilities.JsonSerialize(Command) + "\n";

            Byte[] bytesSent = Encoding.ASCII.GetBytes(SubmitString);

            try
            {
                tcpClient.GetStream().Write(bytesSent, 0, bytesSent.Length);
                HT ht = new HT(Command.id, Command.method);
                PendingACKs.Add(ht);
            }
            catch (Exception ex)
            {
                if (!GV.StopMining)
                {
                    main.WriteLog("Socket error:" + ex.Message);
                    ConnectToServer(Server, Port, Username, Password);
                }

                return; //
            }

            main.WriteLog("Submit (Difficulty " + Difficulty + ")");
        }

        private void ReadCallback(IAsyncResult result)
        {

            try
            {

                NetworkStream networkStream;
                int bytesread;

                byte[] buffer = result.AsyncState as byte[];

                try
                {
                    networkStream = tcpClient.GetStream();
                    bytesread = networkStream.EndRead(result);
                }
                catch (Exception ex)
                {
                    if (!GV.StopMining)
                    {
                        main.WriteLog("Socket error:" + ex.Message);
                    }
                    return;
                }

                if (bytesread == 0)
                {
                    main.WriteLog("Disconnected. Reconnecting...");

                    tcpClient.Close();
                    tcpClient = null;
                    PendingACKs.Clear();

                    if (!GV.StopMining)
                    {
                        ConnectToServer(Server, Port, Username, Password);
                    }
                    return;
                }


                string data = ASCIIEncoding.ASCII.GetString(buffer, 0, bytesread);


                page = page + data;

                int FoundClose = page.IndexOf('}');

                while (FoundClose > 0)
                {
                    string CurrentString = page.Substring(0, FoundClose + 1);


                    ZtratumCommand Command = Utilities.JsonDeserialize<ZtratumCommand>(CurrentString);
                    ZtratumResponse Response = Utilities.JsonDeserialize<ZtratumResponse>(CurrentString);

                    ZtratumEventArgs e = new ZtratumEventArgs();

                    if (Command.method != null)
                    {

                        e.MiningEventArg = Command;

                        switch (Command.method)
                        {
                            case "mining.notify":
                                if (GotNotify != null)
                                    GotNotify(this, e);
                                break;
                            case "mining.set_difficulty":
                                if (GotSetDifficulty != null)
                                    GotSetDifficulty(this, e);
                                break;
                        }
                    }
                    else if (Response.error != null || Response.result != null)
                    {

                        e.MiningEventArg = Response;



                        string Cmd = "";
                        for (int i = 0; i < PendingACKs.Count; i++)
                        {
                            if (PendingACKs[i].id == Response.id)
                            {
                                Cmd = PendingACKs[i].method;
                                PendingACKs.RemoveAt(i);
                                break;
                            }
                        }

                        if (Cmd == null)
                            main.WriteLog("Unexpected Response");
                        else if (GotResponse != null)
                            GotResponse(Cmd, e);
                    }

                    page = page.Remove(0, FoundClose + 2);
                    FoundClose = page.IndexOf('}');
                }


                networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);


            }
            catch { }


        }
    }

    [DataContract]
    public class ZtratumCommand
    {
        [DataMember]
        public string method;
        [DataMember]
        public System.Nullable<int> id;
        [DataMember(Name = "params")]
        public ArrayList parameters;
    }

    [DataContract]
    public class ZtratumResponse
    {
        [DataMember]
        public ArrayList error;
        [DataMember]
        public System.Nullable<int> id;
        [DataMember]
        public object result;
    }

    public class ZtratumEventArgs : EventArgs
    {
        public object MiningEventArg;
    }
}
