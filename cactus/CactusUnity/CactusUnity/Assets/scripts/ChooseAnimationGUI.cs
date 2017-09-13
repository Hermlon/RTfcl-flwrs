using UnityEngine;
using System.Collections;

public class ChooseAnimationGUI : MonoBehaviour {

	private LEDMatrix ledm;

	// Use this for initialization
	void Start () {
		BMLParser bmlParser = new BMLParser();
		Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/c1_mr.bml");/*
		ledm = new LEDMatrix();
		ledm.Init();
		ledm.playAnimation(a1);
		Debug.Log(a1.intToBinaryString (12));*/
		//AnimationUDPManager udp = new AnimationUDPManager ("192.168.0.100", 2390);
		AnimationUDPManager udp = new AnimationUDPManager ("192.168.0.125", 2390);
		udp.playAnimation (a1, AnimationUDPManager.MATRIX_EYER);
	}

	// Update is called once per frame
	void Update () {
		//ledm.Update();
	}
}
