﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private float _defGravityScale;
    private bool _isGrounded = true;
    private bool _isClimbing = false;
    private bool _isCappyJumping = false;

    //public variables
    public float speed = 150.0f;
    public float jumpForce = 5.7f;
    public float cappyJumpMultiplier = 1.4f; //this is multiplied against the jumpForce variable
    public float fakeGravity = 27f;
    public LayerMask ladder = 8;
    public LayerMask ground = 9;
    public float ladderDetectionDistance = 1f;
    public float climbSpeed = 5f;
    public const float PLATFORM_THICKNESS = 0.4f;

    // Use this for initialization
    void Start()
    {
        //get gameobject components
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _box = GetComponent<BoxCollider2D>();

        //set local scales
        _scaleX = transform.localScale.x;
        _scaleY = transform.localScale.y;
        _scaleZ = transform.localScale.z;

        //grab default gravity scale
        _defGravityScale = _body.gravityScale;
    }

    // Update is called once per frame
    void Update()
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
        Vector2 corner1 = new Vector2(max.x, min.y - .06f);
        Vector2 corner2 = new Vector2(min.x, min.y - .06f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        _isGrounded = false;

        //player should only be "grounded" if the object below them is marked as ground layer (note: currently we're using a hard-coded value of 9 to check equality)
        if (hit != null && hit.GetComponent<Collider2D>().gameObject.layer == 9)
        {
            _isGrounded = true;
            _isCappyJumping = false;
        }

        //parent jumpman to object below him if it contains a MovingPlatform script, and perform counter-scaling if needed
        MovingPlatform platform = null;
        Vector3 pScale = Vector3.one;
        if (hit != null && _isGrounded == true)        
            platform = hit.GetComponent<MovingPlatform>();
        if (platform != null)
        {
            transform.parent = platform.transform;
            pScale = platform.transform.localScale;
        }            
        else
            transform.parent = null;
        if (deltaX != 0)
        {
            transform.localScale = new Vector3((Mathf.Sign(deltaX) * _scaleX) / pScale.x, _scaleY / pScale.y, _scaleZ);
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

        #region Cappy Movement
        if (hit != null)
        {            
            if (hit.GetComponent<CappyMovement>() != null && !_isCappyJumping)
            {
                //before applying additional upward force, set velocity.y to 0, to make all bounces off cappy equal no matter where you are in jump arc
                Vector2 currVel = _body.velocity;
                currVel.y = 0;
                _body.velocity = currVel;

                _isCappyJumping = true;                
                _body.AddForce(Vector2.up * (jumpForce*cappyJumpMultiplier), ForceMode2D.Impulse);
            }
        }
        #endregion

        #region Ladder Movement
        RaycastHit2D ladderUp = Physics2D.Raycast(transform.position, Vector2.up, ladderDetectionDistance, ladder);
        RaycastHit2D ladderDown = Physics2D.Raycast(transform.position, Vector2.down, ladderDetectionDistance + 1f, ladder);
        if (ladderUp.collider != null && Input.GetKey(KeyCode.UpArrow) && _isGrounded)
        {
            _isClimbing = true;
            _body.gravityScale = 0; //gravity should not apply to jumpman while climbing                
                                    //set x position to that of the ladder, to "lock" the player into it
            Vector2 newPos = new Vector2(ladderUp.transform.position.x, transform.position.y);
            _body.MovePosition(newPos);
            //apply initial small burst upward if we're grounded, so we're no longer grounded after initially entering climbing mode
            if (_isGrounded)
                _body.AddForce(Vector2.up * climbSpeed, ForceMode2D.Impulse);
        }
        else if (ladderDown.collider != null && Input.GetKey(KeyCode.DownArrow) && _isGrounded)
        {          
            _isClimbing = true;
            _body.gravityScale = 0; //gravity should not apply to jumpman while climbing                
                                    //set x position to that of the ladder, to "lock" the player into it
            Vector2 newPos = new Vector2(ladderDown.transform.position.x, transform.position.y);
            _body.MovePosition(newPos);

            //only set isTrigger to true if there's no ladder immediately above you
            if (hit != null && ladderUp.collider == null)
                hit.gameObject.GetComponent<Collider2D>().isTrigger = true;
            //apply initial small burst downward if we're grounded, so we're no longer grounded after initially entering climbing mode
            if (_isGrounded)
                _body.AddForce(Vector2.down * climbSpeed, ForceMode2D.Impulse);
        }

        if (_isClimbing)
        {
            _body.velocity = Vector3.zero; //set velocity to zero in each frame in climbing-mode to eliminate momentum
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _body.AddForce(Vector2.up * climbSpeed, ForceMode2D.Impulse);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                _body.AddForce(Vector2.down * climbSpeed, ForceMode2D.Impulse);
            }
            //exit climbing mode if touching the ground, and if a downward raycast from bottom of jumpman offset by platform thickness is not touching a ladder
            //11-16-2018, subtracted .04f from y-position of startPos, since there were instances where the raycast was still detecting the ladder
            Vector2 startPos = new Vector2(transform.position.x, _box.bounds.min.y - .04f);
            RaycastHit2D platformHit = Physics2D.Raycast(startPos, Vector2.down, PLATFORM_THICKNESS, 1 << LayerMask.NameToLayer("Ladder"));
            Debug.DrawRay(startPos, Vector2.down, Color.white, 5);
            //if (platformHit != null)
            //    Debug.Log(platformHit.collider.gameObject.name);
            if (_isGrounded && platformHit.collider == null)
            {
                _isClimbing = false;
            }
        }

        else if (!_isClimbing)
        {
            _body.gravityScale = _defGravityScale;
        }

        #endregion

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //set isTrigger for hit == false, only for ground
        if (collision.GetComponent<Collider2D>().gameObject.layer == 9 && collision.gameObject.GetComponent<Collider2D>().isTrigger == true)
            collision.gameObject.GetComponent<Collider2D>().isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //the below code allows jumpman to walk "up" platforms that are not exactly equal in height
        if (_isGrounded && collision.gameObject.GetComponent<StairBehavior>() != null)
        {
            Vector2 bottomRight = new Vector2(_box.bounds.max.x, _box.bounds.min.y);
            Vector2 bottomLeft = new Vector2(_box.bounds.min.x, _box.bounds.min.y);
            float dir = Mathf.Sign(transform.localScale.x);
            //raycasts currently hard-coded to use 9 (ground layer) for LayerMask bitwise operation
            RaycastHit2D platformRight = Physics2D.Raycast(bottomRight, Vector2.right, 0.1f, 1 << 9);
            RaycastHit2D platformLeft = Physics2D.Raycast(bottomLeft, Vector2.left, 0.1f, 1 << 9);
            float currentHeight = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
            if (dir > 0)
            {
                if (platformRight.collider != null && platformRight.collider.gameObject.GetComponent<StairBehavior>() != null)
                {
                    Vector3 newPos = new Vector3((transform.position.x + .02f), platformRight.collider.bounds.max.y + ((currentHeight / 2) + .02f));
                    transform.position = newPos;
                }
            }
            else if (dir < 0)
            {
                if (platformLeft.collider != null && platformLeft.collider.gameObject.GetComponent<StairBehavior>() != null)
                {
                    Vector3 newPos = new Vector3((transform.position.x - .02f), platformLeft.collider.bounds.max.y + ((currentHeight / 2) + .02f));
                    transform.position = newPos;
                }
            }            
        }
    }
}
