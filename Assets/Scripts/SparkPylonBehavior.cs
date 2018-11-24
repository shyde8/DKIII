using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkPylonBehavior : MonoBehaviour
{
    private GameObject _jumpMan;    
    private float _jumpManWidth;
    private float _jumpManHeight;
    [SerializeField]
    private GameObject _brotherPylon;

    // Use this for initialization
    void Start ()
    {
        _jumpMan = GameObject.Find("Jumpman");
        _jumpManWidth = _jumpMan.GetComponent<SpriteRenderer>().bounds.size.x;
        _jumpManHeight = _jumpMan.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CappyMovement>() != null && collision.gameObject.GetComponent<CappyMovement>().IsReturning() == false)
        {
            Destroy(collision.gameObject);
            Vector2 min = _brotherPylon.GetComponent<BoxCollider2D>().bounds.min;
            Vector2 newPos = new Vector2(min.x - (_jumpManWidth / 2), min.y + (_jumpManHeight / 2));
            StartCoroutine(TransportJumpman(newPos,0));            
        }
    }

    private IEnumerator TransportJumpman(Vector2 newPos, int count)
    {
        _jumpMan.SetActive(false);
        while(count < 10)
        {            
            count++;
            yield return new WaitForFixedUpdate();
        }
        _jumpMan.transform.position = newPos;
        _jumpMan.SetActive(true);        
    }
}
