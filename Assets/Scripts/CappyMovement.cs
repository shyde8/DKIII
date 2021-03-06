﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappyMovement : MonoBehaviour
{
    /*todo:
     * cappy should start returning to jumpman as soon as jumpman jumps on him 
     * there should be a cooldown time after cappy returns to jumpman, during which he cannot be thrown
     */

    private AudioSource _audio;
    public float SPIN_MULTIPLIER = 1000f;    
    private float endPosOffset = 1.00f;
    private float speed = 10f;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _trackPercent = 0;
    private float _direction;
    private GameObject _jumpMan;
    private float _yOffset; //to be used in Return() coroutine, so Cappy targets the same height of jumpman he was released from
    private bool _isHovering = false;
    private bool _isReturnRunning = false; //flag to indicate whether the the Return() coroutine has started
    private float _accumulatedTime = 0f;
    private float _returnSpeedMultiplier = 2f; //we want cappy to return to jumpman at faster than the speed at which he is initially thrown
    private BoxCollider2D _box;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _jumpMan = GameObject.Find("Jumpman");
        _startPos = transform.position;
        _direction = Mathf.Sign(transform.localScale.x);
        _endPos = new Vector3(transform.position.x + (_direction * endPosOffset), transform.position.y);        
        _yOffset = _startPos.y - _jumpMan.transform.position.y;
        StartCoroutine(Throw());
        _box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update ()
    {
        //spinning effect
        transform.Rotate(Vector3.up * (SPIN_MULTIPLIER * Time.deltaTime));
    }

    private IEnumerator Throw()
    {
        while(_trackPercent < .99f)
        {
            _trackPercent += speed * Time.deltaTime;
            float x = (_endPos.x - _startPos.x) * _trackPercent + _startPos.x;
            float y = transform.position.y;
            transform.position = new Vector3(x, y);

            yield return new WaitForFixedUpdate();
        }

        //cappy should always hover for a certain duration, before calling the Hover() coroutine
        _isHovering = true;
        yield return new WaitForSeconds(1);
        StartCoroutine(Hover());
    }

    private IEnumerator Hover()
    {
        while (_accumulatedTime < 2f)
        {
            if (Input.GetAxis("Fire2") > 0f)
            {
                _accumulatedTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
                _accumulatedTime = 2f;
        }
        _isHovering = false;

        //we want to keep cappy locked in-place if jumpman is in the process of double-jumping
        while(_jumpMan.GetComponent<PlayerMovement>().IsDoubleJumping() == true)
            yield return new WaitForFixedUpdate();

        StartCoroutine(Return());
    }

    private IEnumerator Return()
    {
        _isReturnRunning = true;
        while(true)
        {
            //_endPos in Return coroutine will always be jumpman's position, and _startPos is cappy's current position
            _endPos = _jumpMan.transform.position;
            _startPos = transform.position;
            float x = (_endPos.x - _startPos.x) * (speed * Time.deltaTime * _returnSpeedMultiplier) + _startPos.x;
            float y = ((_endPos.y+_yOffset) - _startPos.y) * (speed * Time.deltaTime) + _startPos.y; //ensure we're targeting jumpman's y-position, offset by _yOffset
            transform.position = new Vector3(x, y);

            //jumpman and cappy are approximately touching when the distance between their x-coordinates is 0.55, set cappy to isTrigger=true at this point
            if (Mathf.Abs(_endPos.x - _startPos.x) < 0.55f)
            {
                gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                Messenger.Broadcast(GameEvent.CAPPY_DESTROYED);
            }                

            yield return new WaitForFixedUpdate();
        }    
    }

    public bool IsHovering()
    {
        return _isHovering;
    }

    public bool IsReturning()
    {
        return _isReturnRunning;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _jumpMan)
        {
            if (_isReturnRunning)
                Destroy(this.gameObject);
        }
        //set track-percent to .99f if encounter a lever
        if (collision.gameObject.name.Contains("Lever"))
        {
            _trackPercent = .99f;
            transform.position = collision.gameObject.transform.position;
        }            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _box.isTrigger = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _box.isTrigger = false;
    }

    public void StartReturning()
    {
        _accumulatedTime = 2f;
    }

}
