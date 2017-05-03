using System.Collections.Generic;

public class Animation {
	private string name;
	private List<Frame> frames = new List<Frame>();

  public void setName(string name) {
    this.name = name;
  }

  public string getName() {
    return this.name;
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
	/*
	public string getByteText(int frame) {
		string content = "";
		content += getFrame (frame).duration;
			
		getFrame(frame)
	}*/
	public string getByteTextFileName(int frame) {
		return name + "_" + frame + ".txt";
	}

	public string intToBinaryString(int number)
	{
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
		return stringBinaryNumber.ToString();//Doesnt work...
	}

}
