﻿/// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class SkidMarksDestr : MonoBehaviour {

    public int destoryThisTime = 3;//in seconds
    public float alphaFade = 0.001F;//abstarct -0.001 means slow fade out in ~20 secs
    //private Transform skidMatGO;

    //Renderer thisRend; //Renderer

    private Color color = Color.black;

    Material skidMat;

    // Use this for initialization
    void Start () {

        skidMat = GetComponentInChildren<MeshRenderer>().material;
        //skidMatGO = transform.FindChild("skidPrefabMesh");
        Destroy(gameObject, destoryThisTime);
    }
	
	// Update is called once per frame
	void Update () {
            //fades alpha of skidmarks to "softdestroy" effect
            Color newColor = skidMat.color;
            newColor.a -= alphaFade;
            skidMat.color = newColor;

    }
}
