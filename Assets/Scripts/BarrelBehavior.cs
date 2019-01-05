using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBehavior : MonoBehaviour
{
    private SpriteRenderer _sprite;
    [SerializeField]
    private Sprite rollingSprite_1;
    [SerializeField]
    private Sprite rollingSprite_2;
    [SerializeField]
    private Sprite rollingSprite_3;
    [SerializeField]
    private Sprite rollingSprite_4;
    [SerializeField]
    private Sprite fallingSprite_1;
    [SerializeField]
    private Sprite fallingSprite_2;
    private Sprite[] rollingClips;
    private int rollSpriteIndex = 0;
    private int framesSinceRollSpriteSwap = 0;
    private int rollSpriteFrameSwapThreshold = 10;
    private int framesSinceFallSpriteSwap = 0;
    private int fallSpriteFrameSwapThreshold = 20;

    private bool _isRolling = false;
    private bool _isFalling = true;


	// Use this for initialization
	void Start ()
    {
        _sprite = GetComponent<SpriteRenderer>();
        rollingClips = new Sprite[] { rollingSprite_1, rollingSprite_2, rollingSprite_3, rollingSprite_4};

        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //animation code
        if(_isRolling)
        {
            framesSinceRollSpriteSwap++;
            if (framesSinceRollSpriteSwap >= rollSpriteFrameSwapThreshold)
            {
                rollSpriteIndex++;
                if (rollSpriteIndex == 4)
                    rollSpriteIndex = 0;
                framesSinceRollSpriteSwap = 0;
                _sprite.sprite = rollingClips[rollSpriteIndex];
            }
        }
        
        if(_isFalling)
        {
            framesSinceFallSpriteSwap++;
            if (framesSinceFallSpriteSwap >= fallSpriteFrameSwapThreshold)
            {
                if (_sprite.sprite != fallingSprite_1)
                    _sprite.sprite = fallingSprite_1;
                else if (_sprite.sprite == fallingSprite_1)
                    _sprite.sprite = fallingSprite_2;
                framesSinceFallSpriteSwap = 0;
            }
        }    






    }
}
