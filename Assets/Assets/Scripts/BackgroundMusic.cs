using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;
    public float fadeInTime = 2.0f; // Time for the fade-in effect

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = 0f; // Start with volume at 0
        audioSource.Play();       // Begin playback
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeInTime;
            yield return null;
        }
        audioSource.volume = 1f; // Ensure volume is at full
    }
}