using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Sockets;
using Tharga.Toolkit.Console.Commands.Base;

namespace BTSendConsole
{
    public class RandomColorCommand : AsyncActionCommandBase
    {
        private Func<List<BluetoothDeviceInfo>> _getDevices;
        private BluetoothClient _client;
        private Random _rng;

        public RandomColorCommand(Func<List<BluetoothDeviceInfo>> getDevices, BluetoothClient client)
            : base("random", "Set random color.")
        {
            _getDevices = getDevices;
            _client = client;

            _rng = new Random();
        }

        public override bool CanExecute(out string reasonMessage)
        {
            if (!_client.Connected)
            {
                reasonMessage = "Not connected.";
                return false;
            }

            return base.CanExecute(out reasonMessage);
        }

        public override async Task InvokeAsync(string[] param)
        {
            for(var i = 0; i < 10; i++)
            {
                var red = _rng.Next(254);
                var green = _rng.Next(254);
                var blue = _rng.Next(254);

                var data = $"?r={red}&g={green}&b={blue};";
                var bytes = Encoding.UTF8.GetBytes(data);
                _client.Client.Send(bytes);

                await Task.Delay(1000);
            }

            OutputInformation("Done");
        }
    }
}