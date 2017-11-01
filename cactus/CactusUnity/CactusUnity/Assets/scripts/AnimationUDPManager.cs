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
	private int lastsend;

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
		if(text == "/e") {
			Debug.Log("End flag received.");
			lastsend = 0;
			sendMissingFrame(lastsend);
		}
		//get missing frame (/g x)
		if (text.Contains("/g")) {
			try {
				int f = Convert.ToInt32(text.Split(' ')[1]);
				missingFrames.Add(f);
				//Debug.Log(f);
			}
			catch(Exception exception) {
			}
		}
		//Next data
		if (text.Contains ("/n")) {
			Debug.Log ("Next frame");
			sendMissingFrame(lastsend);

			//The last frame was being send
			if (lastsend == currentAnimation.getSize ()) {
				//Now that the animation has been send to the arduino it can be played
				playAnimation (currentAnimation, currentMatrix);
			}
		}
		
	}

	private void sendMissingFrame(int frame) {
		// /s <filename> <data>
		string filename = currentAnimation.getByteTextFileName(frame);
		string data = currentAnimation.getByteText (frame);
		Debug.Log ("Sending missing file: " + filename + "(" + data + ")" );
		udpS.SendMsg("/s " + filename + " " + data);
		lastsend++;
		//playAnimation (currentAnimation, currentMatrix);
  }

	public void quit() {
		udpR.quit();
	}
}