using UnityEngine;
using System.Collections;

public class GUI_script : MonoBehaviour {

	private Control control;
	private Grid grid;
	//private string[] skillNames =new string[4] {"cancel", "shoot", "build", "emp"};
	
	public Texture[] tSkills;
	public Texture smallArrowUp;
	public Texture smallArrowDown;
	private int showSkillInfo = 0; //0 = Reveals no info.
	
	public GUISkin customSkin;
	
	public bool enable;
	public bool lockGUI;
	
	private string buildText = "Build Tower: \nAllows the player to place one more piece on the board. This will not, however, reset the amount of skills used, as if starting a new round.";
	private string shootText = "Shoot Tower: \nThe player may destroy another unused, hostile piece on the board. The piece is ruined, and the tile cannot be built upon.";
	private string empText = "EMP Tower: \nThe opponent is rendered unable to place a piece where he/she would normally be able to build a tower. Also, the opponent will not benefit from any abilities next turn.";
	private string squareText = "Square Tower: \nIncreases the skill cap by one for the player who builds it, allowing the player to use the same skill one more time during the same round. In addition, the player will gain five score points at the end of each turn.";
	
//	private Console theConsole;
	
	private bool towerRow; // whether the straight or diagonal towers shall be shown	
	private bool confirmNewGame = false;
		
	private UndoButton undobutton;
	private ClockGUI clockGui = new ClockGUI();
	
	void Start () {
		control = (Control)FindObjectOfType(typeof(Control));
		grid = (Grid)FindObjectOfType(typeof(Grid));
		undobutton = new UndoButton(control);
		//enable = true;
		//lockGUI = false;
	}
	
	void OnGUI() {
		if(!enable){
			return;
		}
		GUI.enabled = !lockGUI;
		
		GUI.skin = customSkin;
		
		clockGui.PrintGUI();
		
		PopupMessage.PrintGUI();
		
		TextInfo();
		
		EndTurn();
		
		SkillOverview();
		
		SkillDescrDropdown();
		
		NewGameMenu();
				
		Console.PrintGUI();
		
		undobutton.PrintGUI();
		
		//----Framework to handle mouse-input etc----//
		GUI.enabled = true;

		if(Event.current.type == EventType.MouseDown){
			if( Event.current.type != EventType.Used ){
				grid.MouseDown(Input.mousePosition);
			}
		}
	}
	
	private void EndTurn(){
		//End Turn.
		if(control.playerDone && Stats.gameRunning && GUI.Button( new Rect(8, Screen.height-40,100, 40), "End Turn")){
			control.UserEndTurn();
		}	
	}
	
	private void TextInfo(){
		string p1SkillInfo = "Player 1 skills\nShoot: "+Control.cState.player[0].playerSkill.shoot+
				"\nBuild: "+Control.cState.player[0].playerSkill.build+"\nEMP: "+Control.cState.player[0].playerSkill.emp+
				"\nSkill cap: "+(int)(1+Control.cState.player[0].playerSkill.square+Control.cState.globalSkillCap);
		string p2SkillInfo = "Player 2 skills\nShoot: "+Control.cState.player[1].playerSkill.shoot+
				"\nBuild: "+Control.cState.player[1].playerSkill.build+"\nEMP: "+Control.cState.player[1].playerSkill.emp+
				"\nSkill cap: "+(int)(1+Control.cState.player[1].playerSkill.square+Control.cState.globalSkillCap);
		
		GUI.Box(new Rect(0,0,90,100), p1SkillInfo);
		GUI.Box(new Rect(Screen.width - 90,0,90,100), p2SkillInfo);
		
		GUI.Box(new Rect(0,110,90,45), "Player 1 \n score: " +Control.cState.player[0].score);
		GUI.Box(new Rect(Screen.width-90,110,90,45), "Player 2 \nscore: " +Control.cState.player[1].score);
		
		//Turns until skill cap increase.
		if(Stats.rules == Stats.Rules.SOLID_TOWERS){
			if(Control.cState.placedPieces <= Stats.totalArea/3){
				GUI.Box(new Rect(Screen.width/2-200, 0, 400, 25), "Player " + (Control.cState.activePlayer+1) + "'s turn.  " 
						+ "Skill cap: " + (int)(1+Control.cState.globalSkillCap) + ".   Skill cap increase after: " 
						+ (int)(Stats.totalArea/3+1-Control.cState.placedPieces) + " tiles.");
			}else if(Control.cState.placedPieces <= 2*Stats.totalArea/3){
				GUI.Box(new Rect(Screen.width/2-200, 0, 400, 25), "Player " + (Control.cState.activePlayer+1) + "'s turn.  " 
						+ "Skill cap: " + (int)(1+Control.cState.globalSkillCap) + ".   Skill cap increase after: " 
						+ (int)(Stats.totalArea*2/3+1-Control.cState.placedPieces) + " tiles.");
			}else{
				GUI.Box(new Rect(Screen.width/2-200, 0, 400, 25), "Player " + (Control.cState.activePlayer+1) + "'s turn.  " 
						+ "Skill cap: " + (int)(1+Control.cState.globalSkillCap) + ".   Total skill cap increases reached");
			}	
		}else if( Stats.rules == Stats.Rules.INVISIBLE_TOWERS){
			GUI.Box(new Rect(Screen.width/2-200, 0, 400, 25), "Player " + (Control.cState.activePlayer+1) + "'s turn.  " 
						+ "Skill cap: " + (int)(1+Control.cState.globalSkillCap) + ".   Total skill cap increases reached");
		}
	}
	
