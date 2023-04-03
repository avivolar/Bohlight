using System.Threading.Tasks;
using System.Text;
using System.Linq;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;

namespace Bohlight.Manager
{
    public class LightService
    {
        private BluetoothClient _client;

        public LightService()
        {
            _client = new BluetoothClient();
        }

        public void ListDevices(Action<BluetoothDeviceInfo[]> callback)
        {
            Task.Run(() =>
            {
                var devices = _client.DiscoverDevices();
                callback(devices.ToArray());
            });
        }

        public void Connect(string deviceName, Action<string> callback)
        {
            Task.Run(() =>
            {
                try
                {
                    var devices = _client.DiscoverDevices();
                    foreach (BluetoothDeviceInfo d in devices)
                    {
                        if (d.DeviceName == deviceName)
                        {
                            var serviceClass = BluetoothService.SerialPort;
                            var ep = new BluetoothEndPoint(d.DeviceAddress, serviceClass);
                            _client.Connect(ep);
                            callback($"Connected to {d.DeviceName}");
                            return;
                        }
                    }

                    callback($"Did not find device named {deviceName}.");
                }
                catch(Exception exception)
                {
                    callback(exception.Message);
                }
            });
        }

        public bool IsConnected
        {
            get
            {
                if (_client == null) return false;
                return _client.Connected;
            }
        }

        public string SetColor(int red, int green, int blue)
        {
            try
            {
                if (!IsConnected) return "Not connected";

                var data = $"?r={red}&g={green}&b={blue};";
                var bytes = Encoding.UTF8.GetBytes(data);
                var r = _client.Client.Send(bytes);
                return $"OK ({data}) [{r}] ({bytes.Length}, {string.Join("", bytes)})";
            }
            catch(Exception exception)
            {
                return exception.Message;
            }
        }
    }
}