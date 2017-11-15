using UnityEngine;
using System.Collections;
using System;

public class ChooseAnimationGUI : MonoBehaviour {

	private LEDMatrix ledm;
	private AnimationUDPManager udp;


	// Use this for initialization
	void Start () {
		
		ledm = new LEDMatrix();
		ledm.Init();
		BMLParser bmlParser = new BMLParser();
		Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/c1_ml.bml");
		ledm.playAnimation(a1);
		//AnimationUDPManager udp = new AnimationUDPManager ("192.168.0.100", 2390);
		//AnimationUDPManager udp = new AnimationUDPManager ("192.168.0.125", 2390);
		//udp.playAnimation (a1, AnimationUDPManager.MATRIX_EYER);
		//udp.SendMsg("drfpighfhj");

		//udp = new AnimationUDPManager ("192.168.0.100", 2390);
		//udp = new AnimationUDPManager ("127.0.0.1", 57514);
	}

	// Update is called once per frame
	void Update () {
		//ledm.Update();
	}

	void OnGUI()
	{
		//Only in Editor / Development
		if (GUI.Button (new Rect (10, 10, 150, 30), "Click to Stop Threads")) {
			Debug.Log ("You clicked the button! Threads will stop and Unity won't crash in editor!");
			udp.quit();
		}

		if (GUI.Button (new Rect (10, 50, 150, 30), "Play Animation 1")) {
			BMLParser bmlParser = new BMLParser();
			Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/c1_ml.bml");
			udp.playAnimation (a1, AnimationUDPManager.MATRIX_EYER);
		}

		if (GUI.Button (new Rect (10, 90, 150, 30), "Play Animation 2")) {
			BMLParser bmlParser = new BMLParser();
			Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/b1_el.bml");
			udp.playAnimation (a1, AnimationUDPManager.MATRIX_EYER);
		}

		if (GUI.Button (new Rect (10, 130, 150, 30), "Play Animation 3")) {
			BMLParser bmlParser = new BMLParser();
			Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/b1_er.bml");
			udp.playAnimation (a1, AnimationUDPManager.MATRIX_EYER);
		}
	}

}
