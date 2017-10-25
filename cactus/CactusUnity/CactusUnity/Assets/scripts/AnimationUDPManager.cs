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

  	public static int MATRIX_EYEL = 0;
	public static int MATRIX_EYER = 1;
	public static int MATRIX_MOUTHL = 2;
	public static int MATRIX_MOUTHM = 3;
	public static int MATRIX_MOUTHR = 4;

	private Animation currentAnimation;
	private int currentMatrix;
	private ArrayList missingFrames = new ArrayList();

	private UDPReceiver udpR;
	private UDPSender udpS;

  public AnimationUDPManager(string i, int p) {
    ip = i;
    port = p;
   
	udpS = new UDPSender (ip, port);
	udpR = new UDPReceiver (this);
	udpR.init ();
  }

  public void playAnimation(Animation a, int m) {
	Debug.Log ("Playing animation " + a.getName() + " on " + m);
    // /a <filename> <length> <matrix>
	currentAnimation = a;
	currentMatrix = m;
	string filename = a.getName();
    string length = a.getSize().ToString();
    string matrix = m.ToString();
	udpS.SendMsg("/a " + filename + " " + length + " " + matrix);
  }	

	public void onReceive(string text)
	{
		Debug.Log("Received: " + text);

		//Everything received
		if(text == "xend") {
			Debug.Log("End flag received.");
			//sendMissingFrames();
			udpS.SendMsg("Thx for your message!");
			Debug.Log (missingFrames);
		}
		else {
			try {
				int f = Convert.ToInt32(text);
				missingFrames.Add(f);
				//Debug.Log(f);
			}
			catch(Exception exception) {
			}
		}
	}

	private void sendMissingFrames() {
		foreach (int i in missingFrames) {
			// /s <filename> <data>
			string filename = currentAnimation.getByteTextFileName(i);
			string data = currentAnimation.getByteText (i);
			Debug.Log ("Sending missing file: " + filename + "(" + data + ")" );
			udpS.SendMsg("/s " + filename + " " + data);
		}
		missingFrames.Clear ();
		//playAnimation (currentAnimation, currentMatrix);
  }
}