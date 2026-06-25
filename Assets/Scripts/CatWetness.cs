using UnityEngine;

public class CatWetness : MonoBehaviour
{
    [Header("Wetness")]
    public float maxWetness = 100f;
    public float currentWetness;

    [SerializeField] private float wetRate = 15f;
    [SerializeField] private float dryRate = 10f;

    public WetnessState CurrentState { get; private set; }

    private void Update()
    {
        if (CurrentState == WetnessState.Soaked)
        {
            Debug.Log("Cat soaked!");
        }
    }
    public void AddWetness()
    {
        currentWetness +=
            wetRate *
            Time.deltaTime;

        currentWetness =
            Mathf.Clamp(
                currentWetness,
                0,
                maxWetness
            );

        UpdateState();
    }

    public void DryOff()
    {
        currentWetness -=
            dryRate *
            Time.deltaTime;

        currentWetness =
            Mathf.Clamp(
                currentWetness,
                0,
                maxWetness
            );

        UpdateState();
    }

    private void UpdateState()
    {
        float percent =
            currentWetness / maxWetness;

        if (percent < 0.25f)
            CurrentState = WetnessState.Dry;

        else if (percent < 0.5f)
            CurrentState = WetnessState.Damp;

        else if (percent < 0.9f)
            CurrentState = WetnessState.Wet;

        else
            CurrentState = WetnessState.Soaked;
    }

    private void OnGUI()
    {
        string state = "Dry";

        if (currentWetness >= 75)
            state = "SOAKED";
        else if (currentWetness >= 50)
            state = "Wet";
        else if (currentWetness >= 25)
            state = "Damp";

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 24;

        GUI.Box(
            new Rect(20, 70, 400, 40),
            $"Wetness: {Mathf.RoundToInt(currentWetness)} | {state}"
        );
    }
}