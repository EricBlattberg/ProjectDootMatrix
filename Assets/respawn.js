#pragma strict

var spawn : Transform;
var player : GameObject;
var spawndistance : float = 200.0;

function Start () {

}

function Update () {

spawn.transform.position.x = player.transform.position.x;
spawn.transform.position.y = player.transform.position.y + spawndistance;
spawn.transform.position.z = player.transform.position.z;

}

function OnTriggerEnter (other:Collider){
	if(other.tag == "Player"){
		other.transform.position = spawn.position;
	}

}