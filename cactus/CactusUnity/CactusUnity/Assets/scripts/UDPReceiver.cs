using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System;

public class UDPReceiver : MonoBehaviour {

	private int port;
	private Thread receiveThread;
	private UdpClient client;

	public bool newMessage = false;
	public string lastReceivedUDPPacket="";
	public string allReceivedUDPPackets=""; // clean up this from time to time!
	private AnimationUDPManager listener;

	public UDPReceiver(AnimationUDPManager listener) {
		this.listener = listener;
	}

	public void init()
	{
		Debug.Log ("Init");
		port = 8051;
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}


	private  void ReceiveData() 
	{
		Debug.Log ("Init Thread");
		client = new UdpClient(port);
		while (true) 
		{

			try 
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

				// Bytes mit der UTF8-Kodierung in das Textformat kodieren.
				string text = Encoding.UTF8.GetString(data);

				//Debug.Log(">> " + text);

				// latest UDPpacket
				lastReceivedUDPPacket = text;
				newMessage = true;
				allReceivedUDPPackets += text;
				listener.onReceive(text);
			}
			catch (Exception err) 
			{
				Debug.Log(err.ToString());
			}
		}
	}

	public bool isNewMessage()
	{
		if (newMessage)
		{
			newMessage = false;
			return true;
		}
		return false;
	}

	public string getLatestUDPPacket()
	{
		allReceivedUDPPackets = "";
		return lastReceivedUDPPacket;
	}

	void OnApplicationQuit()
	{ 
		Console.WriteLine ("Shutdown!!!");
		Debug.Log ("SHutdown");
		if ( receiveThread!= null) 
			receiveThread.Abort(); 
		client.Close(); 
	} 
}
