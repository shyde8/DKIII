using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPassPlatform : MonoBehaviour
{
    private bool _reachedObjective = false;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.name.Contains("Jump"))
        {
            if (collision.gameObject.GetComponent<PlayerMovement>().pubIsGrounded == true)
            {
                if(!_reachedObjective)
                {
                    _reachedObjective = true;
                    Managers.Mission.ReachObjective();
                }
            }                                               
        }
    }

}
