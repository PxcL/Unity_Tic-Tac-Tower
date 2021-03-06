using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control: MonoBehaviour {

	public bool playerDone = false; //Is true when player has placed a piece. Allows user to "End Turn".	
	public static GameState cState; //the current gamestate of the progressing game
	public static GameState startOfTurn; //the undo-point
	
	private Turn activeTurn;
	private Sound sound;
	private GraphicalEffectFactory graphicalEffectFactory;
	
	private bool victory = false;
	
	void Awake () {
		sound = (Sound)FindObjectOfType(typeof(Sound));
		graphicalEffectFactory = (GraphicalEffectFactory)FindObjectOfType(typeof(GraphicalEffectFactory));
		if (sound == null){
			Debug.LogError("sound-object not found");
		}
		if (graphicalEffectFactory == null){
			Debug.LogError("graphicalEffectFactory-object not found");
		}
		Console.Init(this);
	}
	
	void Start(){
		StartNewGame();
	}
	
	public void UserFieldSelect(FieldIndex index){
		//Called when the user clicks on the field
		if(Stats.playerController[cState.activePlayer] == Stats.PlayerController.localPlayer){
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
					o.skill = SkillType.silence;
					break;
			}
			ExecuteOrder(o);
			if(victory)
				Victory(cState.activePlayer);
		}
	}
	
	public void UserEndTurn(){
		Order o = new Order();
		o.skill = SkillType.noSkill;
		o.endTurn = true;
		ExecuteOrder(o);
	}
	
	public void UserResign(){
		Victory((cState.activePlayer+1)%2);
	}
	
	public void TimeOut(){
		Victory((cState.activePlayer+1)%2);
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
			if(tower.Count > 0){
				cState.player[cState.activePlayer].AddScore(tower.Count);
				sound.PlayEffect(SoundType.newTower);
			}
			Tower fiveTower = null;
			foreach( Tower t in tower){

				//Coloring the towers:
				foreach(FieldIndex i in t.GetList()){ 
					if(t.type == TowerType.five){
						fiveTower = t;
					}
					if(Stats.rules == Stats.Rules.SOLID_TOWERS){
						cState.field[i] = Field<int>.GetDarkRoute(cState.field[i]);
					}else if(Stats.rules == Stats.Rules.INVISIBLE_TOWERS){
						cState.field[i] = Route.empty;
					}
				}
				//adding skills:
				ReportTower(t);
			}
			if(fiveTower != null){
				foreach(FieldIndex i in fiveTower.GetList()){ //recoloring five-towers
					cState.field[i] = Field<int>.GetPlayerColor(cState.activePlayer);
				}
				victory = true;
			}
			graphicalEffectFactory.BuildingConstructionEffect(tower);
		}else if(tower.Count > 0){ //if a tower was found that was blocked by Silence
			return false;
		}
		BroadcastMessage("UpdateField");
		return true;
	}
	
	private void Victory(int player){
		victory = false;
		sound.PlayEffect(SoundType.victory);
		EndTurn();
		Console.PrintToConsole("Player "+(player+1)+" has won!",Console.MessageType.INFO);
		Stats.gameRunning = false;
		PopupMessage.DisplayMessage("Player "+(player+1)+" has won!",10f);
		BroadcastMessage("OnVictory",SendMessageOptions.DontRequireReceiver);
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
		case SkillType.silence:
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
			
			if(cState.activePlayer == 0){
				cState.field[index] = Route.red;
			}else{
				cState.field[index] = Route.blue;
			}
			if(CheckCluster(index)){
				cState.IncPieceCount();
				playerDone = true;
				sound.PlayEffect (SoundType.onClick);
			}else{ //the move is illegal due to silence
				sound.PlayEffect (SoundType.error);
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
			sound.PlayEffect(SoundType.error);
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
			sound.PlayEffect(SoundType.build);
			return true;
		}else{
			Debug.Log("invalid move");
			Console.PrintToConsole("Cannot place there",Console.MessageType.ERROR);
			sound.PlayEffect(SoundType.error);
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
			sound.PlayEffect(SoundType.shoot);
			return true;
		}else{
			Console.PrintToConsole("Can only shoot active enemy pieces",Console.MessageType.ERROR);
			Debug.Log("invalid move");
			sound.PlayEffect(SoundType.error);
			return false;
			//write this out somehow
		}
	}	
	//Move to Skill-class?
	public bool EMP(){
		Debug.Log("player "+(cState.activePlayer+1)+" has used EMP");
		Console.PrintToConsole("player "+(cState.activePlayer+1)+" has used EMP!",Console.MessageType.INFO);
		Skill.skillsUsed.silence++;
		cState.player[cState.activePlayer].playerSkill.silence--;
		cState.player[(cState.activePlayer+1)%2].silenced = true;
		sound.PlayEffect(SoundType.silence);
		Skill.skillInUse = 0;
		graphicalEffectFactory.SilenceEffect();
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
			case TowerType.silence:
				cState.player[cState.activePlayer].playerSkill.silence++;
				break;
			case TowerType.skillCap:
				cState.player[cState.activePlayer].playerSkill.skillCap++;
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
		Stats.gameRunning = true;
		Console.Clear();
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
		
		activeTurn = new Turn();
		BroadcastMessage("InitField");
	}
	
	public void UndoTurn(){
		GameTime gt = cState.player[cState.activePlayer].gameTime;
		cState = new GameState(startOfTurn);
		cState.player[cState.activePlayer].gameTime = gt; // so that you cant undo the time
		playerDone = false;
		activeTurn = new Turn();
		Skill.skillInUse = 0;
		Skill.skillsUsed.Reset();
		BroadcastMessage("UpdateField");
	}
	
	public static void QuitGame(){
		//cleans up stuff, and makes ready to quit the game
//		NetworkInterface nif = (NetworkInterface)FindObjectOfType(typeof(NetworkInterface));
//		if(nif != null){
//			nif.DestroySelf();
//		}
		Application.LoadLevel("mainMenu");
	}
}