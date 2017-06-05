using UnityEngine;
using System.Collections;

public class ChooseAnimationGUI : MonoBehaviour {

	private LEDMatrix ledm;

	// Use this for initialization
	void Start () {/*
		BMLParser bmlParser = new BMLParser();
		Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/d1_el.bml");
		ledm = new LEDMatrix();
		ledm.Init();
		ledm.playAnimation(a1);
		Debug.Log(a1.intToBinaryString (12));*/
		UDPSender udp = new UDPSender ("192.168.0.100", 2390);
		udp.SendMsg("hallo");
	}

	// Update is called once per frame
	void Update () {
		//ledm.Update();
	}
}
