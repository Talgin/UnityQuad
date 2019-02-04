using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class windStart : MonoBehaviour {

    // gameObjects
    public GameObject Quadrocopter;
    public WindZone Wind;
    public Slider windSpeed;
	
    // variables    
    public static int ws = 25;
	public static int dir = 0;
    public void windEast()
    {
        windSettings(ws, -90);
		dir = 1;
        Debug.Log("East");
    }
    public void windSouthEast()
    {       
        windSettings(ws, -45);
		dir = 2;
        Debug.Log("South East");
    }
    public void windSouth()
    {       
        windSettings(ws, 0);
		dir = 3;
        Debug.Log("South");
    }
    public void windSouthWest()
    {
        windSettings(ws, 45);
		dir = 4;
        Debug.Log("South West");
    }
    public void windWest()
    {
        windSettings(ws, 90);
		dir = 5;
        Debug.Log("West");
    }
    public void windNorthWest()
    {
        windSettings(ws, 135);
		dir = 6;
        Debug.Log("North West");
    }
    public void windNorth()
    {
        windSettings(ws, 180);
		dir = 7;
        Debug.Log("North");
    }
    public void windNorthEast()
    {
        windSettings(ws, -135);
		dir = 8;
        Debug.Log("North East");
    }
    public void Slider_Changed()
    {
        //ws = (int)System.Math.Round(windSpeed.value);        
        //Vector3 pos = Quadrocopter.transform.position;
        //pos.y = newValue;
        //Quadrocopter.transform.position = pos;
    }
    public void windSettings(int speed, int angle)
    {
        Wind.GetComponent("WindZone");
        Wind.transform.rotation = Quaternion.Euler(0, angle, 0);
        Wind.mode = WindZoneMode.Directional;
        Wind.windMain = speed;
        Wind.windTurbulence = 1;
        Wind.windPulseMagnitude = 0.5f;
        Wind.windPulseFrequency = 0.01f;
    }
}
