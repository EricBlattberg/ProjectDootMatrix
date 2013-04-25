#pragma strict

var openDoor : boolean = false;

function Start () {

}

function Update () {

	if(openDoor){
		transform.parent.SendMessage("openYourself");
	}

}

function OnTriggerEnter (other:Collider){
	if(other.tag == "Player"){
		//openDoor = true;
		Application.LoadLevel("door1draft");
	}

}