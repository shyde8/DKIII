using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpDetector : MonoBehaviour
{
    [SerializeField]
    private AudioClip scoreSound;
    private AudioSource _audio;
    private BoxCollider2D _box;
    private float detectionDistance = .8f;
    private int framesBetweenScores = 0;
    private int framesBetweenScoresThreshold = 15;

	// Use this for initialization
	void Start ()
    {
        _box = GetComponent<BoxCollider2D>();
        _audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        framesBetweenScores++;
        Vector2 startPos = new Vector2(transform.position.x, _box.bounds.min.y);
        RaycastHit2D enemyHit = Physics2D.Raycast(startPos, Vector2.down, detectionDistance, 1 << LayerMask.NameToLayer("Enemy"));
        if(enemyHit.collider != null)
        {
            if (enemyHit.collider.gameObject.GetComponent<ReactiveEnemy>() != null)
            {
                if(framesBetweenScores >= framesBetweenScoresThreshold)
                {
                    Debug.Log("Here");
                    _audio.PlayOneShot(scoreSound);
                    framesBetweenScores = 0;
                }                
            }
        }        
	}
}
