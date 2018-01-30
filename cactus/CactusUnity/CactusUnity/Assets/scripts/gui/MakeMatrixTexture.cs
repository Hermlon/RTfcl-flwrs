using UnityEngine;
using System.Collections;
using System.IO;

public class MakeMatrixTexture {

	public Texture2D pointOn;
	public Texture2D pointOff;

	private Texture2D pointMatrix;

	public GameObject testCube;

	//private string fileName = "c1_ml";

	public MakeMatrixTexture(Texture2D on, Texture2D off) {
		this.pointOn = on;
		this.pointOff = off;
	}

	// Use this for initialization
	public void createTextures(string fileName) {
		BMLParser bmlParser = new BMLParser ();
		Animation animation = new Animation();

		animation = bmlParser.loadAnimation (Application.dataPath + "/animations_bml/" + fileName);

		Color copyColor;
		pointMatrix = new Texture2D (512, 512);

		// Pixel der Anzeigematrix leuchtet/leuchtet nicht
		bool pixel;

		Debug.Log (Color.green.ToString());

		for(int frameCount = 0; frameCount < animation.getSize(); frameCount++)
		{
			Debug.Log ("Frame " + frameCount);
			// Verarbeite alle 8*8 Pixel der Matrix
			for (int xCount = 0; xCount < 8; xCount++) {//8
				for (int yCount = 0; yCount < 8; yCount++) {//8	
					pixel = animation.getFrame (frameCount).pixels [xCount, yCount];

					// Kopiere Pixel vom An-/Ausbild in die große Textur
					for (int yPixel = 0; yPixel < 64; yPixel++) {//64
						for (int xPixel = 0; xPixel < 64; xPixel++) {//64
							if (pixel) {
								copyColor = pointOn.GetPixel (xPixel, yPixel);
								//copyColor = Color.red;
							} else {
								copyColor = pointOff.GetPixel (xPixel, yPixel);
								//copyColor = Color.green;
							}
							// Kopiere den Pixel aus dem kleinen Bild in das große an die richtige Stelle
							pointMatrix.SetPixel (xCount * 64 + xPixel, (7-yCount) * 64 + yPixel, copyColor);
						}
					}
				}
			}
				
			// Alle Pixel auf die Textur anwenden
		pointMatrix.Apply ();

			// Testweise fertige Textur auf dem Würfel zeigen
		//testCube.GetComponent<Renderer> ().material.mainTexture = (Texture)pointMatrix;


			// ferige Textur abspeichern
			// in PNG-Format kodieren
			byte[] bytes = pointMatrix.EncodeToPNG();
			// fertige Textur in Projektordner schreiben
			File.WriteAllBytes(Application.dataPath + "/Resources/animations/" + fileName + "_" + frameCount + ".png", bytes);
		}
	}
}
