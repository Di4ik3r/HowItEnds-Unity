using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Transition")]
    public Animator transition;
    public float transitionTime;

    public void LoadNewScene(string name)
    {        
        StartCoroutine(LoadScene(name));
    }

    IEnumerator LoadScene(string name)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(name);
    }
}
