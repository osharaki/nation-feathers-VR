﻿using UnityEngine;
using System.Collections;

public class BeamLine : MonoBehaviour {

	public float MaxLength = 1.0f;
	public float StartSize = 1.0f;
	public float AnimationSpd = 0.1f;
	public Color BeamColor = Color.white;

	private float NowAnm;
	private LineRenderer line;

	private float NowLength;

	private bool bStop;

	public float GetNowLength()
	{
		return NowLength;
	}
	public void StopLength(float length)
	{
		NowLength = length;
		bStop = true;
	}
	void LineFunc()
	{
		if(!bStop)
			NowLength = Mathf.Lerp(0,MaxLength,NowAnm);
		float width = Mathf.Lerp(StartSize,0,NowAnm);
        //line.SetWidth(width,width);
        line.startWidth = width; //added by osharaki
        line.endWidth = width; //added by osharaki
		float length = NowLength;
		line.SetPosition(0,transform.position);
		line.SetPosition(1,transform.position+(transform.forward*length));
	}

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer>();
		//line.SetColors(BeamColor,BeamColor);
        line.startColor = BeamColor; //added by osharaki
        line.endColor = BeamColor; //added by osharaki
        NowAnm = 0;
		NowLength = 0;
		bStop = false;
		LineFunc();
	}
	
	// Update is called once per frame
	void Update () {

		NowAnm+=AnimationSpd;

		if(NowAnm > 1.0)
		{
			Destroy(this.gameObject);
		}
		LineFunc();
	}
}
