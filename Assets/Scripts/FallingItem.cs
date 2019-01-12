using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItem : MonoBehaviour
{
    private BoxCollider2D _box;
    private Rigidbody2D _body;

	// Use this for initialization
	void Start ()
    {
        _box = GetComponent<BoxCollider2D>();
        _body = GetComponent<Rigidbody2D>();
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //check if grounded
        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - .065f);
        Vector2 corner2 = new Vector2(min.x, min.y - .065f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);

        if (hit == null)
            _body.velocity = new Vector2(0, _body.velocity.y);
            

    }
}
