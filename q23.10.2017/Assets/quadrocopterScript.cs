using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Timers;
using System.Collections.Generic;
using System.IO;
//using windStart;

public class quadrocopterScript : MonoBehaviour {
    public bool startPressed = false;
    //Timer delta_t = new Timer();

	// Wind Zone details
	public WindZone Wind;

	// Showing the current x and y coordinates 
	public Text xP;
	public Text yP;
	public Text zP;
	private Vector3 xyPosition;

	// Showing rotation
	public Text tbRoll;
	public Text tbPitch;
	public Text tbYaw;

	// Showing targets
	public Text tbTgtRoll;
	public Text tbTgtPitch;
	public Text tbTgtYaw;

    // create list and add values of waypoints
    // drone flies from one point to another and during this period we read an information about coordinates... namely, x and y coordinates, because we don't need z as it an altitude
	//фактические параметры
	private double pitch; //Тангаж
	private double roll; //Крен
	private double yaw; //Рыскание
	public double throttle = 25; //Газ, газ мы задаем извне, поэтому он public

	//требуемые параметры
	public double targetPitch;
	public double targetRoll;
	public double targetYaw;

	//PID регуляторы, которые будут стабилизировать углы
	//каждому углу свой регулятор, класс PID определен ниже
	//константы подобраны на глаз :) пробуйте свои значения
	private PID pitchPID = new PID (100, 0, 20); // 0, 0, 0
    private PID rollPID = new PID (100, 0, 20);  // 1000, 200, 50
    private PID yawPID = new PID (50, 0, 50);    // 10000, 1000, 1000

    //private Quaternion prevRotation = new Quaternion (0, 1, 0, 0);

    public void StartPress() 
    {
        startPressed = true;
		//windStart.windSettings (15, -90);
    }

	// getting position information
	void getPosition()
	{
		xyPosition = GameObject.Find ("Frame").GetComponent<Transform> ().position;
	}

	// showing position in a textbox
	void showPosition()
	{
		getPosition ();
		xP.text = Convert.ToString (xyPosition.x);
		yP.text = Convert.ToString (xyPosition.y);
		zP.text = Convert.ToString (xyPosition.z);
	}

	void showRotation ()
	{
		tbRoll.text = Convert.ToString (Math.Round(roll, 3));
		tbPitch.text = Convert.ToString (Math.Round(pitch, 3));
		tbYaw.text = Convert.ToString (Math.Round(yaw, 3));
	}

	void showTargets()
	{
		tbTgtRoll.text = Convert.ToString (targetRoll);
		tbTgtPitch.text = Convert.ToString (targetPitch);
		tbTgtYaw.text = Convert.ToString (targetYaw);		
	}

	void flyTo()
	{
		
	}

	void holdAltitude()
	{
		//if(
	}

	void readInput()
	{
		if (Input.GetKey (KeyCode.LeftArrow)) {
			targetRoll = 5;
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			targetRoll = -5;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			throttle = throttle - 2;
		} else if (Input.GetKey (KeyCode.UpArrow)) {
			throttle = throttle + 2;
		}
	}

	void readRotation () {		
		//фактическая ориентация нашего квадрокоптера,
		//в реальном квадрокоптере эти данные необходимо получать
		//из акселерометра-гироскопа-магнетометра, так же как делает это ваш
		//смартфон
		Vector3 rot = GameObject.Find ("Frame").GetComponent<Transform> ().rotation.eulerAngles;
		pitch = rot.x;
		yaw = rot.y;
		roll = rot.z;
	}

