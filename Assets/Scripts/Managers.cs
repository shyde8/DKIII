using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(MissionManager))]

public class Managers : MonoBehaviour
{
    public static PlayerManager Player { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static MissionManager Mission { get; private set; }

    private List<IGameManager> _startSequence;

    public static Managers instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            Player = GetComponent<PlayerManager>();
            Inventory = GetComponent<InventoryManager>();
            Mission = GetComponent<MissionManager>();

            _startSequence = new List<IGameManager>();
            _startSequence.Add(Player);
            _startSequence.Add(Inventory);
            _startSequence.Add(Mission);

            StartCoroutine(StartupManagers());
        }            
        else if (instance != this)
        {
            Destroy(gameObject);
            Messenger.Broadcast(GameEvent.GAME_RESET);
        }
            
    }

    private IEnumerator StartupManagers()
    {
        foreach(IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach(IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                    numReady++;
            }

            yield return null;
        }
        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
        
    }
}
