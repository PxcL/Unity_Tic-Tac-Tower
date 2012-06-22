using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	Vector2 scrollPos = new Vector2(0,0);
	public GUISkin skin;
	new public Transform camera;
	public int border = 0;

	private Control control;
	
	//camera positions
	
	int buttonWidth = 100;
	int buttonHeight = 40;
	
	//private Vector3 camPreviewPos = new Vector3(-30,150,35);
	//private Vector3 camPreviewRot = new Vector3(90,0,0);
	
	//event points
	
	private enum Chapter {menu, intro, exampleStr, tutStr, exampleDiag, tutDiag, end};
	private Chapter chapter;
	private TowerType towerTut;	//Used to know which tutorial is run. TowerType.five is used when none is run.
	
	private SkillDescription buildDescr = new SkillDescription(TowerType.build);
	private SkillDescription shootDescr = new SkillDescription(TowerType.shoot);
	private SkillDescription silenceDescr = new SkillDescription(TowerType.silence);
	private SkillDescription skillDescr = new SkillDescription(TowerType.skillCap);
	
	//private int section = 0;
	
	private int menuWidth = 300;
	private int menuChangeSpeed = 50; //100px/2sec
	
	//text
	private string introText = "this is a scroll text \n\n\n\n\n\n\n this is further down\n\n\n\n\nthis is even further down\n\n this sentence is so long only because I wanted to see if the line is clipped or not. Probably it is clipped, and that would make me kinda sad.";
	
	void Start () {
		control = (Control)FindObjectOfType(typeof(Control));
		control.StartNewGame();
		chapter = Chapter.menu;
		towerTut = TowerType.five;
	}
	void Update(){
		//if(changeChapter && chapter == 0 && menuWidth > 200){
		//	menuWidth -= Mathf.RoundToInt(Time.deltaTime*menuChangeSpeed);
		//}
	}
	
	void OnGUI(){
		
		GUI.skin = skin;
		switch(chapter){
		case Chapter.menu:
			PrintMenu();
			//PrintTextWindow();
			break;
		case Chapter.intro:
			PrintSkillInfoWindow();
			switch(towerTut){
			case TowerType.build:
				//For Build Skill Tutorial.
				break;
			case TowerType.shoot:
				//For Shoot Skill Tutorial.
				break;
			case TowerType.silence:
				//For Silence Skill Tutorial.
				break;
			case TowerType.skillCap:
				//For Skill Skill Tutorial.
				break;
			default:
				Debug.LogError("Tried to access invalid skill tutorial");
				break;
			}		
			break;
		case Chapter.exampleStr:
			control.StartNewGame();
			switch(towerTut){
			case TowerType.build:
				//For Build Skill Tutorial.
				Stats.SetTutorialBuild1();
				break;
			case TowerType.shoot:
				//For Shoot Skill Tutorial.
				break;
			case TowerType.silence:
				//For Silence Skill Tutorial.
				break;
			case TowerType.skillCap:
				//For Skill Skill Tutorial.
				break;
			default:
				Debug.LogError("Tried to access invalid skill tutorial");
				break;
			}		
			break;
		case Chapter.tutStr:
			switch(towerTut){
			case TowerType.build:
				//For Build Skill Tutorial.
				break;
			case TowerType.shoot:
				//For Shoot Skill Tutorial.
				break;
			case TowerType.silence:
				//For Silence Skill Tutorial.
				break;
			case TowerType.skillCap:
				//For Skill Skill Tutorial.
				break;
			default:
				Debug.LogError("Tried to access invalid skill tutorial");
				break;
			}
			break;
		case Chapter.exampleDiag:
			switch(towerTut){
			case TowerType.build:
				//For Build Skill Tutorial.
				break;
			case TowerType.shoot:
				//For Shoot Skill Tutorial.
				break;
			case TowerType.silence:
				//For Silence Skill Tutorial.
				break;
			case TowerType.skillCap:
				//For Skill Skill Tutorial.
				break;
			default:
				Debug.LogError("Tried to access invalid skill tutorial");
				break;
			}
			break;
		case Chapter.tutDiag:
			switch(towerTut){
			case TowerType.build:
				//For Build Skill Tutorial.
				break;
			case TowerType.shoot:
				//For Shoot Skill Tutorial.
				break;
			case TowerType.silence:
				//For Silence Skill Tutorial.
				break;
			case TowerType.skillCap:
				//For Skill Skill Tutorial.
				break;
			default:
				Debug.LogError("Tried to access invalid skill tutorial");
				break;
			}
			break;
		case Chapter.end:
			switch(towerTut){
			case TowerType.build:
				//For Build Skill Tutorial.
				break;
			case TowerType.shoot:
				//For Shoot Skill Tutorial.
				break;
			case TowerType.silence:
				//For Silence Skill Tutorial.
				break;
			case TowerType.skillCap:
				//For Skill Skill Tutorial.
				break;
			default:
				Debug.LogError("Tried to access invalid skill tutorial");
				break;
			}
			break;
		}
		
	}
	
		
	void PrintMenu(){
		//deler skjermen i tre like deler:
		//int buttonWidth = 100;
		//int buttonHeight = 40;
		//int b1Start = (Screen.width-2*buttonWidth)/3;
		//int b2Start = 2*b1Start + buttonWidth;
		//int b1Start = (Screen.width-buttonWidth)/2;
		//int b2Start = Screen.width-buttonWidth;
		if(GUI.Button(new Rect(Screen.width/2-buttonWidth/2, 200, buttonWidth, buttonHeight), "Build Tower")){
			Stats.SetDefaultSettings();
			Stats.SetTutorialBuild1();
			chapter = Chapter.intro;
			towerTut = TowerType.build;	//Build Skill Tutorial
			//chapter = Chapter.exampleStr; //Brukes for å fort komme til det kapittelet man selv ønsker (for debug/testing).
		}
		
		if(GUI.Button(new Rect(Screen.width/2-buttonWidth/2, 250, buttonWidth, buttonHeight), "Shoot Build")){
			Stats.SetDefaultSettings();
			Stats.SetTutorialBuild1();
			chapter = Chapter.intro;
			towerTut = TowerType.shoot;	//Shoot Skill Tutorial
		}
		
		if(GUI.Button(new Rect(Screen.width/2-buttonWidth/2, 300, buttonWidth, buttonHeight), "Silence Build")){
			Stats.SetDefaultSettings();
			Stats.SetTutorialBuild1();
			chapter = Chapter.intro;
			towerTut = TowerType.silence;	//Silence Skill Tutorial
		}
		
		if(GUI.Button(new Rect(Screen.width/2-buttonWidth/2, 350, buttonWidth, buttonHeight), "Skill Build")){
			Stats.SetDefaultSettings();
			Stats.SetTutorialBuild1();
			chapter = Chapter.intro;
			towerTut = TowerType.skillCap;	//Skill cap Skill Tutorial
		}
		
		if(GUI.Button(new Rect(Screen.width/2-buttonWidth/2, Screen.height-buttonHeight-border, buttonWidth, buttonHeight), "Main Menu")){
			Stats.SetDefaultSettings();
			Stats.SetTutorialBuild1();
			Application.LoadLevel("mainMenu");
		}
		
	}
	

	private void PrintTextWindow(){

		GUI.BeginScrollView(new Rect(0,100,menuWidth, 500),scrollPos, new Rect(0,0,menuWidth,800));
		GUI.Box(new Rect(0,0,menuWidth,800),introText,"darkbox");
		GUI.EndScrollView();
	}
	
	private void PrintSkillInfoWindow(){
		//GUI.BeginScrollView(new Rect(0,100,200, 500),scrollPos, new Rect(0,0,200,800));
		buildDescr.PrintGUI();
		if(GUI.Button(new Rect(Screen.width/2-buttonWidth/2, 200, buttonWidth, buttonHeight), "Continue")){
			chapter = Chapter.exampleStr;
		}
		//GUI.EndScrollView();
	}
	/*
	private void SimpleChangeChapter(){
		Debug.Log("simple change chapter");
		camera.animation.Play("anim1");
		chapter++;
	}
	
	private IEnumerator ChangeChapter(){
		//check for end of section
		Debug.Log("changing chapter...");
		changeChapter = true;

		switch(chapter){
		case 0:
			camera.animation.Play("anim1");			
			break;
		case 1:
			camera.animation.Play("anim2");	
			GUI_script guis = (GUI_script)gameObject.GetComponent(typeof(GUI_script));
			guis.enable = true;
			break;
		case 2:

			break;
		}

		yield return new WaitForSeconds(2);
		changeChapter = false;
		chapter++;
	}
	 */
}
