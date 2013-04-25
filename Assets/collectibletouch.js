#pragma strict

var pills : GameObject[];
pills = GameObject.FindGameObjectsWithTag("Pills");

var howMany : GameObject;
var timer : GameObject;

function Start () {

}

function Update () {

}

function OnTriggerEnter (other:Collider){

	if(other.tag == "Player"){
		gameObject.tag = null;
			if(this.name == "speedCollectible"){
				other.transform.SendMessage("hitSpeed");
				timer.transform.SendMessage("hitSpeed");
			}
			if(this.name == "massCollectible"){
				other.transform.SendMessage("hitMass");
				timer.transform.SendMessage("hitMass");
			}
			
		howMany.transform.SendMessage("touchedAnother");
		Destroy(gameObject);
	}
}