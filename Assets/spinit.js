#pragma strict

var speed : int = 1000;

function Start () {

}

function Update () {

transform.Rotate(0, Input.GetAxis("Horizontal")*speed, 0,Space.World);

}