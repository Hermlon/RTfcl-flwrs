using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class Animation {
	private string name;
	private List<Frame> frames = new List<Frame>();

  public void setName(string name) {
    this.name = name;
  }

  public string getName() {
    return this.name;
  }

	public int getSize() {
		return frames.Count;
	}

  public void addFrame(Frame frame) {
    this.frames.Add(frame);
  }

	public Frame getFrame(int i) {
		if(i < frames.Count) {
			return frames[i];
		}
		else {
			return null;
		}
	}

	public string getByteText(int frame) {
		Frame f = getFrame (frame);
		string content = "";
		content += intToBinaryString(f.duration);
		for (int y = 0; y < 8; y++) {
			for(int x = 0; x < 8; x ++) {
				if (f.pixels [y, x]) {
					content += "1";
				} else {
					content += "0";
				}
			}
		}
		return content;
	}
	public string getByteTextFileName(int frame) {
		return name + frame + ".txt";
	}

	public string intToBinaryString(int number)
	{
		/*
		char[] stringBinaryNumber = {'0','0','0','0','0','0','0','0'};
		//rest = number;
		for (int i = 7; i >=0; i--)
		{
			if (number <= (2^i)) {
				number = number - (2^i);
				stringBinaryNumber[i]='1';
			}
			else
				stringBinaryNumber[i]='0';
		}
		return new string (stringBinaryNumber);*/
		string r = "";
		string res = Convert.ToString (number, 2);
		int m = 8 - res.Length;
		for (int t = 0; t < m; t++) {
			r += "0";
		}
		return r +res;
	}

}
