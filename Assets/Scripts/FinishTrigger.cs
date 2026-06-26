using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeTime = 2f;

    private bool finished;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        finished = true;
        StartCoroutine(FinishGame());
    }

    private System.Collections.IEnumerator FinishGame()
    {
        Time.timeScale = 0f;

        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0f;
        fadeCanvas.blocksRaycasts = true;

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(t / fadeTime);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        SceneManager.LoadScene("MainMenu");
    }
}