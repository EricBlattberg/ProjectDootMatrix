#pragma strict

var myStyle : GUIStyle;
var endTicks : float = 0;
var displayTicks : float;
private var declineSpeed : float = 1500;//higher# is a slower decline

//private var getCollectible : float = 125;
//
//private var rate : float = Screen.width/declineSpeed;// * percentDecline;

//var distance : float = Vector3.Distance (hero.transform.position, spawn.transform.position);
var hero : GameObject;
var spawn : GameObject;


function Start () {
	//displayTicks = Screen.width;
}

function Update () {

	//var distance: float;

    //distance = Vector3.Distance (hero.transform.position.y, spawn.transform.position.y);
	
//	if (displayTicks > 0) {
//		//displayTicks -= rate;
//		//print(displayTicks);
//	}

}
 
function OnGUI () {
   GUI.Label (Rect (Screen.width - 72, Screen.height - 72, 50, 17), " mytext", myStyle);
}

//function AddTime () {
//	displayTicks += rate * getCollectible;
//}