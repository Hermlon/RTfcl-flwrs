using UnityEngine;
using System.Collections;

public class MakeDots : MonoBehaviour {

	public Texture2D an;
	public Texture2D aus;

	public Texture2D icon;

	public GameObject dummy;

	// Use this for initialization
	void Start () {
		//icon = an;
		//icon = new Texture2D(512, 512);

		Color myColor;
		for (int y = 0; y < 64; y++) {
			for (int x = 0; x < 64; x++) {
				myColor = Color.red;//an.GetPixel (x, y);
				icon.SetPixel (x, y, myColor);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		dummy.GetComponent<Renderer> ().material.mainTexture = icon;
	}
}
