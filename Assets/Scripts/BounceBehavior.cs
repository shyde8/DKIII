using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBehavior : MonoBehaviour
{
    [SerializeField]
    private float _bounceJumpMultiplier = 5.0f;

    public float BounceJumpMultiplier
    {
        get
        {
            return _bounceJumpMultiplier;
        }
        set
        {
            _bounceJumpMultiplier = value;
        } 
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
