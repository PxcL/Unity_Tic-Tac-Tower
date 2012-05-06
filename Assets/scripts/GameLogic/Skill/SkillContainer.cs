using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Struct SkillContainer:
// -stores the availible skills for each player

public struct SkillContainer{
	
	public int shoot;
	public int build;
	public int emp;
	public int square;
		
	public void Reset(){
		shoot = 0;
		build = 0;
		emp = 0;
		square = 0;
	}
	
}