#pragma strict
var mainCamera : Transform;
var spawndistance : float = 156.0;
var hero : Transform;
var myTR : Transform;
function Start () {
	myTR = this.transform;
}
function Update () {
	myTR.position = Vector3(mainCamera.position.x, mainCamera.position.y + spawndistance, mainCamera.position.z);
	myTR.rotation = mainCamera.rotation;
}