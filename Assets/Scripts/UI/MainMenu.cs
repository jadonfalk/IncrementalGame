using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public TitleMusic titleMusic;

    private void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void PlayHover()
    {
        audioSource.PlayOneShot(hoverSound);
    }

    public void PlayGame()
    {
        if (titleMusic != null)
        {
            titleMusic.StopMusic();
        }

        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Incremental"));
    }

    public void Story()
    {
        if (titleMusic != null)
        {
            titleMusic.StopMusic();
        }

        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Story"));
    }

    public void QuitGame()
    {
        PlayClick();
        StartCoroutine(QuitRoutine());
    }

    public void Credits()
    {
        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Credits"));
    }
    
    public void ReturnHome()
    {
        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Incremental"));
    }
    
    public void StatsScene()
    {
        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Stats"));
    }
    
    public void Reroll()
    {
        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Reroll"));
    }

    public void BacktoMain()
    {
        PlayClick();
        StartCoroutine(LoadSceneWithDelay("Title"));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.2f); // small delay so sound plays
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator QuitRoutine()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Application.Quit();
    }

}