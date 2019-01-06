using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour
{
    private SpriteRenderer _sprite;
    [SerializeField]
    private Sprite rollingSprite_1;
    [SerializeField]
    private Sprite rollingSprite_2;
    [SerializeField]
    private Sprite rollingSprite_3;
    [SerializeField]
    private Sprite rollingSprite_4;
    [SerializeField]
    private Sprite fallingSprite_1;
    [SerializeField]
    private Sprite fallingSprite_2;
    private Sprite[] rollingClips;
    private int rollSpriteIndex = 0;
    private int framesSinceRollSpriteSwap = 0;
    private int rollSpriteFrameSwapThreshold = 10;
    private int framesSinceFallSpriteSwap = 0;
    private int fallSpriteFrameSwapThreshold = 20;
    private float ladderDetectionDistance = 1f;

    private bool _isRolling = true;
    private bool _isFalling = false;

    private int _direction = 1;
    [SerializeField]
    private float _speed = 150;
    private Rigidbody2D _body;
    private CircleCollider2D _circle;
    private int _framesSinceStartedRolling = 0;
    private int _framesSinceStartedRollingThreshold = 10;


	// Use this for initialization
	void Start ()
    {
        _sprite = GetComponent<SpriteRenderer>();
        rollingClips = new Sprite[] { rollingSprite_1, rollingSprite_2, rollingSprite_3, rollingSprite_4};
        _body = GetComponent<Rigidbody2D>();
        _circle = GetComponent<CircleCollider2D>();
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //animation code
        if(_isRolling)
        {
            framesSinceRollSpriteSwap++;
            if (framesSinceRollSpriteSwap >= rollSpriteFrameSwapThreshold)
            {
                rollSpriteIndex++;
                if (rollSpriteIndex == 4)
                    rollSpriteIndex = 0;
                framesSinceRollSpriteSwap = 0;
                _sprite.sprite = rollingClips[rollSpriteIndex];
            }
        }
        
        if(_isFalling)
        {
            framesSinceFallSpriteSwap++;
            if (framesSinceFallSpriteSwap >= fallSpriteFrameSwapThreshold)
            {
                if (_sprite.sprite != fallingSprite_1)
                    _sprite.sprite = fallingSprite_1;
                else if (_sprite.sprite == fallingSprite_1)
                    _sprite.sprite = fallingSprite_2;
                framesSinceFallSpriteSwap = 0;
            }
        }
        
        if(_isRolling)
        {
            if (_body.gravityScale == 0)
                _body.gravityScale = 1;
            if (_circle.isTrigger == true)
                _circle.isTrigger = false;

            _framesSinceStartedRolling++;

            float deltaX = 0;
            deltaX = _speed * _direction * Time.deltaTime;
            Vector2 movement = new Vector2(deltaX, _body.velocity.y);
            _body.velocity = movement;

            RaycastHit2D ladderDown = Physics2D.Raycast(transform.position, Vector2.down, ladderDetectionDistance + 1f, 1 << LayerMask.NameToLayer("Ladder"));
            if (ladderDown.collider == null)
                ladderDown = Physics2D.Raycast(transform.position, Vector2.down, ladderDetectionDistance + 1f, 1 << LayerMask.NameToLayer("HalfLadder"));

            if(ladderDown.collider != null)
            {
                if(_framesSinceStartedRolling >= _framesSinceStartedRollingThreshold)
                {
                    if (Mathf.Abs(transform.position.x - ladderDown.collider.gameObject.transform.position.x) < .02)
                    {
                        _body.velocity = Vector3.zero;
                        Vector3 newPos = new Vector3(ladderDown.collider.gameObject.transform.position.x, transform.position.y);
                        transform.position = newPos;
                        _isRolling = false;
                        _isFalling = true;
                        _circle.isTrigger = true;
                        _body.gravityScale = 0;

                    }
                }                             
            }
        }

        if(_isFalling)
        {
            Vector3 newPos = new Vector3(transform.position.x, (transform.position.y - .05f));
            transform.position = newPos;
        }

        if (transform.position.y < -6)
            Destroy(this.gameObject);     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<BarrelDirector>() != null)
        {
            _isFalling = false;
            _isRolling = true;
            _direction = collision.gameObject.GetComponent<BarrelDirector>().direction;
            _circle.isTrigger = false;
            _body.gravityScale = 1;

            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + .1f);
            transform.position = newPos;
            _framesSinceStartedRolling = 0;
        }
    }
}
