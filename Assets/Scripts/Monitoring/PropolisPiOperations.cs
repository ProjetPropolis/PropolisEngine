using UnityEngine;
using Renci.SshNet;
namespace Propolis
{
    public static class PropolisPIOperations
    {
        public const string Username = "pi";
        public const string Password = "raspberry";
        public const string PartialIp = "169.254.64.194"; 
        private static void RunCommand(PropolisAlertUIController alertUIController, int PiID, string command)
        {

            try
            {
                SshClient sshclient = new SshClient(PartialIp, Username, Password);
                sshclient.Connect();
                SshCommand sc = sshclient.CreateCommand(command);
                sc.Execute();
                alertUIController.Show("Propolis Device Communication", sc.Result);
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

            try
            {
                SshClient sshclient = new SshClient(PartialIp, Username, Password);
                sshclient.Connect();
                SshCommand sc = sshclient.CreateCommand("\x004");
                sc.Execute();
                sc = sshclient.CreateCommand("sudo python3 /home/pi/Desktop/python_server/serialTestVF.py");
                sc.Execute();
                alertUIController.Show("Propolis Device Communication", sc.Result);
            }
            catch (System.Exception)
            {

            }
          
        }

    }
}
