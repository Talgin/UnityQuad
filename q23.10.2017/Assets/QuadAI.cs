using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadAI : MonoBehaviour {
	// neural network to be trained to show the desired coordinates given 
	// two inputs: wind velocity and direction
	// 
	alglib.mlptrainer trn;
	alglib.multilayerperceptron network;
	alglib.mlpreport rep;

	//quadrocopterScript

	double[,] xy = new double[,]{{1,1,1},{1,2,2},{2,1,2},{2,2,4}};

	public int runML(){
		// Network is created.
		// Trainer object is created.
		// Dataset is attached to trainer object.
		//
		alglib.mlpcreatetrainer(2, 1, out trn);
		alglib.mlpcreate1(2, 5, 1, out network);
		alglib.mlpsetdataset(trn, xy, 4);

		//
		// Network is trained with 5 restarts from random positions
		//
		alglib.mlptrainnetwork(trn, network, 5, out rep);

		//
		// 2*2=?
		//
		double[] x = new double[]{2,2};
		double[] y = new double[]{0};
		alglib.mlpprocess(network, x, ref y);
		System.Console.WriteLine("{0}", alglib.ap.format(y,1)); // EXPECTED: [4.000]
		System.Console.ReadLine();
		return 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		
	}
}
