using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameMovement : MonoBehaviour
{
    /*todo:
     * Flame should not deliberately fall over an edge -- it should switch direction
     * Climb ladder
     * Floaty behavior when Flame goes up a step
     * Sprite/Animation
     * Optimize switching direction
     * React to hammer
     */
    private BoxCollider2D _box;
    private Rigidbody2D _body;
    private int _dir = -1;
    private float _scaleX;
    private float _scaleY;
    private float _scaleZ;

    public float speed = 155f; //flame should be slightly faster than jumpman
    

    //Use this for initialization
    void Start ()
    {
        _box = GetComponent<BoxCollider2D>();
        _body = GetComponent<Rigidbody2D>();

        //set local scales
        _scaleX = transform.localScale.x;
        _scaleY = transform.localScale.y;
        _scaleZ = transform.localScale.z;
    }
	
	//Update is called once per frame
	void Update ()
    {
        #region HorizontalMovement
        //Flame needs ability to switch directions.  For now, we'll just use a random number generator 
        int randomNumber = Random.Range(1, 31);
        if (randomNumber == 30)
            _dir *= -1;

        float deltaX = _dir * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, 0);
        _body.velocity = movement;
        if(deltaX != 0)
            transform.localScale = new Vector3(Mathf.Sign(deltaX) * _scaleX, _scaleY, _scaleZ);
        #endregion
    }

    //the below code was pulled from PlayerMovement, though currently it omits the _isGrounded check, since it's generally assumed a Flame will be grounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<StairBehavior>() != null)
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
