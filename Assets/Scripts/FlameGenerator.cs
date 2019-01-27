using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _flame;
    private int frequency = 7500;
    private GameObject platform;
	// Use this for initialization
	void Start ()
    {
        if (gameObject.name.Contains("Mid"))
            platform = GameObject.Find("BottomPlatform");
        if (gameObject.name.Contains("Top"))
            platform = GameObject.Find("MiddlePlatform");
        if (gameObject.name.Contains("High"))
            platform = GameObject.Find("TopPlatform");
    }
	
	// Update is called once per frame
	void Update ()
    {
        float rand = Random.Range(0, frequency+1);
        if(rand == frequency)
        {
            PlatformItemManager _item = platform.GetComponent<PlatformItemManager>();
            if (_item.NumItems() == 0)
            {
                GameObject flame = Instantiate(_flame) as GameObject;
                _flame.transform.position = transform.position;                
                Debug.Log("Generated Flame, Platform: " + platform.name + ", Count: " + _item.NumItems());
            }            
        }
	}
}
