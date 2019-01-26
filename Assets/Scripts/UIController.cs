using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Text scoreLabel;

    [SerializeField]
    private Text numCoins;

    [SerializeField]
    private Text highScoreLabel;

    public int timerCountdown = 5000;
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

    [SerializeField]
    private Sprite _deathSprite;

    [SerializeField]
    private Text _insertCoin;
    [SerializeField]
    private Text _credits;
    [SerializeField]
    private Text _numCredits;


    private AudioSource _source;
    [SerializeField]
    private AudioClip _levelCompleteClip;
    [SerializeField]
    private AudioClip _deathClip;
    [SerializeField]
    private AudioClip _hurryUp;

    private bool _hasSetHurryUp = false;
    private bool _hasFailed = false;

    private GameObject _jumpMan;
    private SpriteRenderer _jumpManSprite;

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
        Messenger.AddListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.AddListener(GameEvent.COIN_INSERTED, OnCoinInsert);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_JUMPED, RefreshScore);
        Messenger.RemoveListener(GameEvent.HIGHSCORE_ADJUSTED, RefreshHighScore);
        Messenger.RemoveListener(GameEvent.LEVEL_INCREASED, RefreshLevelLabel);
        Messenger.RemoveListener(GameEvent.LIVE_LOST, DisplayLives);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.RemoveListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.RemoveListener(GameEvent.COIN_INSERTED, OnCoinInsert);
    }

    // Use this for initialization
    void Start()
    {
        //disable InsertCoin, Credits, NumCredits if not Scene0
        if (SceneManager.GetActiveScene().name != "Level0")
        {
            _source = GetComponent<AudioSource>();
            scoreLabel.text = Managers.Player.score.ToString("D6");
            highScoreLabel.text = Managers.Player.highScore.ToString("D6");
            timerCountdownLabel.text = timerCountdown.ToString();
            levelLabel.text = Managers.Player.level.ToString("D2");
            _numLives = Managers.Player.numLives;
            DisplayLives();
            _jumpMan = GameObject.FindGameObjectWithTag("Player");
            _insertCoin.enabled = false;
            _credits.enabled = false;
            _numCredits.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Level0")
        {
            if (!_hasFailed)
            {
                //adjust countdown if needed
                _framesBetweenClockDecrement++;
                if (_framesBetweenClockDecrement >= _framesBetweenClockDecrementThreshold)
                {
                    _framesBetweenClockDecrement = 0;
                    timerCountdown -= 100;
                    timerCountdownLabel.text = timerCountdown.ToString();
                }

                if (timerCountdown == 1000 && !_hasSetHurryUp)
                {
                    _hasSetHurryUp = true;
                    GameObject camera = GameObject.Find("Main Camera");
                    camera.GetComponent<AudioSource>().clip = _hurryUp;
                    camera.GetComponent<AudioSource>().enabled = true;
                    camera.GetComponent<AudioSource>().Play();
                }

                if (timerCountdown == 0)
                {
                    Managers.Mission.LevelFailed();
                    _hasFailed = true;
                }
            }
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

        if (numLives == 2)
        {
            _thirdLive.enabled = false;
        }
        if (numLives == 1)
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

    private void OnLevelFailed()
    {
        _jumpMan.GetComponent<Animator>().enabled = false;
        _jumpMan.GetComponent<SpriteRenderer>().sprite = _deathSprite;
        StartCoroutine(DeathAnimation());
        _source.PlayOneShot(_deathClip);
    }

    private IEnumerator DeathAnimation()
    {
        double seconds = 0;
        while (seconds < 3)
        {
            Vector3 rot = new Vector3(0, 0, 90);
            _jumpMan.transform.Rotate(rot);
            yield return new WaitForSeconds(0.2f);
            seconds += 0.2;
        }
    }

    private void OnCoinInsert()
    {
        Managers.Player.numCredits++;
        numCoins.text = Managers.Player.numCredits.ToString("D2");
    }
}
