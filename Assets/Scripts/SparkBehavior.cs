using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkBehavior : MonoBehaviour
{
    private Vector3 _StartPos;
    public Vector3 _EndPos;
    //[SerializeField]
    //private GameObject _pole;
    [SerializeField]
    private float _speed = .5f;
    private float _trackPercent = 0f;

    // Use this for initialization
    void Start ()
    {
        _StartPos = transform.position;
        //_EndPos = _pole.GetComponent<ElectricPoleController>()._endMarker.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        _trackPercent += _speed * Time.deltaTime;
        float x = (_EndPos.x - _StartPos.x) * _trackPercent + _StartPos.x;
        float y = (_EndPos.y - _StartPos.y) * _trackPercent + _StartPos.y;
        transform.position = new Vector3(x, y, _StartPos.z);

        if (_trackPercent > .99f)
            Destroy(this.gameObject);
    }
}
