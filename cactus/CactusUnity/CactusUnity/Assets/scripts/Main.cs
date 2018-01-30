using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Main : MonoBehaviour {

	public Texture2D pointOn;
	public Texture2D pointOff;

	// Use this for initialization
	void Start () {
		//Delete old animation pictures
		DirectoryInfo oldanimations = new DirectoryInfo(Application.dataPath + "/Resources/animations/");
		foreach (FileInfo file in oldanimations.GetFiles()) {
			file.Delete ();
		}
		foreach (DirectoryInfo d in oldanimations.GetDirectories()) {
			d.Delete (true);
		}

		//Get list of available animations
		MakeMatrixTexture mmt = new MakeMatrixTexture(pointOn, pointOff);
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/animations_bml/");
		FileInfo[] info = dir.GetFiles("*.bml");
		foreach (FileInfo f in info)
		{
			mmt.createTextures (f.Name);
		}

		//Load images to Scroller

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
