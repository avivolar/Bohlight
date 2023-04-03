using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using System;
using System.Text;

public class manager : MonoBehaviour
{
	public Instantiate instantiate;
	public BluetoothHelper bluetoothHelper;

	private string _deviceName;
	private string _lastSent;
	private string _requestToConnect;
	private string _message;
	private string _receivedMessage;
	private DateTime _lastSentTime;
	private ConcurrentDictionary<string, BluetoothHelper> _connections = new ConcurrentDictionary<string, BluetoothHelper>();

	public void ClearMessage()
	{
        Debug.Log("Clear Message");
        _message = null;
    }

	public void SetRainbowCycleEffect()
	{
		Debug.Log("RainbowCycleEffect");
		_message = $"?1;\n";
	}

	public void SetRainbowEffect()
	{
		Debug.Log("RainbowCycleEffect");
		_message = $"?2;\n";
	}

	public void SetSolidMedTech()
	{
		Debug.Log("SetSolidMedTech");
		ColorPicker ctrl = GameObject.Find("WheelColorPicker").GetComponent<ColorPicker>();
		ctrl.pickedColor.r = (float)5 / 255;
		ctrl.pickedColor.g = (float)70 / 255;
		ctrl.pickedColor.b = (float)100 / 255;
		_message = null;
	}

    public void SetSolidElectro()
    {
        Debug.Log("SetSolidElektro");
        ColorPicker ctrl = GameObject.Find("WheelColorPicker").GetComponent<ColorPicker>();
        ctrl.pickedColor.r = (float)105 / 255;
        ctrl.pickedColor.g = (float)105 / 255;
        ctrl.pickedColor.b = (float)105 / 255;
        _message = null;
    }

    public void SetSolidData()
    {
        Debug.Log("SetSolidData");
        ColorPicker ctrl = GameObject.Find("WheelColorPicker").GetComponent<ColorPicker>();
        ctrl.pickedColor.r = (float)200 / 255;
        ctrl.pickedColor.g = (float)15 / 255;
        ctrl.pickedColor.b = (float)15 / 255;
        _message = null;
    }

    public void SetSolidZero()
    {
        Debug.Log("SetSolidZero");
		var slider = GameObject.Find("Slider").GetComponent<Slider>();

		slider.value = 0;

        _message = null;
    }

    private void Start()
	{
		ToggleManger.RequestConnectEvent += (s, e) =>
		{
			Connect(e.Device);
		};

		try
		{
			bluetoothHelper = BluetoothHelper.GetInstance();
			bluetoothHelper.OnConnected += OnConnected;
			bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
			bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data

			//bluetoothHelper.setFixedLengthBasedStream(3); //receiving every 3 characters together
			bluetoothHelper.setTerminatorBasedStream("\n"); //delimits received messages based on \n char
															//if we received "Hi\nHow are you?"
															//then they are 2 messages : "Hi" and "How are you?"


			// bluetoothHelper.setLengthBasedStream();
			/*
			will received messages based on the length provided, this is useful in transfering binary data
			if we received this message (byte array) :
			{0x55, 0x55, 0, 3, 'a', 'b', 'c', 0x55, 0x55, 0, 9, 'i', ' ', 'a', 'm', ' ', 't', 'o', 'n', 'y'}
			then its parsed as 2 messages : "abc" and "i am tony"
			the first 2 bytes are the length data writted on 2 bytes
			byte[0] is the MSB
			byte[1] is the LSB

			on the unity side, you dont have to add the message length implementation.

			if you call bluetoothHelper.SendData("HELLO");
			this API will send automatically :
			 0x55 0x55    0x00 0x05   0x68 0x65 0x6C 0x6C 0x6F
			|________|   |________|  |________________________|
			 preamble      Length             Data


			when sending data from the arduino to the bluetooth, there's no preamble added.
			this preamble is used to that you receive valid data if you connect to your arduino and its already send data.
			so you will not receive
			on the arduino side you can decode the message by this code snippet:
			char * data;
			char _length[2];
			int length;

			if(Serial.available() >2 )
			{
				_length[0] = Serial.read();
				_length[1] = Serial.read();
				length = (_length[0] << 8) & 0xFF00 | _length[1] & 0xFF00;

				data = new char[length];
				int i=0;
				while(i<length)
				{
					if(Serial.available() == 0)
						continue;
					data[i++] = Serial.read();
				}


				...process received data...


				delete [] data; <--dont forget to clear the dynamic allocation!!!
			}
			*/

			LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();

			//var findCount = 0;

			foreach (BluetoothDevice d in ds)
			{
				Debug.Log($"{d.DeviceName} {d.DeviceAddress}");

				//findCount++;
				//Debug.Log(findCount);

				instantiate.RequestInstantiateOfToggle(d);
				//Debug.Log("Instantiated");
			}
			//Test();
			//bluetoothHelper.Connect();

			//Debug.Log(ds);
			// if(bluetoothHelper.isDevicePaired())
			// 	sphere.GetComponent<Renderer>().material.color = Color.blue;
			// else
			// 	sphere.GetComponent<Renderer>().material.color = Color.grey;
		}
		catch (Exception ex)
		{
			//sphere.GetComponent<Renderer>().material.color = Color.yellow;
			Debug.Log(ex.Message);
			//text.text = ex.Message;
			//BlueToothNotEnabledException == bluetooth Not turned ON
			//BlueToothNotSupportedException == device doesn't support bluetooth
			//BlueToothNotReadyException == the device name you chose is not paired with your android or you are not connected to the bluetooth device;
			//								bluetoothHelper.Connect () returned false;
		}
	}

