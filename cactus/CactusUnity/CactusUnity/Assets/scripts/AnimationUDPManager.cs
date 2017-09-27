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

 	private IPEndPoint remoteEndPoint;
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
		Debug.Log ("Receiving");
		try
		{
			client.BeginReceive(new AsyncCallback(ReceiveData), null);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

 	public void SendMsg(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
	}

	private void ReceiveData(IAsyncResult res) {
		IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);
		// Bytes mit der UTF8-Kodierung in das Textformat kodieren.
		string text = Encoding.UTF8.GetString(received);
		Debug.Log("Received: " + text);

       	//Everything received
       	if(text == "xend") {
			Debug.Log("End flag received.");
			//client.Close ();
       	  	sendMissingFrames();
      	}
       	else {
        	try {
         		int f = Convert.ToInt32(text);
         		missingFrames.Add(f);
				//Debug.Log(f);
         	}
         	catch(Exception e) {
          	}
			client.BeginReceive(new AsyncCallback(ReceiveData), null);
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
