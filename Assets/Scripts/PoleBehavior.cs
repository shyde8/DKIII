using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleBehavior : MonoBehaviour
{
    private GameObject _jumpMan;
    private float _jumpManWidth;
    private float _jumpManHeight;
    private float _accumulatedTime;
    private float _maxAccumulatedTime = 1f;

    [SerializeField]
    private float _launchForce = 5f;

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
            //for now, we'll simply deactivate Jumpman and destroy Cappy
            _jumpMan.SetActive(false);
            Destroy(collision.gameObject);

            //calculate new position of jumpman, from which he'll launch from the pole
            Vector2 min = gameObject.GetComponent<BoxCollider2D>().bounds.min;
            Vector2 newPos = new Vector2(min.x - (_jumpManWidth/2), min.y + (_jumpManHeight/2));
            StartCoroutine(Launch(newPos));                   
        }
    }

    private IEnumerator Launch(Vector2 newPos)
    {
        _accumulatedTime = 0f;
        bool _hasPressedDown = false;
        while(true)
        {
            if(Input.GetKey(KeyCode.DownArrow))
            {
                _accumulatedTime += Time.deltaTime;
                _hasPressedDown = true;
                yield return new WaitForFixedUpdate();
            }
            else if (_hasPressedDown)
            {
                if (_accumulatedTime > _maxAccumulatedTime)
                    _accumulatedTime = _maxAccumulatedTime;

                _jumpMan.transform.position = newPos;
                _jumpMan.SetActive(true);
                float forceFactor = _accumulatedTime * _launchForce;
                _jumpMan.GetComponent<Rigidbody2D>().AddForce(Vector2.up * forceFactor, ForceMode2D.Impulse);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
