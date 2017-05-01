using UnityEngine;

public class LEDMatrix {
  private GameObject[,] pixels = new GameObject[8,8];
  private Texture texture_enabled = (Texture) Resources.Load("pixel_enabled");
  private Texture texture_disabled = (Texture) Resources.Load("pixel_disabled");
  private Animation currentAnimation = null;
  private int currentFrame = 0;
  private int remainingTime = 0;

  // Use this for initialization
  public void Init () {
    for(int i = 0; i < 8; i ++) {
      for(int e = 0; e < 8; e ++) {
        pixels[i,e] = GameObject.CreatePrimitive(PrimitiveType.Plane);
        pixels[i,e].transform.Rotate(-90,0,0);
    		pixels[i,e].transform.localScale = new Vector3(0.09f, 1.0f, 0.09f);
    		pixels[i,e].transform.Translate(new Vector3(i * 1.0f - 4, 0.0f, e * 1.0f - 3));
     		pixels[i,e].GetComponent<Renderer>().material.mainTexture = texture_disabled;
    		pixels[i,e].GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
      }
    }
	}

  private void showFrame(Frame f) {
    for(int i = 0; i < 8; i ++) {
      for(int e = 0; e < 8; e ++) {
        if(f.pixels[i,e] == true) {
          pixels[i,e].GetComponent<Renderer>().material.mainTexture = texture_enabled;
        }
        else {
          pixels[i,e].GetComponent<Renderer>().material.mainTexture = texture_disabled;
        }
      }
    }
    remainingTime = f.duration;
  }

  public void playAnimation(Animation a) {
    currentAnimation = a;
    currentFrame = 0;
    remainingTime = 0;
  }

	// Update is called once per frame
  public void Update () {
    if(currentAnimation != null) {
      if(remainingTime == 0) {
        Frame nextFrame = currentAnimation.getFrame(currentFrame);
        if(nextFrame != null) {
        }
        else {
          //No more frames -> replay animation
          currentFrame = 0;
          nextFrame = currentAnimation.getFrame(currentFrame);
        }
        Debug.Log("Showing frame_: " + currentFrame);
        showFrame(nextFrame);
        currentFrame ++;
      }
      remainingTime --;
    }
    }
}
