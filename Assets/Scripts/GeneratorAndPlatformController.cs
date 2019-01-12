using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorAndPlatformController : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstGenerator;
    [SerializeField]
    private GameObject _secondGenerator;
    [SerializeField]
    private GameObject _topPlatform;
    [SerializeField]
    private GameObject _middlePlatform;
    [SerializeField]
    private GameObject _bottomPlatform;
    private GameObject[] platforms;
    private GameObject[] generators;

    public int minWaitTime = 8;
    private float waitTime = 0;
    public int waitAfterFlip = 3;
    public bool withinTwoSecondsOfFlip = false;

    private bool _isFlipping = false;

    // Use this for initialization
    void Start ()
    {
        platforms = new GameObject[] { _topPlatform, _middlePlatform, _bottomPlatform };
        generators = new GameObject[] { _firstGenerator, _secondGenerator };
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(waitTime == 0)
        {
            float rand = Random.Range(0, 10);
            waitTime = minWaitTime + rand;
        }

        if(!_isFlipping)
        {
            withinTwoSecondsOfFlip = false;
            StartCoroutine(Flip(waitTime));
            StartCoroutine(AdjustTimeUntilFlip(waitTime));            
        }
	}

    private IEnumerator Flip(float waitTime)
    {
        _isFlipping = true;
        yield return new WaitForSeconds(waitTime);
        
        //move platforms
        foreach(GameObject platform in platforms)
        {
            platform.GetComponent<MovingPlatform>().enabled = true;
            platform.GetComponent<ConveyorBelt>().Flip();
        }

        string enabledName = string.Empty;
        foreach (GameObject generator in generators)
        {
            if (generator.GetComponent<GeneratorBehavior>().enabled == true)
            {
                generator.GetComponent<GeneratorBehavior>().enabled = false;
                enabledName = generator.gameObject.name;
            }              
        }

        //after shifting platforms, wait for a few seconds before enabling the new generators
        yield return new WaitForSeconds(waitAfterFlip);

        foreach(GameObject generator in generators)
        {
            if (!string.Equals(generator.name, enabledName))
                generator.GetComponent<GeneratorBehavior>().enabled = true;
        }

        waitTime = 0;
        _isFlipping = false;
    }

    private IEnumerator AdjustTimeUntilFlip(float waitTime)
    {
        while(waitTime > 5)
        {
            yield return new WaitForSeconds(1);
            waitTime--;
        }
        withinTwoSecondsOfFlip = true;
    }
}
