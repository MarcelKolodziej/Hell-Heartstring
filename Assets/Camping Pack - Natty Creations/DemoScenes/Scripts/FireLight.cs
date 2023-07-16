using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLight : MonoBehaviour
{
	private float y0;
	private Vector3 tempPos;
	public float bobFrequency;
	public float amplitude;
	void Start()
	{
		//y0 = transform.position.y;

	}
	void FixedUpdate()
	{
		tempPos = transform.position;
		tempPos.x += Mathf.Sin(Time.fixedTime * Mathf.PI * bobFrequency * 0.4f) * (amplitude * 0.01f);
		tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * bobFrequency) * (amplitude * 0.01f);
		tempPos.z += Mathf.Sin(Time.fixedTime * Mathf.PI * bobFrequency * 0.6f) * (amplitude * 0.01f);
		transform.position = tempPos;
	}
}