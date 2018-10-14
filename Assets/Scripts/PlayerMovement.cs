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
    private bool _isGrounded = true;
    private bool _isClimbing = false;   

    //public variables
    public float speed = 150.0f;
    public float jumpForce = 5.7f;
    public float fakeGravity = 27f;
    public LayerMask ladder = 8;
    public LayerMask ground = 9;
    public float ladderDetectionDistance = 1f;
    public float climbSpeed = 5f;

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
        #region Horizontal Movement
        //only apply movement if either left or right arrow are down, to avoid "floaty" behavior
        float deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y);
		if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && _isGrounded && !_isClimbing)
        {
            _body.velocity = movement;
            _anim.SetBool("isHorKeyDown", true);
        }
        else if (_isGrounded && !_isClimbing)
        {
            _anim.SetBool("isHorKeyDown", false);
        }
        if (!Mathf.Approximately(deltaX, 0))
            transform.localScale = new Vector3(Mathf.Sign(deltaX) * _scaleX, _scaleY, _scaleZ);
        #endregion

        #region Jumping
        //check-if grounded
        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        _isGrounded = false;

        //player should only be "grounded" if the object below them is marked as ground layer
        if (hit != null && hit.GetComponent<Collider2D>().gameObject.layer==ground)
        {
            _isGrounded = true;
        }        

        //jumping
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space) && !_isClimbing)
        {
            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }

        //decrease gravity while in the air
        if (!_isGrounded && !_isClimbing)
        {
            Vector2 vel = _body.velocity;
            vel.y += fakeGravity * Time.deltaTime;
            _body.velocity = vel;
        }

        //jump animation
        if (!_isGrounded && !_isClimbing)
        {
            _anim.SetBool("isJumping", true);
        }
        else
        {
            _anim.SetBool("isJumping", false);
        }
        #endregion

        #region Ladder Movement
        //ladder code
        RaycastHit2D ladderHit = Physics2D.Raycast(transform.position, Vector2.up, ladderDetectionDistance, ladder);
        if (ladderHit.collider != null)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                _isClimbing = true;
        }
        else
        {
            _isClimbing = false;
        }

        if (_isClimbing)
        {
            _body.gravityScale = 0;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                float deltaY = Input.GetAxis("Vertical") * climbSpeed;
                Debug.Log(deltaY);
                Vector2 climbMovement = new Vector2(_body.velocity.x, deltaY);
                _body.velocity = climbMovement;
                Debug.Log("set velocity");
            }
        }
        #endregion 
    }
}
