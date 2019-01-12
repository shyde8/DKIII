using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtAltitude : MonoBehaviour
{
    public float destroyY;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.y <= destroyY)
            Destroy(this.gameObject);
	}
}
