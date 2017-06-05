using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSender {

	private string ip;
	private int port;

	IPEndPoint remoteEndPoint;
	UdpClient client;

	Thread receiveThread;
	public string lastReceivedUDPPacket="";
	public string allReceivedUDPPackets=""; 

	public UDPSender (string i, int p, ChooseAnimationGUI listener) {
		ip = i;
		port = p;
		remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		client = new UdpClient();

	}

	public void SendMsg(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}

	private void ReceiveData() {
		while (true)
		{
			try
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

				// Bytes mit der UTF8-Kodierung in das Textformat kodieren.
				string text = Encoding.UTF8.GetString(data);
				
			}
			catch (Exception err)
			{
				//print(err.ToString());
			}
		}

	}
}
