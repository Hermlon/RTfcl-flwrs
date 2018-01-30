using UnityEngine;
using System.Collections;

public class MenuTest : MonoBehaviour {

	public Vector2 scrollPosition = Vector2.zero;
	public GUIStyle style;

	private Texture2D[,] image;

	GUIContent content = new GUIContent ();

	public float duration = 0.5f;
	private float durationCounter = 0.0f;

	private int actualFrame = 0;

	public void fillArray(Texture2D[] t) {
		this.image = t;
	}

	void OnGUI() {
		GUI.skin.scrollView = style;
		scrollPosition = GUI.BeginScrollView(new Rect(10, 300, 120, 100), scrollPosition, new Rect(0, 0, 100, 200));

			for (int i = 0; i < image.Length; i++) {
				GUI.Button(new Rect(0, i * 45, 40, 40), image[i]);
			}
		GUI.EndScrollView();
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