using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Text scoreLabel;

    [SerializeField]
    private Text highScoreLabel;

    public int timerCountdown = 7000;
    [SerializeField]
    private Text timerCountdownLabel;

    [SerializeField]
    private Text levelLabel;

    private int _numLives;
    [SerializeField]
    private Image _firstLive;
    [SerializeField]
    private Image _secondLive;
    [SerializeField]
    private Image _thirdLive;

    //private GameObject _Manager;

    private AudioSource _source;
    [SerializeField]
    private AudioClip _levelCompleteClip;

    private int _framesBetweenClockDecrement = 0;
    private int _framesBetweenClockDecrementThreshold = 120; //game runs at approximately 60fps, so decrement clock every ~2 seconds

    private void Awake()
    {
        Messenger.AddListener(GameEvent.ENEMY_JUMPED, RefreshScore);
        Messenger.AddListener(GameEvent.HIGHSCORE_ADJUSTED, RefreshHighScore);
        Messenger.AddListener(GameEvent.LEVEL_INCREASED, RefreshLevelLabel);
        Messenger.AddListener(GameEvent.LIVE_LOST, DisplayLives);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_JUMPED, RefreshScore);
        Messenger.RemoveListener(GameEvent.HIGHSCORE_ADJUSTED, RefreshHighScore);
        Messenger.RemoveListener(GameEvent.LEVEL_INCREASED, RefreshLevelLabel);
        Messenger.RemoveListener(GameEvent.LIVE_LOST, DisplayLives);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
    }

    // Use this for initialization
    void Start ()
    {
        _source = GetComponent<AudioSource>();
        scoreLabel.text = Managers.Player.score.ToString("D6");
        highScoreLabel.text = Managers.Player.highScore.ToString("D6");
        timerCountdownLabel.text = timerCountdown.ToString();
        levelLabel.text = Managers.Player.level.ToString("D2");
        _numLives = Managers.Player.numLives;
        DisplayLives();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //adjust countdown if needed
        _framesBetweenClockDecrement++;
        if(_framesBetweenClockDecrement >= _framesBetweenClockDecrementThreshold)
        {
            _framesBetweenClockDecrement = 0;
            timerCountdown -= 100;
            timerCountdownLabel.text = timerCountdown.ToString();
        }
    }

    void RefreshScore()
    {
        scoreLabel.text = Managers.Player.score.ToString("D6");
    }

    void RefreshHighScore()
    {
        highScoreLabel.text = Managers.Player.highScore.ToString("D6");
    }

    void RefreshLevelLabel()
    {
        levelLabel.text = Managers.Player.level.ToString("D2");
    }

    void DisplayLives()
    {
        int numLives = Managers.Player.numLives;

        _firstLive.enabled = true;
        _secondLive.enabled = true;
        _thirdLive.enabled = true;

        if(numLives == 2)
        {
            _thirdLive.enabled = false;
        }
        if(numLives == 1)
        {
            _thirdLive.enabled = false;
            _secondLive.enabled = false;
        }
    }

    void OnLevelComplete()
    {
        RefreshScore();
        RefreshHighScore();
        _source.PlayOneShot(_levelCompleteClip);
    }

    private void OnGameOver()
    {
        RefreshScore();
        RefreshLevelLabel();
        DisplayLives();
        RefreshHighScore();         
    }
}
