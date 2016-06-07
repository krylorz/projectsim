using UnityEngine;
using System;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public string Time;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Time = DateTime.UtcNow.ToString();
	}
}
