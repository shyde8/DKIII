using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public int curLevel { get; private set; }
    public int maxLevel { get; private set; }

    public void Startup()
    {
        curLevel = 0;
        maxLevel = 3;

        status = ManagerStatus.Started;
    }

    public void GoToNext()
    {
        curLevel = 3;
        SceneManager.LoadScene("Level3");
        //if (curLevel < maxLevel)
        //{
        //    curLevel++;
        //    string name = "Level" + curLevel;
        //    SceneManager.LoadScene(name);
        //}
        //else
        //{
        //    Messenger.Broadcast(GameEvent.LEVEL_INCREASED);
        //    curLevel = 1;
        //    string name = "Level" + curLevel;
        //    SceneManager.LoadScene(name);
        //}
    }

    public void ReachObjective()
    {
        //add bonus timer to score
        GameObject uiController = GameObject.Find("UIController");
        int remainingTime = uiController.GetComponent<UIController>().timerCountdown;
        Managers.Player.AddTimerTime(remainingTime);

        DisableEnemiesInScene();
        FreezeJumpman();
        DisableAudio();

        Messenger.Broadcast(GameEvent.LEVEL_COMPLETE);

        StartCoroutine(PauseAndAdvance());
    }

    private IEnumerator PauseAndAdvance()
    {
        yield return new WaitForSeconds(3);
        GoToNext();
    }

    private IEnumerator AfterDeathRestartLevelIfPossible()
    {
        yield return new WaitForSeconds(5);

        if (Managers.Player.numLives > 0)
        {
            Messenger.Broadcast(GameEvent.LIVE_LOST);
            RestartScene();
        }
        else
        {
            Managers.Player.GameReset();
            curLevel = 0;
            SceneManager.LoadScene("Level0");
        }
    }

    public void LevelFailed()
    {
        Managers.Player.numLives--;

        //death animation
        DisableEnemiesInScene();
        FreezeJumpman();
        DisableAudio();
        Messenger.Broadcast(GameEvent.LEVEL_FAILED);

        StartCoroutine(AfterDeathRestartLevelIfPossible());        
    }

    private void RestartScene()
    {
        string name = "Level" + curLevel;
        SceneManager.LoadScene(name);
    }

    private void DisableEnemiesInScene()
    {
        //disable all enemies and enemy generators in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

    private void FreezeJumpman()
    {
        //disable jumpman
        GameObject jumpman = GameObject.FindGameObjectWithTag("Player");
        jumpman.GetComponent<PlayerMovement>().enabled = false;
    }

    private void DisableAudio()
    {
        //disable main camera's audio source
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<AudioSource>().enabled = false;
    }



}
