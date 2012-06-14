using UnityEngine;
using System.Collections;

public class GUI_script : MonoBehaviour {

	private Control control;
	private Grid grid;
	
	public GUISkin customSkin;
	
	public bool enable;
	public bool lockGUI;

	private SkillGUI skillGUI = SkillGUI.Create();
	private HeaderBar header;
	private ButtonRow buttonRow;
	
	void Start () {
		control = (Control)FindObjectOfType(typeof(Control));
		grid = (Grid)FindObjectOfType(typeof(Grid));
		buttonRow =  new ButtonRow(control);
		header = new HeaderBar(control);
	}
	
	void OnGUI() {
		if(!enable){
			return;
		}
		GUI.enabled = !lockGUI;
		GUI.skin = customSkin;

		header.PrintGUI();
		skillGUI.PrintGUI();
		buttonRow.PrintGUI();
		
		Console.PrintWindow();
		PopupMessage.PrintGUI();
		
		//----Framework to handle mouse-input etc----//
		GUI.enabled = true;

		if(Event.current.type == EventType.MouseDown){
			if( Event.current.type != EventType.Used ){
				grid.MouseDown(Input.mousePosition);
			}
		}
	}	
}
