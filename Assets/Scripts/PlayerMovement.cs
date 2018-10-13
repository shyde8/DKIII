using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Current Gravity settings in-place:
 * Projects -> Physics 2D -> Gravity Y = -40
 * Gravity Scale = 1
 * Speed = 150
 * Jump Force = 5.7
 * Fake Gravity = 27
*/

public class PlayerMovement : MonoBehaviour
{
    //gameobject components
    private Rigidbody2D _body;
    private Animator _anim;
    private BoxCollider2D _box;

    //private variables
    private float _scaleX;
    private float _scaleY;
    private float _scaleZ;   

    //public variables
    public float speed = 250.0f;
    public float jumpForce = 12.0f;
    public float fakeGravity = 20f;

    // Use this for initialization
    void Start ()
    {
        //get gameobject components
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _box = GetComponent<BoxCollider2D>();

        //set local scales
        _scaleX = transform.localScale.x;
        _scaleY = transform.localScale.y;
        _scaleZ = transform.localScale.z;

	}
	
	// Update is called once per frame
	void Update ()
    {
        //horizontal movement
		//only apply movement if either left or right arrow are down, to avoid "floaty" behavior
		float deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y);
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        	_body.velocity = movement;


        //check-if grounded
        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        bool grounded = false;

        if (hit != null)
        {
            grounded = true;
        }        

        //jumping
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }

        //decrease gravity while in the air
        if (!grounded)
        {
            Vector2 vel = _body.velocity;
            vel.y += fakeGravity * Time.deltaTime;
            _body.velocity = vel;
        }

        //animator code
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
		{
			_anim.SetBool("isHorKeyDown", true);
		}
		else 
		{
			_anim.SetBool("isHorKeyDown", false);
		}
        if (!Mathf.Approximately(deltaX, 0))
            transform.localScale = new Vector3(Mathf.Sign(deltaX) * _scaleX, _scaleY, _scaleZ);

        if (!grounded)
        {
            _anim.SetBool("isJumping", true);
        }
        else
        {
            _anim.SetBool("isJumping", false);
        }
            
	}
}
