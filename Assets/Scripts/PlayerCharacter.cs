using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private GameObject cappyPrefab;
    private GameObject _cappy;
    private float _width;
    private float _height;

    private void Start()
    {
        _width = gameObject.GetComponent<Renderer>().bounds.size.x;
        _height = gameObject.GetComponent<Renderer>().bounds.size.y;
    }

    private void Update()
    {
        //instantiate cappy if fire2 button is pressed
        if (Input.GetAxis("Fire2") > 0f)
        {
            if (_cappy == null)
            {
                _cappy = Instantiate(cappyPrefab) as GameObject;
                float cappyHeight = _cappy.GetComponent<Renderer>().bounds.size.y;
                //scale is positive if jumpman is facing right, scale is negative if jumpman is facing left
                float dir = Mathf.Sign(transform.localScale.x);
                Vector2 newPos = new Vector2(transform.position.x + (dir * (_width/1.5f)), transform.position.y + (_height/2) - (cappyHeight/2));
                _cappy.transform.position = newPos;                
            }
        }                   
    }

    //for now, we just reset the scene when player touches a ReactiveEnemy
    void OnCollisionEnter2D(Collision2D col)
    {
        ReactiveEnemy enemy = col.gameObject.GetComponent<ReactiveEnemy>();
        if (enemy != null)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    


}
