using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject[] points; //starting position of platform should be same as position of first object in points[]
    public float speed = 0.5f;

    private Vector3[] positions; //array to contain positions of each gameobject in points[]
    private int _posCounter = 0; //this will track the current position of the platform
    private Vector3 _currPosition;
    private Vector3 _endPosition;
    private int _directionX;
    private int _directionY;
    private float _trackPercentX = 0f;
    private float _trackPercentY = 0f;

	// Use this for initialization
	void Start ()
    {
        //fill positions vector with position of each gameobject in points[]
        positions = new Vector3[points.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = points[i].transform.position;
        }

        //if _endPosition is to the right of _currPosition, then _directionX will be positive to start
        if (positions[_posCounter].x < positions[_posCounter + 1].x)
            _directionX = 1;
        else if (positions[_posCounter].x > positions[_posCounter + 1].x)
            _directionX = -1;
        else
            _directionX = 0;
        //if _endPosition is above _currPosition, then _directionY will be positive to start
        if (positions[_posCounter].y < positions[_posCounter + 1].y)
            _directionY = 1;
        else if (positions[_posCounter].y > positions[_posCounter + 1].y)
            _directionY = -1;
        else
            _directionY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _currPosition = positions[_posCounter];
        if (_posCounter + 1 >= positions.Length)
            _endPosition = positions[0];
        else
            _endPosition = positions[_posCounter + 1];

        _trackPercentX += _directionX * speed * Time.deltaTime;
        _trackPercentY += _directionY * speed * Time.deltaTime;

        float x = (Mathf.Abs(_endPosition.x - _currPosition.x)) * _trackPercentX + _currPosition.x;
        float y = (Mathf.Abs(_endPosition.y - _currPosition.y)) * _trackPercentY + _currPosition.y;
        transform.position = new Vector3(x, y, _currPosition.z);

        //platform will have arrived at destination when _trackPercentX and _trackPercentY are both around 1f
        //if direction is 0, then we do not need to make sure that _trackPercent is around 1f, since the platform doesn't need to move on that axis
        if ((Mathf.Abs(_trackPercentX) > .97f || _directionX == 0) && (Mathf.Abs(_trackPercentY) > .97f || _directionY == 0))
        {
            if (_posCounter + 1 >= positions.Length)
                _posCounter = 0;
            else
                _posCounter++;

            _trackPercentX = 0f;
            _trackPercentY = 0f;

            Vector3 tempEnd;
            if (_posCounter + 1 >= positions.Length)
            {
                tempEnd = positions[0];
            }
            else
            {
                tempEnd = positions[_posCounter + 1];
            }

            //if _endPosition is to the right of _currPosition, then _directionX will be positive to start
            if (positions[_posCounter].x < tempEnd.x)
                _directionX = 1;
            else if (positions[_posCounter].x > tempEnd.x)
                _directionX = -1;
            else
                _directionX = 0;
            //if _endPosition is above _currPosition, then _directionY will be positive to start
            if (positions[_posCounter].y < tempEnd.y)
                _directionY = 1;
            else if (positions[_posCounter].y > tempEnd.y)
                _directionY = -1;
            else
                _directionY = 0;
        }       
	}
}
