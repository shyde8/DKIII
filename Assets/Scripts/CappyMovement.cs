using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappyMovement : MonoBehaviour
{
    public float SPIN_MULTIPLIER = 1000f;
    public float speed = 1.75f;
    public float endPosOffset = 1.75f;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _trackPercent = 0;
    private float _direction;
    private GameObject _jumpMan;
    private float _yOffset; //to be used in Return() coroutine, so Cappy targets the same height of jumpman he was released from
    private bool _isReturnRunning = false; //flag to indicate whether the the Return() coroutine has started

    private void Start()
    {
        _startPos = transform.position;
        _direction = Mathf.Sign(transform.localScale.x);
        _endPos = new Vector3(transform.position.x + (_direction * endPosOffset), transform.position.y);
        _jumpMan = GameObject.Find("Jumpman");
        _yOffset = _startPos.y - _jumpMan.transform.position.y;
        StartCoroutine(Throw());
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
            float x = (_endPos.x - _startPos.x) * (speed * Time.deltaTime) + _startPos.x;
            float y = ((_endPos.y+_yOffset) - _startPos.y) * (speed * Time.deltaTime) + _startPos.y; //ensure we're targeting jumpman's y-position, offset by _yOffset
            transform.position = new Vector3(x, y);

            //jumpman and cappy are approximately touching when the distance between their x-coordinates is 0.55, set cappy to isTrigger=true at this point
            if (Mathf.Abs(_endPos.x - _startPos.x) < 0.55f)
                gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

            yield return new WaitForFixedUpdate();
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _jumpMan)
        {
            if (_isReturnRunning)
                Destroy(this.gameObject);
        }
    }

}
