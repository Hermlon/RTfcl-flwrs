using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

public class AnimationUDPManager {

 	private string ip;
  	private int port;
	private int receiveport = 56697;

 	private IPEndPoint remoteEndPoint;
	public static bool messageSent = false;
	public static bool messageReceived = false;

	private UdpClient client;

  	public static int MATRIX_EYEL = 0;
	public static int MATRIX_EYER = 1;
	public static int MATRIX_MOUTHL = 2;
	public static int MATRIX_MOUTHM = 3;
	public static int MATRIX_MOUTHR = 4;

	private Animation currentAnimation;
	private int currentMatrix;
	private ArrayList missingFrames = new ArrayList();

  public AnimationUDPManager(string i, int p) {
    ip = i;
    port = p;
    remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
	client = new UdpClient();
  }

  public void playAnimation(Animation a, int m) {
	Debug.Log ("Playing animation " + a.getName() + " on " + m);
    // /a <filename> <length> <matrix>
	currentAnimation = a;
	currentMatrix = m;
	string filename = a.getName();
    string length = a.getSize().ToString();
    string matrix = m.ToString();
	SendMsgAndReceive("/a " + filename + " " + length + " " + matrix);
	//SendMsg("/a " + filename + " " + length + " " + matrix);
  }

	public void SendMsgAndReceive(string msg) {
		SendMsg (msg);
		ReceiveData();
	}

 	public void SendMsg(string msg) {
		// create the udp socket
		UdpClient u = new UdpClient();

		u.Connect(remoteEndPoint);
		Byte [] sendBytes = Encoding.ASCII.GetBytes(msg);

		// send the message
		// the destination is defined by the call to .Connect()
		u.BeginSend(sendBytes, sendBytes.Length, 
			new AsyncCallback(SendCallback), u);

		// Do some work while we wait for the send to complete. For 
		// this example, we'll just sleep
		/*
		while (!messageSent)
		{
			Thread.Sleep(100);
		}*/
	}

	public void SendCallback(IAsyncResult ar)
	{
		UdpClient u = (UdpClient)ar.AsyncState;

		Console.WriteLine("number of bytes sent: {0}", u.EndSend(ar));
		messageSent = true;
	}

	public void ReceiveCallback(IAsyncResult ar)
	{
		UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
		IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

		Byte[] receiveBytes = u.EndReceive(ar, ref e);
		string text = Encoding.ASCII.GetString(receiveBytes);

		Debug.Log("Received: " + text);

		//Everything received
		if(text == "xend") {
			Debug.Log("End flag received.");
			//client.Close ();
			//sendMissingFrames();
			SendMsg ("Test Message");
		}
		else {
			try {
				int f = Convert.ToInt32(text);
				missingFrames.Add(f);
				//Debug.Log(f);
			}
			catch(Exception egg) {
			}
			ReceiveData();
		}
		messageReceived = true;
	}

	private void ReceiveData() {
		Debug.Log ("Receiving");
		IPEndPoint e = new IPEndPoint(IPAddress.Any, receiveport);
		UdpClient u = new UdpClient(e);

		UdpState s = new UdpState();
		s.e = e;
		s.u = u;

		Debug.Log("listening for messages");
		u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

		// Do some work while we wait for a message. For this example,
		// we'll just sleep
		while (!messageReceived)
		{
			Thread.Sleep(100);
		}
	}

	private void sendMissingFrames() {
		foreach (int i in missingFrames) {
			// /s <filename> <data>
			string filename = currentAnimation.getByteTextFileName(i);
			string data = currentAnimation.getByteText (i);
			Debug.Log ("Sending missing file: " + filename + "(" + data + ")" );
			SendMsg("/s " + filename + " " + data);
		}
		missingFrames.Clear ();
		//playAnimation (currentAnimation, currentMatrix);
  }
}

public class UdpState

{



	public IPEndPoint e ;



	public UdpClient u ;

}