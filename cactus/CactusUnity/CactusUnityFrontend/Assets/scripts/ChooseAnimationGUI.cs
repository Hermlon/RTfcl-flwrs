using UnityEngine;
using System.Collections;

public class ChooseAnimationGUI : MonoBehaviour {

	private LEDMatrix ledm;

	// Use this for initialization
	void Start () {
		BMLParser bmlParser = new BMLParser();
		Animation a1 = bmlParser.loadAnimation("Assets/animations_bml/left_eye_of_a_cactus.bml");
		Debug.Log(a1.getName());

		ledm = new LEDMatrix();
		ledm.Init();
		ledm.playAnimation(a1);
	}

	// Update is called once per frame
	void Update () {
		ledm.Update();
	}
}
