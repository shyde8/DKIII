using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    private int numLives = 3;

    public void Startup()
    {
        status = ManagerStatus.Started;
    }

    public int NumberOfLives()
    {
        return numLives;
    }



}
