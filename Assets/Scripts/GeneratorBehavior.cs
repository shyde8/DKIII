﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject _pie;
    [SerializeField]
    private GameObject _taxi;
    [SerializeField]
    private GameObject _referencePlatform;
    private bool _isGenerating = false;

    public float generationSpeed;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!_isGenerating)
        {            
            StartCoroutine(Generate());
        }          
	}

    private IEnumerator Generate()
    {        _isGenerating = true;
        yield return new WaitForSeconds(generationSpeed);
        GameObject obj;
        if (_referencePlatform.GetComponent<PlatformItemManager>().ContainsTaxi() == false)
            obj = _taxi;
        else
            obj = _pie;
        obj = Instantiate(obj) as GameObject;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y - 0.5f);
        obj.transform.position = newPos;
        _isGenerating = false; 
    }
}
