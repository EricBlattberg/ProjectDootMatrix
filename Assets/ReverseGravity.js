#pragma strict

function Start () {

//test

}

function Update () {

	if(Input.GetButton("reverse"))
	
	{

		 Physics.gravity = Vector3(0, 10.0, 0);
	
	}
	
	else
	
		 Physics.gravity = Vector3(0, -10.0, 0);


}