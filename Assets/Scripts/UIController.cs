﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private int _score = 0;
    [SerializeField]
    private Text scoreLabel;

    private int _highScore = 7650;
    [SerializeField]
    private Text highScoreLabel;

    private int _timerCountdown = 7000;
    [SerializeField]
    private Text timerCountdownLabel;

    private int _level = 1;
    [SerializeField]
    private Text levelLabel;

    private int _pointsOnEnemyJump = 100;
    private int _framesBetweenClockDecrement = 0;
    private int _framesBetweenClockDecrementThreshold = 120; //game runs at approximately 60fps, so decrement clock every ~2 seconds

    private void Awake()
    {
        Messenger.AddListener(GameEvent.ENEMY_JUMPED, OnEnemyJump);
        Messenger.AddListener(GameEvent.LEVEL_INCREASED, OnLevelIncrease);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_JUMPED, OnEnemyJump);
        Messenger.RemoveListener(GameEvent.LEVEL_INCREASED, OnLevelIncrease);
    }

    // Use this for initialization
    void Start ()
    {
        scoreLabel.text = _score.ToString("D6");
        highScoreLabel.text = _highScore.ToString("D6");
        timerCountdownLabel.text = _timerCountdown.ToString();
        levelLabel.text = _level.ToString("D2");
    }
	
	// Update is called once per frame
	void Update ()
    {
        //adjust high score if needed
        if (_score > _highScore)
        {
            _highScore = _score;
            highScoreLabel.text = _highScore.ToString("D6");
        }

        //adjust countdown if needed
        _framesBetweenClockDecrement++;
        if(_framesBetweenClockDecrement >= _framesBetweenClockDecrementThreshold)
        {
            _framesBetweenClockDecrement = 0;
            _timerCountdown -= 100;
            timerCountdownLabel.text = _timerCountdown.ToString();
        }
    }

    void OnEnemyJump()
    {
        _score += _pointsOnEnemyJump;
        scoreLabel.text = _score.ToString("D6");
    }

    void OnLevelIncrease()
    {
        _level++;
        levelLabel.text = _level.ToString("D2");
    }
}
