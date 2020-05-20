using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using UdpBroadcastCapture.Model;
using System.Threading.Tasks;

namespace UdpBroadcastCapture
{
    class Program
    {
        // https://msdn.microsoft.com/en-us/library/tst0kwb1(v=vs.110).aspx
        // IMPORTANT Windows firewall must be open on UDP port 7000
        // Use the network EGV5-DMU2 to capture from the local IoT devices
        private const int Port = 7000;
        //private static readonly IPAddress IpAddress = IPAddress.Parse("192.168.5.137"); 
        // Listen for activity on all network interfaces
        // https://msdn.microsoft.com/en-us/library/system.net.ipaddress.ipv6any.aspx
        static void Main()
        {

            using (UdpClient socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port)))
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(0, 0);
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast {0}", socket.Client.LocalEndPoint);
                    byte[] datagramReceived = socket.Receive(ref remoteEndPoint);

                    string message = Encoding.ASCII.GetString(datagramReceived, 0, datagramReceived.Length);
                    Console.WriteLine("Receives {0} bytes from {1} port {2} message {3}", datagramReceived.Length,
                        remoteEndPoint.Address, remoteEndPoint.Port, message);

                    Checkin checkin = ParseToCheckin(message);
                    Post("api/checkin", checkin);
                }
            }
        }
        public static async Task<string> Post(string url, Checkin objectToPost)
        {
            string ServerUrl = "https://studinapifinal.azurewebsites.net/";
            HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = true };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ServerUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var headers = client.DefaultRequestHeaders.GetValues("Accept");
                foreach (var VARIABLE in headers)
                {
                    Console.WriteLine(VARIABLE);
                }
                try
                {
                    var serializedString = JsonConvert.SerializeObject(objectToPost);
                    StringContent content = new StringContent(serializedString, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = await client.PostAsync(url, content);

                    if (responseMessage.IsSuccessStatusCode)
                        return await responseMessage.Content.ReadAsStringAsync();

                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

        }
        private static Checkin ParseToCheckin(string msg)
        {
            string[] parts = msg.Split(' ');
            string[] datePart = parts[2].Split('-');
            string[] timePart = parts[3].Split(':');


            for (int i = 0; i < datePart.Length; i++)
            {
                if (datePart[i].Substring(0, 1) == "0") datePart[i] = datePart[i].Substring(1, datePart[i].Length - 1);
            }
            for (int i = 0; i < timePart.Length; i++)
            {
                if (timePart[i].Substring(0, 1) == "0") timePart[i] = timePart[i].Substring(1, timePart[i].Length - 1);
            }

            return new Checkin(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), new DateTime(Convert.ToInt32(datePart[2]), Convert.ToInt32(datePart[1]), Convert.ToInt32(datePart[0]), Convert.ToInt32(timePart[0]), Convert.ToInt32(timePart[1]), 0), parts[4] == "True" ? true : false);
        }
    }
}
