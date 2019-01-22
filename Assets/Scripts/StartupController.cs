using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupController : MonoBehaviour
{
    [SerializeField]
    private Text _coinsLabel;
    private int _numCoins = 0;

    [SerializeField]
    private Text _highScoreLabel;

    private AudioSource _audio;
    [SerializeField]
    private AudioClip _startupClip;
    [SerializeField]
    private AudioClip _coinInsert;
    [SerializeField]
    private AudioClip _gameStart;

    private bool _isGameStarted;

    private void Awake()
    {
        Messenger.AddListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
        Messenger.AddListener(GameEvent.COIN_INSERTED, OnCoinInsert);
        Messenger.AddListener(GameEvent.GAME_RESET, OnReset);
        _audio = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        Messenger.AddListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
        Messenger.AddListener(GameEvent.COIN_INSERTED, OnCoinInsert);
        Messenger.RemoveListener(GameEvent.GAME_RESET, OnReset);
    }

    private void Start()
    {
        _isGameStarted = false;
        _audio.PlayOneShot(_startupClip);
        _coinsLabel.text = _numCoins.ToString("D2");
        _highScoreLabel.text = Managers.Player.highScore.ToString("D6");
    }

    private void OnManagersStarted()
    {
        StartCoroutine(StartGame());
    }

    private void OnCoinInsert()
    {
        _numCoins++;
        if(_coinsLabel == null)
        {
            _coinsLabel = GameObject.Find("Canvas/Credit/NumCredits").GetComponent<Text>();
        }            
        _coinsLabel.text = _numCoins.ToString("D2");
    }

    private IEnumerator StartGame()
    {
        while(!_isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Messenger.Broadcast(GameEvent.COIN_INSERTED);
                _audio.PlayOneShot(_coinInsert);
            }                
            if (Input.GetKeyDown(KeyCode.F1) && _numCoins > 0)
            {
                _audio.PlayOneShot(_gameStart);
                _isGameStarted = true;
                yield return new WaitForSeconds(7);
            }                
            yield return new WaitForFixedUpdate();
        }
        Managers.Mission.GoToNext();        
    }

    private void OnReset()
    {
        StartCoroutine(StartGame());
    }
}
