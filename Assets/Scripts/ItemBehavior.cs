using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    private const string MOON = "moon";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Contains("Jump"))
        {
            string name = string.Empty;
            if (this.gameObject.name.Contains(MOON))
                name = MOON;
            Managers.Inventory.AddItem(name);
            Destroy(this.gameObject);
        }
    }



}
