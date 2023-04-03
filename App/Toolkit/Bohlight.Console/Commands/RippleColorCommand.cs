using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Sockets;
using Tharga.Toolkit.Console.Commands.Base;

namespace BTSendConsole
{
    public class RippleColorCommand : AsyncActionCommandBase
    {
        private Func<List<BluetoothDeviceInfo>> _getDevices;
        private BluetoothClient _client;

        public RippleColorCommand(Func<List<BluetoothDeviceInfo>> getDevices, BluetoothClient client)
            : base("ripple", "Ripple between different colors.")
        {
            _getDevices = getDevices;
            _client = client;
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
            var red = 0;
            var green = 0;
            var blue = 0;

            while (true)
            {
                var data = $"?r={red}&g={green}&b={blue};";
                var bytes = Encoding.UTF8.GetBytes(data);
                _client.Client.Send(bytes);

                if (red <= 250)
                    red += 10;
                else if (green <= 250)
                    green += 10;
                else if (blue <= 250)
                    blue += 10;
                else
                {
                    OutputInformation("Done");
                    return;
                }

                await Task.Delay(100);
            }
        }
    }
}