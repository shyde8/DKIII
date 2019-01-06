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
    private float sparkGenSpeed = 2.5f;
    private float secondaryWaitSpeed;
    [SerializeField]
    private bool isBigPole;
    // Use this for initialization
    void Start()
    {
        _sparkList = new List<GameObject>();
        if (isBigPole)
            secondaryWaitSpeed = .85f;
        else
            secondaryWaitSpeed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_shouldGenerate)
            StartCoroutine(Generate(false));
    }

    private IEnumerator Generate(bool secondCall)
    {
        _shouldGenerate = false;
        GameObject _newSpark = Instantiate(_spark) as GameObject;
        _newSpark.GetComponent<SparkBehavior>()._EndPos = _endMarker.transform.position;
        _newSpark.transform.position = _startMarker.transform.position;
        _sparkList.Add(_newSpark);

        float rand = Random.Range(0, 2);
        if (rand == 1 && !secondCall)
        {
            yield return new WaitForSeconds(secondaryWaitSpeed);
            StartCoroutine(Generate(true));
        }

        //we will ALWAYS wait the specified amount in sparkGenSpeed before calling the co-routine again
        yield return new WaitForSeconds(sparkGenSpeed);
        _shouldGenerate = true;
    }
}
