using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterludeAdvancer : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(PauseAndAdvance());
	}

    private IEnumerator PauseAndAdvance()
    {
        yield return new WaitForSeconds(3);
        Managers.Mission.GoToNext();
    }

}
