using UnityEngine;
using System.Collections;

public class UDPSender {

	private string ip = "192.168.0.101";
	private int port = 2340;
	private UdpClient client;

	private string sendUdp() {
		client = new UdpClient (port);
	}
}
