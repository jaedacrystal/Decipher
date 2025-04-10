using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayerScenes : MonoBehaviour
{
    public void PlaySingleplayer()
    {
        Animation();
        Invoke("SingleplayerLoadNext", 1f);
    }

    public void PlayMultiplayer()
    {
        Animation();
        Invoke("MultiplayerLoadNext", 1f);
    }

    private void Animation()
    {
        GetComponent<Animator>().Play("FadeOut");
    }

    private void SingleplayerLoadNext()
    {
        SceneManager.LoadScene("Desktop");
    }

    private void MultiplayerLoadNext()
    {
        SceneManager.LoadScene("Multiplayer");
    }
}
