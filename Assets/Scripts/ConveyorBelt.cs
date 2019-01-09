using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 200f;
    public float maxVel = 3f;
    public bool flip = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D body = collision.gameObject.GetComponent<Rigidbody2D>();
        float deltaX = speed * Time.deltaTime;
        float tempMaxVel = maxVel;        

        if (Mathf.Sign(speed) == -1)
        {
            tempMaxVel *= -1;
        }
            

        Vector2 force = Vector2.right * speed;
        Debug.Log(force.x);
        body.AddForce(force);

        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            if ((collision.gameObject.GetComponent<PlayerMovement>().pubDeltaX > 0 && deltaX > 0) ||
                (collision.gameObject.GetComponent<PlayerMovement>().pubDeltaX < 0 && deltaX < 0))
            {
                if (Mathf.Sign(speed) == -1)
                    tempMaxVel -= 2.5f;
                else
                    tempMaxVel += 2.5f;
            }
        }
        else if (collision.gameObject.GetComponent<PlayerMovement>() == null)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(1 * Mathf.Sign(speed), 0);
        }

        if (Mathf.Abs(body.velocity.x) > Mathf.Abs(tempMaxVel))
        {
            body.velocity = new Vector2(tempMaxVel * Mathf.Sign(body.velocity.x), body.velocity.y);
        }

    }

    public void Flip()
    {
        speed *= -1;
        maxVel *= -1;
    }
}
