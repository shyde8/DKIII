using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject platform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Contains("Cappy"))
            platform.GetComponent<LeverControlledPlatform>().isActive = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Cappy"))
            platform.GetComponent<LeverControlledPlatform>().isActive = false;
    }
}
