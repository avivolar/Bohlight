using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using InTheHand.Net.Sockets;

public class SetColor : MonoBehaviour
{
    private Bohlight.Manager.LightService _service;
    private BluetoothDeviceInfo[] _devices;

    // Start is called before the first frame update
    void Start()
    {
        _service = new Bohlight.Manager.LightService();
        _service.ListDevices((devices) => {
            _devices = devices;
            foreach (var device in devices)
            {
                Debug.Log($"{device.DeviceName} {device.DeviceAddress}");
            }
        });
        Debug.Log("Searching for bluetooth devices...");
        //_service.Connect("HC-05", (message) => { Debug.Log(message); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string[] GetDevices()
    {
        if (_devices == null) return new string[] { "No devices found..." };
        return _devices.Select(x => x.DeviceName).ToArray();
    }

    public void Set()
    {
        foreach (var deviceName in GetDevices())
        {
            Debug.Log(deviceName);
        }

        // Connect to Device
        //if (deviceName == "HC-05")
        //{
        //}

        //if (!_service.IsConnected)
        //{
        //    Debug.Log("Not connected.");
        //    return;
        //}

        //var result = _service.SetColor(100, 0, 0);
        //Debug.Log($"Color set. {result}");
    }

    public void StartButton()
    {
        var deviceName = "HC-05";

        if (!_service.IsConnected)
        {
            print(deviceName);
            var isConnected = BluetoothService.StartBluetoothConnection(deviceName);
            print(isConnected);
        }
    }
}
