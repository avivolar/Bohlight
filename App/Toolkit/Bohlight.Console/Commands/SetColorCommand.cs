using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Sockets;
using Tharga.Toolkit.Console.Commands.Base;
using Bohlight.Manager;

namespace BTSendConsole
{
    public class SetColorCommand : AsyncActionCommandBase
    {
        private LightService _lightServic;

        public SetColorCommand(LightService lightServic)
            : base("color", "Set Color.")
        {
            _lightServic = lightServic;
        }

        public override bool CanExecute(out string reasonMessage)
        {
            if (!_lightServic.IsConnected)
            {
                reasonMessage = "Not connected.";
                return false;
            }

            return base.CanExecute(out reasonMessage);
        }

        public override async Task InvokeAsync(string[] param)
        {
            var red = QueryParam<int>("Red");
            var green = QueryParam<int>("Green");
            var blue = QueryParam<int>("Blue");

            var result = _lightServic.SetColor(red, green, blue);

            OutputInformation(result);
        }
    }
}