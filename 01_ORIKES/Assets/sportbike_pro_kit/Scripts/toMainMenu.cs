﻿/// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class toMainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if  (Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("u5bike_MainMenu");
		}
	}
}
