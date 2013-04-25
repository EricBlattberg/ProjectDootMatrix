#pragma strict

var walls : GameObject[];
walls = GameObject.FindGameObjectsWithTag("Walls");

function Start () {

}

function Update () {

}

function OnTriggerEnter (other:Collider){

	if(other.tag == "Player"){
		print("Playertouch");
		for (var wall : GameObject in walls)  {
			wall.transform.SendMessage("blowDownTheWall");
		}
		Destroy (gameObject);
	}
}