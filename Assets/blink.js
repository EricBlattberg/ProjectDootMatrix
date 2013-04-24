#pragma strict

var Countdown = 3;

function Start(){

    renderer.enabled=true;

    var whenAreWeDone=Time.time + Countdown;

    while(3 <  4){

        yield WaitForSeconds(0.5);

        renderer.enabled=!renderer.enabled;

    }

    renderer.enabled=true;

}

function Update () {



}