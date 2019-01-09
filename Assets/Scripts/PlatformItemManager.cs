using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformItemManager : MonoBehaviour
{
    private List<GameObject> items;
	// Use this for initialization
	void Start ()
    {
        items = new List<GameObject>();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        items.Add(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        items.Remove(collision.gameObject);
    }

    public bool ContainsTaxi()
    {
        bool contains = false;
        foreach(GameObject obj in items)
        {
            if(obj.name.Contains("Taxi"))
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
}
