using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000; // the texture's order in the draw hierarchy, make sure it's the last thing
	private float alpha = 1.0f;
	private int fadeDir = -1; // the dir for fade: in = -1 or out = 1



	void OnGUI(){
		// fade out/in the alpha value using a direction, a speed and Time.deltatime to convert the operation to seconds.
		alpha += fadeDir * fadeSpeed * Time.deltaTime;

		// force the number between 0 and 1 bc GUI.color use aplha values between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// set color of our GUI(in this case our texture), all color values remain the same & the Alpha is set to the alpha var
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha); // set the alpha
		GUI.depth = drawDepth;	// make the black texture render on top
		GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture ); // draw the texture to fit the entire screen area

	}

	// sets fadeDir to the direction param making the scene fade in if -1 / out if 1
	public float BeginFade( int direction ){
		fadeDir = direction;
		return (fadeSpeed);
	}

	// OnLevelWasLoaded is called when a level is loaded. It takes loaded level index (int) as a param so you can limit the fade in to certain scenes
	void OnLevelWasLoaded(){
		// alpha = 1;  //use this if alpha is not set to 1 by default
		BeginFade (-1);	// call the fade in direction

	}
}
