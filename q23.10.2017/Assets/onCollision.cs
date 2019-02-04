using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class onCollision : MonoBehaviour {
	public Text collisionCount;
	// counting the collsions between particle system and plane 
	// to find out the effectivity of the proposed approach
	int counter = 0; 

	// getting the coordinates of the quad
	public Text xP;
	public Text yP;
	public Text zP;
	private Vector3 xyPosition;

	// creating Timer to get time and collision count
	//Timer tm = new Timer();
	Stopwatch stopwatch = new Stopwatch();
	string[] data = new string[300];
	//	string[] dataop = new string[300];
	int cnt = 0;

	void Start () {
		stopwatch.Start();
	}

	void OnParticleCollision (GameObject other)
	{
		//showPosition ();
		counter += 1;
		collisionCount.text = Convert.ToString (counter);
		//Debug.Log ("entered");
		//Debug.Log (Convert.ToString (counter));
	}

	void getPosition()
	{
		xyPosition = GameObject.Find ("Frame").GetComponent<Transform> ().position;
	}

	void showPosition()
	{
		getPosition ();
		//xP.text = Convert.ToString (xyPosition.x);
		//yP.text = Convert.ToString (xyPosition.y);
		//zP.text = Convert.ToString (xyPosition.z);
		writeToFile ();
		if (cnt < 300) {
			cnt++;
		}	
	}

	void writeToFile()
	{		
		//Debug.Log("South");
		//data = {Convert.ToDouble(stopwatch.Elapsed), counter};
		TimeSpan ts = stopwatch.Elapsed;
		string elTime = String.Format ("{0:00}.{1:0000}", ts.Seconds, ts.Milliseconds);
		data[cnt] = elTime + "," + Convert.ToString(counter);  
		//dataop[cnt] = stopwatch.Elapsed.ToString()
		//data [cnt, cnt + 1] = Convert.ToString(counter);
		//UnityEngine.Debug.Log(elTime);
		System.IO.File.WriteAllLines (@"non_w.csv", data);

		//System.IO.File.WriteAllLines (@"operator.csv", data);
	}

}