	private void Update()
	{
		foreach (var connection in _connections.Values)
		{
			if (connection.isConnected())
			{
				if ((DateTime.UtcNow - _lastSentTime).TotalMilliseconds > 200)
				{
					var request = GetMessage();
					if (!string.IsNullOrEmpty(request) && request != _lastSent)
					{
						_lastSentTime = DateTime.UtcNow;
						_lastSent = request;
						connection.SendData(request);
						Debug.Log($"Sent: {request}");
					}
				}
			}
		}
	}

	private string GetMessage()
	{
		if (!string.IsNullOrEmpty(_message))
		{
			return _message;
		}

		return GetColor();
	}

	private string GetColor()
	{
		var slider = GameObject.Find("Slider").GetComponent<Slider>();
		var color = GameObject.Find("WheelColorPicker").GetComponent<ColorPicker>().pickedColor;
		double SliderValue = slider.value;
		double factor = SliderValue / slider.maxValue;
		double energy = 0.4;
		int r = (int)(255 * color.r * factor * energy);
		int g = (int)(255 * color.g * factor * energy);
		int b = (int)(255 * color.b * factor * energy);

		return $"?r={r}&g={g}&b={b};\n";
	}

	private void Connect(BluetoothDevice device)
    {
        try
        {
            var x = BluetoothHelper.GetNewInstance();
			x.setDeviceAddress(device.DeviceAddress);

            Debug.Log("Requesting to connect to: " + device.DeviceAddress);

            x.OnConnected += OnConnected;
            x.OnConnectionFailed += OnConnectionFailed;
            x.OnDataReceived += OnMessageReceived;
            x.setTerminatorBasedStream("\n");
            LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();
			_connections.TryAdd(device.DeviceAddress, x);
            x.Connect();
        }
        catch (Exception exeption)
        {
            Debug.Log(exeption.Message);
        }
    }

	//IEnumerator blinkSphere()
	//{
	//	//sphere.GetComponent<Renderer>().material.color = Color.cyan;
	//	yield return new WaitForSeconds(0.5f);
	//	//sphere.GetComponent<Renderer>().material.color = Color.green;
	//}

	private void OnMessageReceived(BluetoothHelper helper)
	{
		_receivedMessage = helper.Read();
        //Debug.Log(received_message);
	}

	private void OnConnected(BluetoothHelper helper)
	{
		try
		{
			helper.StartListening ();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	private void OnConnectionFailed(BluetoothHelper helper)
	{
		Debug.Log("Connection Failed.");
	}

	private void OnGUI()
	{
	}

	private void OnDestroy()
	{
		if(bluetoothHelper!=null)
		bluetoothHelper.Disconnect ();
	}
}
