#pragma strict

var mainCamera : Transform;
var mainRot = mainCamera.transform.rotation;
var mainPos = mainCamera.transform.position;

function Start () {


}

function Update () {

this.transform.position = mainPos;
this.transform.rotation = mainRot;

}