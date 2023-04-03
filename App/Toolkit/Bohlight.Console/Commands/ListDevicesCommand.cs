using System;
using System.Linq;
using System.Collections.Generic;
using Bohlight.Manager;
using InTheHand.Net.Sockets;
using Tharga.Toolkit.Console.Commands.Base;

namespace BTSendConsole
{
    public class ListDevicesCommand : ActionCommandBase
    {
        private LightService _lightServic;

        public ListDevicesCommand(LightService lightServic)
            : base("list", "List available bluetooth devices.")
        {
            _lightServic = lightServic;
        }

        public override void Invoke(string[] param)
        {
            _lightServic.ListDevices((devices) =>
            {
                var title = new[] { "Name", "Address", "Connected" };
                var data = devices.Select(x => new[] { x.DeviceName, x.DeviceAddress.ToString(), x.Connected.ToString() });
                OutputTable(title, data);
            });

            OutputInformation("Searching...");
        }
    }
}