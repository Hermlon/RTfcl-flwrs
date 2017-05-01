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
}
