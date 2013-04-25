#pragma strict

var spawn : Transform;
var player : GameObject;
var spawndistance : float = 200.0;

var pills : GameObject[];
pills = GameObject.FindGameObjectsWithTag("Pills");

var nextlevel : boolean = false;


function Start () {

nextlevel = false;

}

function Update () {

spawn.transform.position.x = player.transform.position.x;
spawn.transform.position.y = player.transform.position.y + spawndistance;
spawn.transform.position.z = player.transform.position.z;

}

function OnTriggerEnter (other:Collider){

	if(other.tag == "Player" && !nextlevel){
		other.transform.position = spawn.position;
		}
	if(other.tag == "Player" && nextlevel){
		Application.LoadLevel("door2");
		}
	

}

function nextLevel() {
	nextlevel = true;
	print("true");
}