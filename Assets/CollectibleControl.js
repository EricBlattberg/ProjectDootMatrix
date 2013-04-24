#pragma strict

var i : int = 0;
//var j : int;
var pills : GameObject;

var isSucceed : boolean = false;
var successText : GameObject;

var myStyle : GUIStyle;

var temp : GameObject;

var tempColor : Color;

var endTime : float;
var textMesh : TextMesh;


function Start () {

	tempColor = Color.Lerp(Color.blue, Color.red, Time.time/5000);

	pills = this.gameObject;
	
	print("the arraylength is: " + pills.transform.childCount);
	
	var j : int = 0;

	for (j = 0; j < pills.transform.childCount; j++)
	{
		temp = pills.transform.GetChild(j).gameObject;
		
		temp.particleSystem.startColor = Color.cyan;
		temp.particleSystem.startLifetime = .3;
		temp.particleSystem.gravityModifier = -10;
		temp.particleSystem.playbackSpeed = .1;
	
	}
 
 } 

function Update () {
		
	if(pills.transform.childCount > 0){
		successText.active = false;
	}
	
	else{
		successText.active = true;
	}
	
}