	//функция стабилизации квадрокоптера
	//с помощью PID регуляторов мы настраиваем
	//мощность наших моторов так, чтобы углы приняли нужные нам значения
	void stabilize () {

		//нам необходимо посчитать разность между требуемым углом и текущим
		//эта разность должна лежать в промежутке [-180, 180] чтобы обеспечить
		//правильную работу PID регуляторов, так как нет смысла поворачивать на 350
		//градусов, когда можно повернуть на -10

		double dPitch = targetPitch - pitch;
		double dRoll = targetRoll - roll;
		double dYaw = targetYaw - yaw;

		dPitch -= Math.Ceiling (Math.Floor (dPitch / 180.0) / 2.0) * 360.0;
		dRoll -= Math.Ceiling (Math.Floor (dRoll / 180.0) / 2.0) * 360.0;
		dYaw -= Math.Ceiling (Math.Floor (dYaw / 180.0) / 2.0) * 360.0;

		//1 и 2 мотор впереди
		//3 и 4 моторы сзади
		double motor1power = throttle;
		double motor2power = throttle;
		double motor3power = throttle;
		double motor4power = throttle;

		//ограничитель на мощность подаваемую на моторы,
		//чтобы в сумме мощность всех моторов оставалась
		//одинаковой при регулировке
		double powerLimit = throttle > 20 ? 20 : throttle;

		//управление тангажем:
		//на передние двигатели подаем возмущение от регулятора
		//на задние противоположное возмущение

        // айналу
		double pitchForce = - pitchPID.calc (0, dPitch / 180.0);
		pitchForce = pitchForce > powerLimit ? powerLimit : pitchForce;
		pitchForce = pitchForce < -powerLimit ? -powerLimit : pitchForce;
		motor1power +=   pitchForce;
		motor2power +=   pitchForce;
		motor3power += - pitchForce;
		motor4power += - pitchForce;

		//управление креном:
		//действуем по аналогии с тангажем, только регулируем боковые двигатели

        // қисаю 
		double rollForce = - rollPID.calc (0, dRoll / 180.0);
		rollForce = rollForce > powerLimit ? powerLimit : rollForce;
		rollForce = rollForce < -powerLimit ? -powerLimit : rollForce;
		motor1power +=   rollForce;
		motor2power += - rollForce;
		motor3power += - rollForce;
		motor4power +=   rollForce;

		// управление рысканием:
        // жортуды басқару
		double yawForce = yawPID.calc (0, dYaw / 180.0);
		yawForce = yawForce > powerLimit ? powerLimit : yawForce;
		yawForce = yawForce < -powerLimit ? -powerLimit : yawForce;
		motor1power +=   yawForce;
		motor2power += - yawForce;
		motor3power +=   yawForce;
		motor4power += - yawForce;

		GameObject.Find ("Motor1").GetComponent<motorScript>().power = motor1power;
		GameObject.Find ("Motor2").GetComponent<motorScript>().power = motor2power;
		GameObject.Find ("Motor3").GetComponent<motorScript>().power = motor3power;
		GameObject.Find ("Motor4").GetComponent<motorScript>().power = motor4power;
	}    

	// как советуют в доке по Unity вычисления проводим в FixedUpdate, а не в Update
	void FixedUpdate () {		
		//windSettings(15, -90);
		getPosition (); 
		showPosition ();
		readInput ();
		showRotation ();
		showTargets ();
		Debug.Log (Convert.ToString (windStart.dir));
        if (startPressed)
        {
            readRotation();
			//stabilize ();
			// write a function to change direction of flight according to speed and dir of wind
			if (xyPosition.y >= 27.2) {
				//target
				targetPitch = 0;
				throttle = 0;
				stabilize ();
			} else if (xyPosition.y <= 27.1) {
				targetPitch = 0;
				throttle = 30;
				stabilize ();		// hold quadrotor in a range
			} else if (windStart.dir == 1) {
				targetRoll = -5;
			} else if (roll > 2) {
				targetPitch = 5;
				//Debug.Log ("range");
			} /*else if (pitch > 2) {
				targetPitch = 0;
				//Debug.Log ("derange");
			} else if (windStart.dir == 1) { // if wind comes from east
				targetRoll = 5;
				Debug.Log ("right");
			} else if (windStart.ws == 25 && windStart.dir == 5) { // if wind comes from west 
				roll = -5;
				//Debug.Log ("left");
			}*/
        }
	}
	
}

public class Control {
    private double forward;
    public Control(double forward) {
        this.forward = forward;
    }
}

public class PID {
	
	private double P;
	private double I;
	private double D;
	
	private double prevErr;
	private double sumErr;
	
	public PID (double P, double I, double D) {
		this.P = P;
		this.I = I;
		this.D = D;
	}
	
	public double calc (double current, double target) {
		
		double dt = Time.fixedDeltaTime;
		
		double err = target - current;
		this.sumErr += err;
		
		double force = this.P * err + this.I * this.sumErr * dt + this.D * (err - this.prevErr) / dt;
		
		this.prevErr = err;
		return force;
	}
	
};
