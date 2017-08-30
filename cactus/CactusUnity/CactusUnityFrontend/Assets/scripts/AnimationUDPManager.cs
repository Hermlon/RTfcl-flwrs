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

	private Thread receiveThread;

  	public static int MATRIX_EYEL = 0;
	public static int MATRIX_EYER = 1;
	public static int MATRIX_MOUTHL = 2;
	public static int MATRIX_MOUTHM = 3;
	public static int MATRIX_MOUTHR = 4;

	private Animation currentAnimation;
	private int currentMatrix;
	private int tries = 0;

  public AnimationUDPManager(string i, int p) {
    ip = i;
    port = p;
    remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
	client = new UdpClient();
  }

  public void playAnimation(Animation a, int m) {
    // /a <filename> <length> <matrix>
	currentAnimation = a;
	currentMatrix = m;
	string filename = a.getName();
    string length = a.getSize().ToString();
    string matrix = m.ToString();
	//SendMsgAndReceive("/a " + filename + " " + length + " " + matrix);
	SendMsg("/a " + filename + " " + length + " " + matrix);
  }

	public void SendMsgAndReceive(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
	}

  public void SendMsg(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
	}

	private void ReceiveData() {
    List<int> missingFrames = new List<int>();
		while (true)
		{
			try
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

				// Bytes mit der UTF8-Kodierung in das Textformat kodieren.
				string text = Encoding.UTF8.GetString(data);
				Debug.Log("Received: " + text);

        //Everything received
        if(text == "xend") {
			Debug.Log("End flag received.");
          sendMissingFrames(missingFrames);
          return;
        }
        else {
          try {
            int f = Convert.ToInt32(text);
            missingFrames.Add(f);
          }
          catch(Exception e) {

          }
        }
			}
			catch (Exception err)
			{
				//print(err.ToString());
			}
		}
	}

  private void sendMissingFrames(List<int> mf) {
		foreach (int i in mf) {
			// /s <filename> <data>
			string filename = currentAnimation.getByteTextFileName(i);
			string data = currentAnimation.getByteText (i);
			Debug.Log ("Sending missinf file: " + filename + "(" + data + ")" );
			SendMsg("/s " + filename + " " + data);
		}
		if (tries == 0) {
			playAnimation (currentAnimation, currentMatrix);
			tries++;
		}
  }
}
