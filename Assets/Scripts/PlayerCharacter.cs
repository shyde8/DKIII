using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{

    //for now, we just reset the scene when player touches a ReactiveEnemy
    void OnCollisionEnter2D(Collision2D col)
    {
        ReactiveEnemy enemy = col.gameObject.GetComponent<ReactiveEnemy>();
        if (enemy != null)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }


}
