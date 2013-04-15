#pragma strict

var mainCamera : Transform;
var spawndistance : float = 200.0;
var hero : Transform;

var myTR : Transform;

function Start () {

myTR = this.transform;


}

function Update () {

myTR.position.x = mainCamera.position.x;
myTR.position.y = mainCamera.position.y - spawndistance;
myTR.position.z = mainCamera.position.z;

this.transform.LookAt(hero);

}