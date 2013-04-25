#pragma strict

var massTimerStyle : GUIStyle;
var speedTimerStyle : GUIStyle;

var screen : float = Screen.height;
var massTimer : float = 0.0;//timer start
var speedTimer : float = 0.0;//timer start
var pillWorth : float = 10.0;//timer add

var speedDisplay : float;
var massDisplay : float;

function Start () {

}

function Update () {
	if(massTimer > 0){
			massTimer -= Time.deltaTime;
			massDisplay = massTimer * screen / 10;
		}
	if(speedTimer > 0){
			speedTimer -= Time.deltaTime;
			speedDisplay = speedTimer * screen / 10;
		}
}

function OnGUI () {
	GUI.Label(new Rect(0,screen,12,-massDisplay), "", massTimerStyle);
	GUI.Label(new Rect(Screen.width - 12,screen,12,-speedDisplay), "", speedTimerStyle);
}

function hitMass () {
	massTimer += pillWorth;
}

function hitSpeed () {
	speedTimer += pillWorth;
}