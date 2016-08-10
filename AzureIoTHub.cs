using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

static class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "myIoTdevidce". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=IoT-temperature1.azure-devices.net;DeviceId=myIoTdevidce;SharedAccessKey=TGdKDSqdddzf3fq9vPPtOGHPrPdOFtcpdiz8W4kiSZU=";

    //
    // To monitor messages sent to device "myIoTdevidce" use iothub-explorer as follows:
    //    iothub-explorer HostName=IoT-temperature1.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=mZV444b6fvjvEZRpedv4gUtDE7PUhp5SJqtLyK5esfQ= monitor-events "myIoTdevidce"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    public static async Task SendDeviceToCloudMessageAsync(int reportnumber, float temp, float pressure)
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);
        int deviceId = 1;
        var resultobject = new
        {
            count = reportnumber,
            mDeviceId = deviceId,
            mTemprature = temp,
            mPressure = pressure
            
        };

        //#if WINDOWS_UWP
        //        var str = mString ;
        //#else
        //        var str = "Hello, Cloud from a C# app!";
        //#endif
        var reslutmessage = JsonConvert.SerializeObject(resultobject);
        var message = new Message(Encoding.ASCII.GetBytes(reslutmessage));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
