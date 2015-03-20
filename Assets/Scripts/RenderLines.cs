﻿using UnityEngine;
using System.Collections;

public class RenderLines : MonoBehaviour {
	private LineRenderer line;
	private LineRenderer bounceLine;
	private Vector3 mousePosition;
	private Vector3 mouseWorld;
	public float chargeLevel;
	public Camera gameCamera;
	public GameObject sphereCastTestObject;
	private Vector3 cueBallDirection;
	private bool allAsleep;
	private float now;
	private float lastHit;

	private float startCharge;

	// Use this for initialization
	void Start () {
		line = gameObject.GetComponent<LineRenderer> ();
		line.SetWidth(0.1f, 0.1f);
		line.SetVertexCount(0);
		bounceLine = gameCamera.GetComponent<LineRenderer>();
		bounceLine.SetWidth (0.1f, 0.1f);
		bounceLine.SetVertexCount (0);
		//gameCamera = GameObject.FindObjectOfType<Camera> ();
		//Physics.sleepThreshold = 1000.0f;
		allAsleep = true;

	}
	
	// Update is called once per frame
	void Update () {

		if (allAsleep) {
			UpdateLine ();
			if (Input.GetMouseButtonDown(0))
			{
				startCharge = Time.realtimeSinceStartup;
			}
			if (Input.GetMouseButtonUp (0)) 
			{
				line.SetVertexCount (0);
				bounceLine.SetVertexCount(0);
				chargeLevel = Mathf.Clamp(Time.realtimeSinceStartup - startCharge, 0.0f, 2.0f) / 2.0f;
				this.gameObject.GetComponent<Rigidbody> ().AddForce (cueBallDirection * 20000.0f * chargeLevel);
				allAsleep = false;

				lastHit = Time.realtimeSinceStartup;
				
			}

		} else {

			CheckObjectsHaveStopped ();
		} 
		now = Time.realtimeSinceStartup;
		if (now - lastHit >= 5.0f)
		{
			StopAllObjects();
			//allAsleep = true;
		}

		
	}
	
	
	void UpdateLine()
	{
			
			Ray mouseToWorldSpaceRay = gameCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			RaycastHit sphereHit;
			RaycastHit bounceHit;
			Debug.DrawRay (mouseToWorldSpaceRay.origin, mouseToWorldSpaceRay.direction * 100, Color.yellow);
			line.SetVertexCount(2);
			line.SetPosition (0, this.gameObject.transform.position);
			if (Physics.Raycast (mouseToWorldSpaceRay, out hit)) {
			cueBallDirection = (new Vector3 (hit.point.x, this.gameObject.transform.position.y, hit.point.z) - gameObject.transform.position).normalized;
				cueBallDirection.y = 0.0f;
			}
		if (Physics.SphereCast (this.gameObject.transform.position, 0.30f, cueBallDirection, out sphereHit,(new Vector3 (hit.point.x, this.gameObject.transform.position.y, hit.point.z) - gameObject.transform.position).magnitude)){
				Vector3 newDirection = new Vector3 (sphereHit.normal.x, 0.0f, sphereHit.normal.z);
				Physics.Raycast (sphereHit.collider.transform.position, -newDirection, out bounceHit);
				
				line.SetPosition (1,new Vector3(sphereHit.point.x,this.gameObject.transform.position.y,sphereHit.point.z));
				bounceLine.SetVertexCount(2);
				bounceLine.SetPosition (0, sphereHit.collider.transform.position);
				bounceLine.SetPosition (1, bounceHit.point);

		} else{
			bounceLine.SetVertexCount(0);
			line.SetPosition (1, new Vector3 (hit.point.x, this.gameObject.transform.position.y, hit.point.z));
		}
				
	} 


	void CheckObjectsHaveStopped()
	{

		Rigidbody[] GOS = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
		bool allObjectsAsleep = true;
			foreach (Rigidbody GO in GOS) 
			{
				if(!GO.IsSleeping())
				{
					allObjectsAsleep = false;
				}
			}
		allAsleep = allObjectsAsleep;
			
	}

	void StopAllObjects()
	{
		Rigidbody[] GOS = FindObjectsOfType (typeof(Rigidbody)) as Rigidbody[];
		foreach (Rigidbody GO in GOS) {
			GO.Sleep ();

		}
	}

		
}