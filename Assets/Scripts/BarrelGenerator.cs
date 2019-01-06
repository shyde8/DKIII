using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _barrelPrefab;
    [SerializeField]
    private GameObject _GenPoint1;
    [SerializeField]
    private GameObject _GenPoint2;
    [SerializeField]
    private GameObject _DestPoint1;
    [SerializeField]
    private GameObject _DestPoint2;
    private GameObject _barrel;

    private int _numFrames;
    public int framesBetweenBarrels = 400;

	// Use this for initialization
	void Start ()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        _numFrames++;
        if (_numFrames >= framesBetweenBarrels)
        {
            float rand = Random.Range(0, 4);

            if (rand == 0 || rand == 1)
            {
                _barrel = Instantiate(_barrelPrefab) as GameObject;
                _barrel.transform.position = _GenPoint1.transform.position;
            }
            else if (rand == 2)
            {
                _barrel = Instantiate(_barrelPrefab) as GameObject;
                _barrel.transform.position = _GenPoint2.transform.position;
                StartCoroutine(BarrelThrow(_barrel, _DestPoint1));
            }
            else if (rand == 3)
            {
                _barrel = Instantiate(_barrelPrefab) as GameObject;
                _barrel.transform.position = _GenPoint2.transform.position;
                StartCoroutine(BarrelThrow(_barrel, _DestPoint2));
            }            
            _numFrames = 0;
        }
	}

    private IEnumerator BarrelThrow(GameObject barrel, GameObject endPoint)
    {
        Vector2 start = barrel.transform.position;
        Vector2 end = new Vector2(endPoint.transform.position.x, endPoint.transform.position.y);
        Vector2 mid = new Vector2();
        mid = start + ((end - start) / 2) + Vector2.up * 1.0f;
        float count = 0.0f;
        while (count < 1.0f)
        {
            count += 2.0f * Time.deltaTime;
            Vector2 m1 = Vector2.Lerp(start, mid, count);
            Vector2 m2 = Vector2.Lerp(mid, end, count);
            barrel.transform.position = Vector2.Lerp(m1, m2, count);
            yield return new WaitForFixedUpdate();
        }
    }
}
