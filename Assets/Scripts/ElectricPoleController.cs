using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPoleController : MonoBehaviour
{
    [SerializeField]
    private GameObject _spark;
    private List<GameObject> _sparkList;
    [SerializeField]
    private GameObject _startMarker;
    [SerializeField]
    private GameObject _endMarker;
    private bool _shouldGenerate = true;
    public float sparkGenSpeed = 4f;
	// Use this for initialization
	void Start ()
    {
        _sparkList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_shouldGenerate)
            StartCoroutine(Generate());
	}

    private IEnumerator Generate()
    {
        _shouldGenerate = false;
        GameObject _newSpark = Instantiate(_spark) as GameObject;
        _newSpark.GetComponent<SparkBehavior>()._EndPos = _endMarker.transform.position;
        _newSpark.transform.position = _startMarker.transform.position;        
        _sparkList.Add(_newSpark);
        for (int i = 0; i < sparkGenSpeed; i++)
        {            
            yield return new WaitForSeconds(1);
        }
        _shouldGenerate = true;
    }
}
