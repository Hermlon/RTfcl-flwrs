using UnityEngine;
using System.Collections;

public class IconAnimator : MonoBehaviour {

	GUIContent content = new GUIContent ();
	public Texture2D[] image;
	public string text = "";

	public float duration = 0.5f;
	private float durationCounter = 0.0f;

	private int actualFrame = 0;

	void Awake()
	{
		content.image = (Texture2D)image[actualFrame];
		content.text = text;
	}

	void OnGUI()
	{
		GUI.skin.button.normal.background = (Texture2D)content.image;

		if (GUI.Button (new Rect (10, 10, 74, 74), content)) {
			// Nix
		}
	}

	void Update()
	{
		durationCounter += Time.deltaTime;
		if (durationCounter > duration) {
			if (actualFrame < image.Length - 1) {
				actualFrame++;
			} else {
				actualFrame = 0;
			}
			durationCounter = 0.0f;
			Debug.Log (actualFrame);
			content.image = (Texture2D)image[actualFrame];
		}
	}

}
