﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private GameObject cappyPrefab;
    private GameObject _cappy;
    private float _width;
    private float _height;
    public bool isCoolingDown = false;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.CAPPY_DESTROYED, StartCoolDown);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.CAPPY_DESTROYED, StartCoolDown);
    }


    private void Start()
    {
        _width = gameObject.GetComponent<Renderer>().bounds.size.x;
        _height = gameObject.GetComponent<Renderer>().bounds.size.y;
    }

    private void Update()
    {
        //instantiate cappy if fire2 button is pressed
        if (Input.GetAxis("Fire2") > 0f)
        {
            Debug.Log(isCoolingDown);
            if (_cappy == null && !isCoolingDown)
            {
                gameObject.GetComponent<PlayerMovement>().CappyThrowBurst();
                _cappy = Instantiate(cappyPrefab) as GameObject;
                float cappyHeight = _cappy.GetComponent<Renderer>().bounds.size.y;
                //scale is positive if jumpman is facing right, scale is negative if jumpman is facing left
                float dir = Mathf.Sign(transform.localScale.x);
                //12-23, if Jumpman is wall-jumping, then multiple dir by -1, so cappy is thrown in direction opposite of wall
                if (gameObject.GetComponent<PlayerMovement>().IsWallJumping() == true)
                    dir *= -1;
                Vector2 newPos = new Vector2(transform.position.x + (dir * (_width/1.5f)), transform.position.y + (_height/2) - (cappyHeight/2));
                _cappy.transform.position = newPos;
                //set scale of cappy to negative, if he was generated to the left of jumpman
                Vector3 tempScale = _cappy.transform.localScale;
                _cappy.transform.localScale = new Vector3(tempScale.x * dir, tempScale.y); 
                            
            }
        }               
    }

    public IEnumerator Cooldown()
    {
        isCoolingDown = true;
        int numFrames = 0;        
        while(numFrames < 10)
        {
            numFrames++;
            yield return new WaitForFixedUpdate();
        }
        isCoolingDown = false;
    }

    private void StartCoolDown()
    {
        StartCoroutine(Cooldown());
    }
}
