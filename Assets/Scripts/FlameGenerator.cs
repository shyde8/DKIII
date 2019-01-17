using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _flame;
    public int frequency = 1000;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float rand = Random.Range(0, frequency+1);
        if(rand == frequency)
        {
            GameObject flame = Instantiate(_flame) as GameObject;
            _flame.transform.position = transform.position;
        }
	}
}
