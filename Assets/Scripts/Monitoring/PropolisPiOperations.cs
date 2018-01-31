using UnityEngine;
using Renci.SshNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using System.Threading;
using System.IO;
using System.Text;
namespace Propolis
{
    public static class PropolisPIOperations
    {
        public const string Username = "pi";
        public const string Password = "raspberry";
        public const string PartialIp = "10.0.1.30"; 
        private static void RunCommand(PropolisAlertUIController alertUIController, int PiID, string command)
        {

            try
            {
                using (SshClient sshclient = new SshClient(PartialIp, Username, Password))
                {
                    sshclient.Connect();
                    SshCommand sc = sshclient.CreateCommand(command);
                    sc.Execute();
                    alertUIController.Show("Propolis Device Communication", sc.Result);

                }
             
            }
            catch (System.Exception)
            {
    
            }

        }

        public static void Close(PropolisAlertUIController alertUIController,int PiID)
        {
            RunCommand(alertUIController,PiID, "sudo shutdown");
        }

        public static void Reboot(PropolisAlertUIController alertUIController,int PiID)
        {
            RunCommand(alertUIController,PiID, "sudo reboot");
        }


        public static void RestartPythonScript(PropolisAlertUIController alertUIController, int PiID)
        {



            //new Thread(() =>
            //{
            //    Debug.Log("Start Thread Update Pi");
            //    using (SshClient sshclient = new SshClient(PartialIp, Username, Password))
            //    {
            //        sshclient.Connect();
            //        Debug.Log("Thread started");
            //        ShellStream stream = sshclient.CreateShellStream("dumb", 80, 24, 800, 2000, 16000);
            //        Thread.Sleep(500);
            //        var result = sendCommand("cd Documents/teensyUpdate/", stream).ToString();
            //        Thread.Sleep(500);
            //        result = sendCommand("ls", stream).ToString();
            //        Thread.Sleep(500);
            //        result = sendCommand("./updateTeensy.sh", stream).ToString();
            //        Thread.Sleep(45000);
            //    }
            //}).Start();


            //new Thread(() =>
            //{
            //    Debug.Log("Start Thread Update Pi");
            //    using (SshClient sshclient = new SshClient(PartialIp, Username, Password))
            //    {
            //        sshclient.Connect();
            //        Debug.Log("Thread started");
            //        ShellStream stream = sshclient.CreateShellStream("dumb", 80, 24, 800, 2000, 16000);
            //        Thread.Sleep(500);
            //        var result = sendCommand("./dispatchCMD.sh updateTeensy hexgroup", stream).ToString();

            //    }
            //}).Start();


            RunCommand(alertUIController, PiID, "./dispatchCMD.sh updateTeensy hexgroup");
            //RunCommand(alertUIController, PiID, ".");

        }

        public static  StringBuilder sendCommand(string customCMD, ShellStream stream)
        {
            StringBuilder answer;

            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            WriteStream(customCMD, writer, stream);
            answer = ReadStream(reader);
            Debug.Log(answer);
            return answer;
        }

        private static void TestSendCommand()
        {
            new Thread(() =>
            {

                try
                {
                    var connectionInfo = new PasswordConnectionInfo(PartialIp, 22, Username, Password);

                    using (var client = new SshClient(connectionInfo))
                    {
                        client.Connect();

                        var command = client.RunCommand("./dispatchCMD.sh updateTeensy Blink");
                        Debug.Log(command.Result);

                        client.Disconnect();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error SSh");

                }
            }).Start();
        }

        private static void WriteStream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.WriteLine(cmd);
            while (stream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        private static StringBuilder ReadStream(StreamReader reader)
        {
            StringBuilder result = new StringBuilder();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                result.AppendLine(line);
            }
            return result;
        }

    }
}
