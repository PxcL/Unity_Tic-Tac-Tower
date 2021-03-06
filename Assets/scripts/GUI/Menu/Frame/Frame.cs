using UnityEngine;
using System.Collections;

public abstract class Frame : MenuContent{
	public string title;
	protected Rect buttonSize = new Rect(0,0,300,100);
	protected Rect position = new Rect(200,100,400,800);
	protected int border = 20;
	
	protected int spacing = 200; 
	protected int maxSpacing = 200;
	protected int minSpacing = 50;
	
		
	public abstract void AddButton( MenuButton button);
	
	public static Frame Create(string title){
		# if UNITY_WEBPLAYER
			return new DoubleFrame(title,25,new Rect(25,40,Screen.width-50,Screen.height-80));
		# elif UNITY_ANDROID
			return new SingleFrame(title,25,new Rect(25,40,Screen.width-50,Screen.height-80));		
		# else
			return new DoubleFrame(title,25,new Rect(25,40,Screen.width-50,Screen.height-80));		
		# endif
	}
	
}
