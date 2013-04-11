#pragma strict

var pills : GameObject[];
pills = GameObject.FindGameObjectsWithTag("Pills");

var howMany : GameObject;

function Start () {

}

function Update () {

}

function OnTriggerEnter (other:Collider){

	if(other.tag == "Player"){
		gameObject.tag = null;
		howMany.transform.SendMessage("touchedAnother");
		Destroy(gameObject);
	}
}