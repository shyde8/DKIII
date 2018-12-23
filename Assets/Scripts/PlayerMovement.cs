using System.Collections;
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
    private bool _isJumping = false; //added on 11-17-2018, we'll use this variable to determine whether player can jump, rather than checking _isGrounded
    private bool _isClimbing = false;
    private bool _isCappyJumping = false;
    private bool _isBouncing = false;
    private bool _isDoubleJumping = false;
    private bool _isWallJumping = false;
    private int _framesSinceJump = 0;
    private GameObject _cappy;

    //public variables
    public float speed = 150.0f;
    public float jumpForce = 5.7f;
    public float wallSlideSpeed = -1f;
    public float wallJumpForce = 2.5f;
    public float cappyJumpMultiplier = 1.4f; //this is multiplied against the jumpForce variable
    public float cappyThrowBurstMultiplier = 3f;
    public float fakeGravity = 27f;
    public bool enableDoubleJump = false;
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
        float deltaX = 0;
        deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y);
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && (_isGrounded || _isBouncing) && !_isClimbing)
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
        Vector2 corner1 = new Vector2(max.x, min.y - .065f);
        Vector2 corner2 = new Vector2(min.x, min.y - .065f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        _isGrounded = false;

        //player should only be "grounded" if the object below them is marked as ground layer (note: currently we're using a hard-coded value of 9 to check equality)
        //_framesSinceJump is an awful hack to avoid a scenario where jumpman immediately becomes Grounded after jumping
        //12-23, you should only be considered grounded if your vertical velocity is close to 0
        if (hit != null && hit.GetComponent<Collider2D>().gameObject.layer == 9 && _framesSinceJump>10 && (Mathf.Abs(_body.velocity.y)<.05))
        {                         
            _isGrounded = true;
            _isJumping = false;
            _isCappyJumping = false;
            _isBouncing = false;
        }

        //jumping
        bool _jumpedInFrame = false;
        if (!_isJumping && Input.GetKeyDown(KeyCode.Space) && !_isClimbing && !_isBouncing && !_isWallJumping)
        {
            _framesSinceJump = 0;
            //11-25 zero out y-velocity prior to jump?
            Vector2 currVel = _body.velocity;
            currVel.y = 0;
            _body.velocity = currVel;

            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _isJumping = true;
            _isGrounded = false;
            _jumpedInFrame = true;
        }
        _framesSinceJump++;

        //decrease gravity while in the air
        if (!_isGrounded && !_isClimbing && (_isJumping || _isBouncing || _isWallJumping))
        {                    
            Vector2 vel = _body.velocity;
            vel.y += fakeGravity * Time.deltaTime;
            _body.velocity = vel;            
        }

        //jump animation
        if (!_isGrounded && !_isClimbing && (_isJumping || _isBouncing || _isWallJumping))
        {
            _anim.SetBool("isJumping", true);
        }
        else
        {
            _anim.SetBool("isJumping", false);
        }
        #endregion

        #region Wall Jump
        //Vector2 corner1 = new Vector2(max.x, min.y - .06f);
        //Vector2 corner2 = new Vector2(min.x, min.y - .06f);
        Vector2 corner3 = new Vector2(min.x, max.y - .06f);
        Vector2 corner4 = new Vector2(max.x, max.y - .06f);
        Collider2D wallLeft = Physics2D.OverlapArea(corner2, corner3, 1 << LayerMask.NameToLayer("Wall"));
        Collider2D wallRight = Physics2D.OverlapArea(corner1, corner4, 1 << LayerMask.NameToLayer("Wall"));

        if ((wallLeft != null && Input.GetKey(KeyCode.LeftArrow) && !_isGrounded) || (wallRight != null && Input.GetKey(KeyCode.RightArrow) && !_isGrounded))
            _isWallJumping = true;
        else
            _isWallJumping = false;

        if (_isWallJumping)
        {            
            if (_body.velocity.y < wallSlideSpeed)
            {
                Vector2 tempVel = new Vector2(_body.velocity.x, wallSlideSpeed);
                _body.velocity = tempVel;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _jumpedInFrame == false)
            {
                Vector2 currVel = _body.velocity;
                currVel.y = 0;
                currVel.x = wallJumpForce * (Mathf.Sign(transform.localScale.x)*-1);
                _body.velocity = currVel;
                _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        #endregion

        #region Moving Platform
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

        #endregion

        #region Cappy Movement
        if (hit != null)
        {            
            if (hit.GetComponent<CappyMovement>() != null && !_isCappyJumping)
            {
                //before applying additional upward force, set velocity.y to 0, to make all bounces off cappy equal no matter where you are in jump arc
                Vector2 currVel = _body.velocity;
                currVel.y = 0;
                //11-28, hard-coding x-velocity on cappy jump to 2.6
                float dir = Mathf.Sign(transform.localScale.x);
                currVel.x = dir * 2.6f;
                _body.velocity = currVel;
                _isCappyJumping = true;                
                _body.AddForce(Vector2.up * (jumpForce*cappyJumpMultiplier), ForceMode2D.Impulse);
            }
        }
        #endregion

        #region Double Jump
        //https://gamedev.stackexchange.com/questions/157642/moving-a-2d-object-along-circular-arc-between-two-points
        _cappy = GameObject.Find("Cappy(Clone)");
        if (_cappy != null && _cappy.GetComponent<CappyMovement>() != null && _cappy.GetComponent<CappyMovement>().IsHovering() == true && enableDoubleJump)
        {
            if (_isJumping && !_jumpedInFrame && !_isDoubleJumping && Input.GetKeyDown(KeyCode.Space))
            {
                _isDoubleJumping = true;
                StartCoroutine(DoubleJump(gameObject, _cappy));
            }
        }

        #endregion

        #region BounceBehavior
        if (hit != null)
        {
            if (hit.GetComponent<BounceBehavior>() != null)
            {
                //before applying additional upward force, set velocity.y to 0, to make all bounces off BounceBehaviors are equal no matter where you are in jump arc
                Vector2 currVel = _body.velocity;
                currVel.y = 0;                
                _body.velocity = currVel;
                _body.AddForce(Vector2.up * (jumpForce * hit.GetComponent<BounceBehavior>().BounceJumpMultiplier), ForceMode2D.Impulse);
                _isBouncing = true;
            }
        }
        #endregion

        #region Ladder Movement
        RaycastHit2D ladderUp = Physics2D.Raycast(transform.position, Vector2.up, ladderDetectionDistance, 1 << LayerMask.NameToLayer("Ladder"));
        RaycastHit2D ladderDown = Physics2D.Raycast(transform.position, Vector2.down, ladderDetectionDistance + 1f, 1 << LayerMask.NameToLayer("Ladder"));
        //12-16-2018, removed grounded check so you can jump into ladder
        if (ladderUp.collider != null && Input.GetKey(KeyCode.UpArrow)/* && _isGrounded*/)
        {
            _isClimbing = true;
            _body.gravityScale = 0; //gravity should not apply to jumpman while climbing                
                                    //set x position to that of the ladder, to "lock" the player into it
            Vector2 newPos = new Vector2(ladderUp.transform.position.x, transform.position.y);
            _body.MovePosition(newPos);
            //apply initial small burst upward if we're grounded, so we're no longer grounded after initially entering climbing mode
            if (_isGrounded)
            {
                _body.AddForce(Vector2.up * climbSpeed, ForceMode2D.Impulse);
                _isGrounded = false; //11-17-2018, added hard-coding of _isGrounded to false once you enter climbing mode
            }                
        }
        else if (ladderDown.collider != null && Input.GetKey(KeyCode.DownArrow)/* && _isGrounded*/)
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
            {
                _body.AddForce(Vector2.down * climbSpeed, ForceMode2D.Impulse);
                _isGrounded = false; //11-17-2018, added hard-coding of _isGrounded to false once you enter climbing mode
            }                
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
            Vector2 startPos = new Vector2(transform.position.x, _box.bounds.min.y - .05f);
            RaycastHit2D platformHit = Physics2D.Raycast(startPos, Vector2.down, PLATFORM_THICKNESS, 1 << LayerMask.NameToLayer("Ladder"));
            //12-16-2018, if climbing downward, then exit climbing mode as soon as not touching a ladder
            Vector2 topMid = new Vector2(transform.position.x, _box.bounds.max.y - .1f);
            RaycastHit2D detectAbove = Physics2D.Raycast(topMid, Vector2.up, .01f, 1 << LayerMask.NameToLayer("Ladder"));
            if ((_isGrounded && platformHit.collider == null) || (platformHit.collider == null && detectAbove.collider == null))
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
        if (_isGrounded && collision.gameObject.GetComponent<StairBehavior>())
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

    public void CappyThrowBurst()
    {
        if (!_isGrounded && _isJumping)
        {
            Vector2 currVel = _body.velocity;
            currVel.y = 0;
            _body.velocity = currVel;
            _body.AddForce(Vector2.up * cappyThrowBurstMultiplier, ForceMode2D.Impulse);
        }
    }


    private IEnumerator DoubleJump(GameObject _jumpMan, GameObject _cappy)
    {
        Vector2 start = _jumpMan.transform.position;
        Vector2 end = new Vector2(_cappy.transform.position.x, _cappy.GetComponent<BoxCollider2D>().bounds.max.y + 0.3f); //_cappy.transform.position; //end-position should be the top-center of cappy
        Vector2 mid = new Vector2();
        mid = start + ((end - start) / 2) + Vector2.up * 1.0f;
        float count = 0.0f;
        while(count < 1.0f)
        {
            count += 2.0f * Time.deltaTime;
            Vector2 m1 = Vector2.Lerp(start, mid, count);
            Vector2 m2 = Vector2.Lerp(mid, end, count);
            gameObject.transform.position = Vector2.Lerp(m1, m2, count);
            yield return new WaitForFixedUpdate();
        }        
        _isDoubleJumping = false;
    }

    public bool IsDoubleJumping()
    {
        return _isDoubleJumping;
    }

    public bool IsWallJumping()
    {
        return _isWallJumping;
    }
}
