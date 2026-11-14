using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;

public class StartGame : MonoBehaviour
{
    [Header("UI Toolkit")]
    private Button startButton;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioSource clickSound;

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // seconds

    void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        startButton.clicked += OnStartButtonClick;
    }

    void Start()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    void OnStartButtonClick()
    {
        StartCoroutine(ClickFadeAndLoad());
    }

    IEnumerator ClickFadeAndLoad()
    {
        // Play click sound
        if (clickSound != null)
        {
            clickSound.Stop();
            clickSound.Play();
        }

        // Fade out background music
        if (backgroundMusic != null)
        {
            yield return StartCoroutine(FadeOutMusic(backgroundMusic, fadeDuration));
        }

        SceneManager.LoadScene("Registration");
    }

    IEnumerator FadeOutMusic(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset for next time
    }
}
