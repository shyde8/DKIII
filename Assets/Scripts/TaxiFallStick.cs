using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiFallStick : MonoBehaviour
{
    private Rigidbody2D _body;
	// Use this for initialization
	void Start ()
    {
        _body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<ConveyorBelt>() != null)
        {
            Debug.Log("Here");
            StartCoroutine(PauseForSecond());
        }
    }

    private IEnumerator PauseForSecond()
    {
        _body.constraints = RigidbodyConstraints2D.FreezePositionX;
        yield return new WaitForSeconds(0.2f);
        
        _body.constraints = RigidbodyConstraints2D.None;
    }
}
