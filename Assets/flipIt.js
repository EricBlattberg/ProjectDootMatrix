#pragma strict

var hero : GameObject;

var originalZRot : Quaternion = Quaternion.Euler(0,0,0);
//var originalYPos = transform.position.y;

var angle : float = 180;
var rotation : Quaternion = Quaternion.Euler(90,0,0);

//var aroundPlayer : Vector3 = hero.transform.position;

var worldFlipped : boolean;

function Start () {

}

function Update () {
	if(Input.GetKeyDown("joystick button 4") && !worldFlipped){
		transform.rotation = rotation;
		//transform.position.y = 101.13;
		worldFlipped = true;
	}
	
	if(Input.GetKeyUp("joystick button 4") && worldFlipped){
			transform.rotation = originalZRot;
			//transform.position.y = originalYPos;
			worldFlipped = false;
		}
	
}