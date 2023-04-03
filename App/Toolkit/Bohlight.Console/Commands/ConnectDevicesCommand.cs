using Bohlight.Manager;
using Tharga.Toolkit.Console.Commands.Base;

namespace BTSendConsole
{
    public class ConnectDevicesCommand : ActionCommandBase
    {
        private LightService _lightServic;

        public ConnectDevicesCommand(LightService lightServic)
            : base("connect", "Connect to a bluetooth device.")
        {
            _lightServic = lightServic;
        }

        public override bool CanExecute(out string reasonMessage)
        {
            if (_lightServic.IsConnected)
            {
                reasonMessage = "Already connected.";
                return false;
            }

            return base.CanExecute(out reasonMessage);
        }

        public override void Invoke(string[] param)
        {
            var deviceName = QueryParam<string>("Device", param); //, _getDevices().ToDictionary(x => x, x => x.DeviceName));
            _lightServic.Connect(deviceName, (string message) => { OutputEvent(message); });

            OutputInformation("Connecting...");
        }
    }
}