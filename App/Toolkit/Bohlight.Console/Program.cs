using System;
using System.Collections.Generic;
using InTheHand.Net.Sockets;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Entities;
using Bohlight.Manager;

namespace BTSendConsole
{
    class Program
    {
        //private static BluetoothClient _client;
        //private static List<BluetoothDeviceInfo> _devices = new List<BluetoothDeviceInfo>();
        private static LightService _lightService;

        [STAThread]
        private static void Main(string[] args)
        {
            //_client = new BluetoothClient();
            _lightService = new LightService();

            var console = new ClientConsole();

            var command = new RootCommand(console);
            command.RegisterCommand(new ListDevicesCommand(_lightService));
            command.RegisterCommand(new ConnectDevicesCommand(_lightService));
            command.RegisterCommand(new SetColorCommand(_lightService));
            //command.RegisterCommand(new RippleColorCommand(() => { return _devices; }, _client));
            //command.RegisterCommand(new RandomColorCommand(() => { return _devices; }, _client));

            var engine = new CommandEngine(command);
            //engine.TaskRunners = new[]
            //{
            //    new TaskRunner((c,a) =>
            //    {
            //        console.OutputInformation($"Looking for devices...");
            //        var devices = _client.DiscoverDevices();
            //        foreach (BluetoothDeviceInfo d in devices)
            //        {
            //            _devices.Add(d);
            //        }
            //        console.OutputEvent($"Found {_devices.Count} devices.");
            //    })
            //};
            engine.Start(args);
        }
    }
}