using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(LoadSceneWithDelay());
    }

    IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
