using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using System.Linq;

public class Instantiate : MonoBehaviour
{
    public GameObject deviceObject;
    private ConcurrentDictionary<string, BluetoothDevice> _itemsToInstantiate = new ConcurrentDictionary<string, BluetoothDevice>();

    public void RequestInstantiateOfToggle(BluetoothDevice device)
    {
        //Debug.Log("RequestInstantiateOfToggle");
        _itemsToInstantiate.TryAdd(device.DeviceAddress, device);
    }

    void start()
    {
    }

    void Update()
    {
        var deviceToCreate = _itemsToInstantiate.Keys.FirstOrDefault();

        if (!string.IsNullOrEmpty(deviceToCreate))
        {
            if (_itemsToInstantiate.TryRemove(deviceToCreate, out var device))
            {
                var newDevice = Instantiate(deviceObject);
                var panel = GameObject.Find("Panel");
                newDevice.transform.parent = panel.transform;

                var nameOfDevice = newDevice.GetComponentInChildren<Text>();
                var txt = nameOfDevice.GetComponentInChildren<Text>();
                txt.text = device.DeviceName;

                var x = newDevice.GetComponentInChildren<ToggleManger>();
                x.Device = device;
            }
        }
    }
}