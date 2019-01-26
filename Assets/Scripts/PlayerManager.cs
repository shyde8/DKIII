using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    //UI elements
    public int numLives = 3;
    public int score = 0;
    public int highScore = 7650;
    public int level = 0;
    public int numCredits = 0;

    public void Startup()
    {
        status = ManagerStatus.Started;
    }

    public void OnEnemyJumped()
    {
        score += 100;
        AdjustHighScoreIfNeeded();
        Messenger.Broadcast(GameEvent.ENEMY_JUMPED);
    }

    private void AdjustHighScoreIfNeeded()
    {
        if(score > highScore)
        {
            highScore = score;
            Messenger.Broadcast(GameEvent.HIGHSCORE_ADJUSTED);
        }
    }

    public void AddTimerTime(int remainingTime)
    {
        score += remainingTime;
        AdjustHighScoreIfNeeded();
    }

    public void GameReset()
    {
        numLives = 3;
        score = 0;
        level = 0;
        Messenger.Broadcast(GameEvent.GAME_OVER);
    }
}
