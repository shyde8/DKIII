using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController : MonoBehaviour
{
    [SerializeField]
    private Sprite _firstSprite;
    [SerializeField]
    private Sprite _secondSprite;
    private int _framesSinceSpriteSwap = 0;
    public int spriteSwapFrameThreshold = 10;

    private SpriteRenderer _sprite;
    // Use this for initialization
    void Start ()
    {
        _sprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _framesSinceSpriteSwap++;
        if(_framesSinceSpriteSwap >= spriteSwapFrameThreshold)
        {
            if (_sprite.sprite == _firstSprite)
                _sprite.sprite = _secondSprite;
            else
                _sprite.sprite = _firstSprite;
            _framesSinceSpriteSwap = 0;
        }
	}
}
