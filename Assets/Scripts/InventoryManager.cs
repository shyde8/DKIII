using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; set; }
    private Dictionary<string, int> _items;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.LEVEL_FAILED, ClearInventory);
        Messenger.AddListener(GameEvent.GAME_RESET, ResetInventory);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.LEVEL_FAILED, ClearInventory);
        Messenger.RemoveListener(GameEvent.GAME_RESET, ResetInventory);
    }

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

        if(_items[name] == 4)
        {
            ClearInventory();
            Managers.Mission.ReachObjective();
        }
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

    private void ClearInventory()
    {
        _items.Clear();
    }

    private void ResetInventory()
    {
        _items = new Dictionary<string, int>();
    }
}
