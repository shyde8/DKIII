using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSpriteRenderer : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}
