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

    private void Start()
    {
        _startPos = transform.position;
        _direction = Mathf.Sign(transform.localScale.x);
        _endPos = new Vector3(transform.position.x + (_direction * endPosOffset), transform.position.y);

        StartCoroutine(Throw());
    }

    // Update is called once per frame
    void Update ()
    {
        //spinning effect
        transform.Rotate(Vector3.up * (SPIN_MULTIPLIER * Time.deltaTime));

        //if ((_direction == 1 && _trackPercent > .9f) || (_direction == -1 && _trackPercent < .1f))
        //{
        //    _direction *= -1;
        //}

        Debug.Log(_direction);
    }

    private IEnumerator Throw()
    {
        while(_trackPercent < .98f)
        {
            _trackPercent += _direction * speed * Time.deltaTime;
            float x = (_endPos.x - _startPos.x) * _trackPercent + _startPos.x;
            float y = transform.position.y;
            transform.position = new Vector3(x, y);

            yield return new WaitForFixedUpdate();
        }
    }
}
