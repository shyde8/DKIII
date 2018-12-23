using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; set; }
    private Dictionary<string, int> _items;
    private bool debug = true;

    public void Startup()
    {
        _items = new Dictionary<string, int>();
        status = ManagerStatus.Started;
    }

    public void AddItem(string name)
    {
        if (_items.ContainsKey(name))
            _items[name] += 1;
        else
            _items[name]=1;

        if (debug)
            DisplayItems();
    }

    private void DisplayItems()
    {
        string itemDisplay = "Items: ";
        foreach(KeyValuePair<string, int> item in _items)
        {
            itemDisplay += item.Key + "(" + item.Value + ") ";
        }
        Debug.Log(itemDisplay);
    }

}