	private void SkillOverview(){
		GUI.Box(new Rect(Screen.width/2-200, 30, 400, 100), "");

		//tanke for � switche bilder:
		//legge til 4 p� telleren, og deretter h�ndtere skillInUse
		//kan caste boolen til en int og gange med 4 for � lage generell formel
		for(int i = 0; i<3; i++){
			if(Skill.skillInUse != (i+1) && GUI.Button(new Rect(Screen.width/2-200+i*100, 30, 100, 100),tSkills[i+Bool2Int(towerRow)*5]) ){
				//the user has pressed a skill-button
				UseSkillError(Skill.UseSkill(i+1));
			}else if( Skill.skillInUse == i+1 ){
				GUI.Box(new Rect(Screen.width/2-200+i*100, 30, 100, 100),tSkills[i+Bool2Int(towerRow)*5]);
			}
		}
		
		
		
		if(Skill.skillInUse == 0){
			GUI.Box(new Rect(Screen.width/2+100, 30, 100, 100), tSkills[3+Bool2Int(towerRow)*5]);
		}else{
			if(GUI.Button(new Rect(Screen.width/2+100, 30, 100, 100), tSkills[4])){
				Skill.UseSkill(0);
			}
		}
		towerRow = GUI.Toggle(new Rect(Screen.width/2-225,105,25,25),towerRow, "","towerDispToggle");
		
		switch(showSkillInfo){
			case 0:
				break;
			case 1:
				GUI.Box(new Rect(Screen.width/2-218, 144, 436, 70),shootText, "darkBox");
				break;
			case 2:
				GUI.Box(new Rect(Screen.width/2-218, 144, 436, 70),buildText, "darkBox");
				break;
			case 3:
				GUI.Box(new Rect(Screen.width/2-218, 144, 436, 70),empText, "darkBox");
				break;
			case 4:
				GUI.Box(new Rect(Screen.width/2-218, 144, 436, 70),squareText, "darkBox");
				break;
		}		
		
	}
	
	private void SkillDescrDropdown(){
		for(int i = 1;i<5;i++){
			if(showSkillInfo != i){ 
				if(GUI.Button(new Rect(Screen.width/2-270+i*100, 130, 40, 15),smallArrowDown)){
					showSkillInfo = i;
				}
			}else{ 
				if(GUI.Button(new Rect(Screen.width/2-270+i*100, 130, 40, 15),smallArrowUp)){
				showSkillInfo = 0;
				}
			}
		}		
	}
	
	private void NewGameMenu(){
		if( !confirmNewGame && GUI.Button(new Rect(Screen.width - 100, Screen.height - 280, 100, 40), "NEW GAME")){
			confirmNewGame = true;
		}else if(confirmNewGame){
			GUI.Box(new Rect(Screen.width - 100, Screen.height - 280, 100, 40), "NEW GAME");
			if(GUI.Button(new Rect(Screen.width - 110, Screen.height - 240, 55, 25), "confirm")){
				Application.LoadLevel("mainMenu");
				confirmNewGame = false;
			}else if(GUI.Button(new Rect(Screen.width - 55, Screen.height - 240, 55, 25), "cancel")){
				confirmNewGame = false;
			}
		}		
	}
	
	private int Bool2Int(bool b){
		int i;
		if(b){
			i = 1;
		}else{
			i = 0;
		}
		return i;
	}
	
	/*
	private void PlacePieceError(int errorCode, Rect pos){
		switch(errorCode){
			case 3:
				if(Time.time < errorStartTime + errorDisplayTime){
					GUI.Box(pos, "Cannot build towers while being silenced", darkTextBoxes);
				}
				return;	
		}
		return;
	*/
	
	private void UseSkillError(SkillSelectError errorCode){
		switch(errorCode){
			case SkillSelectError.NO_ERROR:
				return;
			case SkillSelectError.SKILL_AMMO_ERROR:
				PopupMessage.DisplayMessage("You dont have towers for that skill");
				return;
			case SkillSelectError.SKILL_CAP_ERROR:
				PopupMessage.DisplayMessage("Cannot use that skill that many times");
				return;
			case SkillSelectError.UNKNOWN_ERROR:
				PopupMessage.DisplayMessage("Unknown error occured");
				return;
		}
		return;
	}
	
}
