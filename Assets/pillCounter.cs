using UnityEngine;
using System.Collections;

public class pillCounter : MonoBehaviour {
	
	public GUISkin mySkin; // assign in the inspector
	public int pillsLeft = 0;
	public GameObject spawnCollider;
	
	public GameObject[] pills;
	
	public int pillsTotal;
	
	// Use this for initialization
	void Start () {
		
		pills = GameObject.FindGameObjectsWithTag("Pills");
		pillsTotal = pills.Length;
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(pillsLeft == pillsTotal){
			spawnCollider.transform.SendMessage("nextLevel");
		}
	
	}
	
	void OnGUI(){
   	GUI.skin = mySkin;
   	GUILayout.Label(""+pillsLeft+"/"+pillsTotal, "label");
		
	}
	
	void touchedAnother () {
		pillsLeft ++;
//		Camera.mainCamera.GetComponent(DepthOfField).enabled = true;
//  		Camera.mainCamera.GetComponent(DepthOfField).focalSize = 5;
		
	}

}
