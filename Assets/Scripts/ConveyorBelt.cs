using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private float speed = 20f;
    public float velItem = 3f;
    public float velChar = 3f;
    public bool flip = false;

    // Use this for initialization
    void Start()
    {
        if (flip)
        {
            speed *= -1;
            velItem *= -1;
            velChar *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D body = collision.gameObject.GetComponent<Rigidbody2D>();
        float deltaX = speed * Time.deltaTime;
        float tempVel = velChar;

        //jumpman
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Vector2 force = Vector2.right * speed;
            body.AddForce(force);
            if ((collision.gameObject.GetComponent<PlayerMovement>().pubDeltaX > 0 && deltaX > 0) || (collision.gameObject.GetComponent<PlayerMovement>().pubDeltaX < 0 && deltaX < 0))
            {
                if (Mathf.Sign(speed) == -1)
                    tempVel -= 2.5f;
                else
                    tempVel += 2.5f;
            }
            if (Mathf.Abs(body.velocity.x) > Mathf.Abs(tempVel))
            {
                body.velocity = new Vector2(tempVel, body.velocity.y);
            }
        }

        //taxi and pie
        else if (collision.gameObject.GetComponent<PlayerMovement>() == null)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
            if (Mathf.Abs(body.velocity.x) > Mathf.Abs(velItem))
            {
                body.velocity = new Vector2(velItem, body.velocity.y);
            }
        }
    }

    public void Flip()
    {
        speed *= -1;
        velItem *= -1;
        velChar *= -1;
    }
}
