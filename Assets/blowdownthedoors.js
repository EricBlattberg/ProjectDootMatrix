#pragma strict

function Start () {

}

function Update () {

}

function blowDownTheWall () {
	transform.rigidbody.isKinematic = false;
	transform.rigidbody.AddForce(Vector3.forward * 7,ForceMode.Impulse);
	Destroy(gameObject, 20);
}