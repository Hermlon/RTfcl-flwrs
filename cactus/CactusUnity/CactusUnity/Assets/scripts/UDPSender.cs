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

	public UDPSender (string i, int p) {
		ip = i;
		port = p;
		remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		client = new UdpClient();

	}

	public void SendMsg(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
	}

}
