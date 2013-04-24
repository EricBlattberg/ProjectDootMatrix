#pragma strict

var walls : GameObject[];
walls = GameObject.FindGameObjectsWithTag("Walls");

var touch : AudioClip;

function Start () {

}

function Update () {

}

function OnTriggerEnter (other:Collider){

	if(other.tag == "Player")
	{
		print("Playertouch");
		
		for (var wall : GameObject in walls)  
		{
			wall.transform.SendMessage("blowDownTheWall");
		}
		
		Destroy (gameObject);
		
		Camera.mainCamera.audio.Stop();
		Camera.mainCamera.audio.clip = touch;

		Camera.mainCamera.audio.Play();
		

    } 

	}
	