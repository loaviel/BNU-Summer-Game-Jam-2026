using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeTime = 2f;

    private bool finished;

    private void OnTriggerEnter(Collider other)
    {
        if (finished)
            return;

        if (!other.CompareTag("Player"))
            return;

        finished = true;

        StartCoroutine(FinishGame());
    }

    private System.Collections.IEnumerator FinishGame()
    {
        Time.timeScale = 0f;

        // TODO:
        // Play meow
        // Play curl-up animation
        // Fade screen

        yield return null;

        SceneManager.LoadScene("MainMenu");
    }
}