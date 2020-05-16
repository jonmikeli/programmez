using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;

namespace IoT.Simulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Programmez!-Quickstart-Simulateur ReButton");

            try
            {
                using (DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("TO BE REPLACED"))
                {

                    //schema according to official documentation
                    var reButtonAction = new
                    {
                        actionNum = "1",
                        message = "Single click",
                        singeClick = "Single click",
                        batteryVoltage = 2.8
                    };

                    string jsonData = JsonSerializer.Serialize(reButtonAction);

                    var message = new Message(Encoding.UTF8.GetBytes(jsonData));

                    //metadata to define the message type
                    message.Properties.Add("messageType", "data");

                    message.ContentType = "application/json";
                    message.ContentEncoding = "utf-8";

                    await deviceClient.SendEventAsync(message);

                    Console.WriteLine("Message envoyé");
                    Console.WriteLine(jsonData);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();

            }
        }
    }
}
