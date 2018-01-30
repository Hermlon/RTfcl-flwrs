using System.Collections;
using System.Xml;
﻿using UnityEngine;

public class BMLParser {

	public Animation loadAnimation(string filename) {
		Animation a = new Animation();
		XmlDocument doc = new XmlDocument();
		try {
			doc.Load(filename);
		}
		catch(XmlException e) {
			Debug.LogError (e.StackTrace);
		}
		XmlNode root = doc.DocumentElement;
		XmlNode header = root.SelectSingleNode("header");
		XmlNode title = header.SelectSingleNode("title");
		a.setName(title.InnerText.Remove(title.InnerText.Length - 1));

		XmlNodeList nodeList = root.SelectNodes ("frame");
		foreach (XmlNode node in nodeList) {
			Frame f = new Frame ();
			f.pixels = new bool[8,8];
			f.duration = System.Convert.ToInt32(node.Attributes["duration"].Value);
			XmlNodeList rows = node.SelectNodes("row");

			string b = "";
			for(int i = 0; i < 8; i ++) {
				char[] pix = rows[i].InnerText.ToCharArray();
				for(int e = 0; e < 8; e ++) {
					if(pix[e] == '1') {
						f.pixels[e,i] = true;
					}
					else {
						f.pixels[e,i] = false;
					}
				}
					

				string g = "";
				for (int t = 0; t < 8; t++) {
					if (f.pixels [t, i]) {
						g += "1";
					} else {
						g += "0";
					}
				}
				b = b + g + "\n";
			}

			Debug.Log ("_____________________");
			Debug.Log (b);
			a.addFrame(f);
		}
		return a;
	}
}
