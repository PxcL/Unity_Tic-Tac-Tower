using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control: MonoBehaviour {

	public bool playerDone = false; //Is true when player has placed a piece. Allows user to "End Turn".	
	public static GameState cState; //the current gamestate of the progressing game
	public static GameState startOfTurn; //the undo-point
	
	public Transform towerBuildEffect;
	
	private Turn activeTurn;
	private Sound sound;
	private GraphicsCenter graphicsCenter;
	
	void Awake () {
		sound = (Sound)FindObjectOfType(typeof(Sound));
		graphicsCenter = (GraphicsCenter)FindObjectOfType(typeof(GraphicsCenter));
		if (sound == null){
			Debug.LogError("sound-object not found");
		}
		if (graphicsCenter == null){
			Debug.LogError("graphicsCenter-object not found");
		}
		Console.Init(this);
	}
	
	void Start(){
		StartNewGame();
	}
	
	public void UserFieldSelect(FieldIndex index){
		//Called when the user clicks on the field
		Order o = new Order();
		o.endTurn = false;
		switch(Skill.skillInUse){
			case 0:
				o.skill = SkillType.place;
				o.position = index;
				break;
			case 1:
				o.skill = SkillType.shoot;
				o.position = index;
				break;
			case 2:
				o.skill = SkillType.build;
				o.position = index;
				break;
			case 3:
				o.skill = SkillType.emp;
				break;
		}
		ExecuteOrder(o);
	}
	
	public void UserEndTurn(){
		Order o = new Order();
		o.skill = SkillType.noSkill;
		o.endTurn = true;
		ExecuteOrder(o);
	}
	
	private bool CheckCluster(FieldIndex index){ //rename?
		//Finds a cluster from a field index recursively
		//calls appropriate FindTower-functions on this cluster
		//reports found towers
		//returns true if a piece was placed
		
		// **DEBUG** tilpasse funksjonalitet til silence
		Field<bool> cluster = new Field<bool>(false); 

		cluster = Tower.FindAllClusterRecurse(index,cluster);
		List<Tower> tower = Tower.FindTower(cluster);
		

//			Debug.Log(activePlayer + " silenced: " + player[activePlayer].silenced);
		if(!cState.player[cState.activePlayer].silenced){
			cState.player[cState.activePlayer].AddScore(tower.Count);
			foreach( Tower t in tower){
				//Checking for victory

				//Coloring the towers:
				foreach(FieldIndex i in t.GetList()){ 
					
					if(Stats.rules == Stats.Rules.SOLID_TOWERS || t.type == TowerType.five){
						cState.field[i] = Field<int>.GetDarkRoute(cState.field[i]);
					}else if(Stats.rules == Stats.Rules.INVISIBLE_TOWERS){
						cState.field[i] = Route.empty;
					}
				}
				if(t.type == TowerType.five){
					Victory();
				}
				//adding skills:
				ReportTower(t);
			}
			graphicsCenter.BuildingConstructionEffect(tower);
		}else if(tower.Count > 0){ //if a tower was found that was blocked by Silence
			return false;
		}
		BroadcastMessage("UpdateField");
		return true;
	}
	
	private void Victory(){
		sound.PlaySound(SoundType.victory);
		cState.player[cState.activePlayer].score += 1000;
		Console.PrintToConsole("Player "+(cState.activePlayer+1)+" has won!",Console.MessageType.INFO);
		//active player has won!
		//TODO: victory-screen
	}
	
	public bool ExecuteOrder(Order o){
		// Executes an order from the order-format
		bool validMove = false;
		switch(o.skill){
		case SkillType.noSkill:
			validMove = true;
			break;
		case SkillType.place:
			validMove = PlacePiece(o.position);
			break;
		case SkillType.shoot:
			if(Skill.CanUseShoot() == SkillSelectError.NO_ERROR){
				validMove = Shoot(o.position);
			}else{
				validMove = false;
			}
			break;
		case SkillType.build:
			if(Skill.CanUseBuild() == SkillSelectError.NO_ERROR){
				validMove = ExtraBuild(o.position);
			}else{
				validMove = false;
			}
			break;
		case SkillType.emp:
			if(Skill.CanUseSilence() == SkillSelectError.NO_ERROR){
				validMove = EMP();
			}else{
				validMove = false;
			}
			break;
		}
		if(validMove){
			activeTurn.Add(o);
			if(o.endTurn && playerDone){
				EndTurn();
			}else if(o.endTurn){
				Console.PrintToConsole("You are trying to end the turn without placing your piece",Console.MessageType.ERROR);	
			}
		}
		return validMove;
	}
	
	public void ExecuteTurn(Turn t){
		if(t.IsValid()){
			foreach( Order o in t.GetOrderList()){
				if(!ExecuteOrder(o))
					break;
			}
		}else{
			Debug.LogError("invalid turn-code");
		}
	}
	
	//Move to Skill-class?
	private bool PlacePiece(FieldIndex index){ //placing piece in a normal turn
		if (playerDone == false && cState.field[index] == Route.empty){
//			Debug.Log("Index: " + index);
			sound.PlaySound(SoundType.onClick);
			if(cState.activePlayer == 0){
				cState.field[index] = Route.red;
			}else{
				cState.field[index] = Route.blue;
			}
			if(CheckCluster(index)){
				cState.IncPieceCount();
				playerDone = true;
			}else{ //the move is illegal due to silence
				cState.field[index] = Route.empty;
				Console.PrintToConsole("You are silenced; You can't build towers",Console.MessageType.ERROR);
				return false;
			}
			return true;
		}else{
			if(playerDone){
				Console.PrintToConsole("Can only place one piece each turn",Console.MessageType.ERROR);
			}else{
				Console.PrintToConsole("Cannot place there",Console.MessageType.ERROR);
			}
			sound.PlaySound(SoundType.error);
			// **DEBUG** write this out somehow
			return false;
		}
	}
	//Move to Skill-class?
	private bool ExtraBuild(FieldIndex index){ //placing an extra piece with the build-skill
		if (cState.field[index] == Route.empty){
		
			cState.field[index] = Field<int>.GetPlayerColor(cState.activePlayer);
			cState.player[cState.activePlayer].playerSkill.build--;
			Skill.skillsUsed.build++;
			
			CheckCluster(index);
			cState.IncPieceCount();
			Skill.skillInUse = 0;
			sound.PlaySound(SoundType.build);
			return true;
		}else{
			Debug.Log("invalid move");
			Console.PrintToConsole("Cannot place there",Console.MessageType.ERROR);
			sound.PlaySound(SoundType.error);
			return false;
		}
		//do not change first player
	}
	//Move to Skill-class?
	private bool Shoot(FieldIndex index){ //select an enemy piece to destroy it
	
		if (cState.field[index] == Field<int>.GetPlayerColor( (cState.activePlayer+1)%2 ) || cState.field[index] == Field<int>.GetPlayerColor(cState.activePlayer) ){
			cState.field[index] = Route.destroyed;
			cState.player[cState.activePlayer].playerSkill.shoot--;
			Skill.skillsUsed.shoot++;
			Skill.skillInUse = 0;
			BroadcastMessage("UpdateField");
			sound.PlaySound(SoundType.shoot);
			return true;
		}else{
			Console.PrintToConsole("Can only shoot active enemy pieces",Console.MessageType.ERROR);
			Debug.Log("invalid move");
			sound.PlaySound(SoundType.error);
			return false;
			//write this out somehow
		}
	}	
	//Move to Skill-class?
	public bool EMP(){
		Debug.Log("player "+(cState.activePlayer+1)+" has used EMP");
		Console.PrintToConsole("player "+(cState.activePlayer+1)+" has used EMP!",Console.MessageType.INFO);
		Skill.skillsUsed.emp++;
		cState.player[cState.activePlayer].playerSkill.emp--;
		cState.player[(cState.activePlayer+1)%2].silenced = true;
		sound.PlaySound(SoundType.emp);
		Skill.skillInUse = 0;
		return true;
	}
	
	private void ReportTower(Tower t){
		switch(t.type){
			case TowerType.shoot:
				cState.player[cState.activePlayer].playerSkill.shoot++;
				break;
			case TowerType.build:
				cState.player[cState.activePlayer].playerSkill.build++;
				break;
			case TowerType.emp:
				cState.player[cState.activePlayer].playerSkill.emp++;
				break;
			case TowerType.square:
				cState.player[cState.activePlayer].playerSkill.square++;
				break;
		}
	}
	
	private void EndTurn(){
		cState.ChangeActivePlayer();
		Console.PrintToConsole(activeTurn.ToString(),Console.MessageType.TURN);
		activeTurn = new Turn();
		playerDone = false;
		
		startOfTurn = new GameState(cState);
	}
	
	public void StartNewGame(){
		GameState tmp = Stats.startState;
		if (tmp == null){
			tmp = new GameState();
			tmp.SetDefault();
			Stats.SetDefaultSettings();
			Debug.Log("No settings found. Using default settings");
		}
		
		cState = new GameState(tmp);
		startOfTurn = new GameState(tmp);
		
		Skill.skillInUse = 0;
		Skill.skillsUsed = new SkillContainer();
		playerDone = false;
		
		sound.PlaySound(SoundType.background);
		activeTurn = new Turn();
		BroadcastMessage("InitField");
	}
	
	public void UndoTurn(){
		cState = new GameState(startOfTurn);
		playerDone = false;
		activeTurn = new Turn();
		Skill.skillInUse = 0;
		Skill.skillsUsed.Reset();
		BroadcastMessage("UpdateField");
	}
}