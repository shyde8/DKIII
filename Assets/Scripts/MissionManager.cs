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
        if (curLevel < maxLevel)
        {
            curLevel++;
            string name = "Level" + curLevel;
            SceneManager.LoadScene(name);
        }
    }

    public void ReachObjective()
    {
        //add bonus timer to score
        GameObject uiController = GameObject.Find("UIController");
        int remainingTime = uiController.GetComponent<UIController>().timerCountdown;
        Managers.Player.AddTimerTime(remainingTime);

        //disable all enemies and enemy generators in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }

        //disable main camera's audio source
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<AudioSource>().enabled = false;
        Messenger.Broadcast(GameEvent.LEVEL_COMPLETE);

        //disable jumpman
        GameObject jumpman = GameObject.FindGameObjectWithTag("Player");
        jumpman.GetComponent<PlayerMovement>().enabled = false;

        StartCoroutine(PauseAndAdvance());
    }

    private IEnumerator PauseAndAdvance()
    {
        yield return new WaitForSeconds(3);
        GoToNext();
    }

    public void LevelFailed()
    {
        Managers.Player.numLives--;
        if(Managers.Player.numLives > 0)
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

    private void RestartScene()
    {
        string name = "Level" + curLevel;
        SceneManager.LoadScene(name);
    }



}
