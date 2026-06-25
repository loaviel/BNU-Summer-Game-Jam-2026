using UnityEngine;

public class UmbrellaSystem : MonoBehaviour
{
    [Header("Umbrella")]
    public float maxCharge = 100f;
    public float currentCharge = 100f;

    [SerializeField] private float glideDrainRate = 15f;
    [SerializeField] private float rainDrainRate = 10f;
    [SerializeField] private float rechargeRate = 20f;

    public bool IsOpen { get; private set; }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(20, 20, 300, 50),
            $"Umbrella: {currentCharge:F0}"
        );
    }
    public void OpenUmbrella()
    {
        if (currentCharge <= 0)
            return;

        IsOpen = true;
    }

    public void CloseUmbrella()
    {
        IsOpen = false;
    }

    public void DrainForGlide()
    {
        currentCharge -=
            glideDrainRate *
            Time.deltaTime;

        CheckEmpty();
    }

    public void DrainForRain()
    {
        currentCharge -=
            rainDrainRate *
            Time.deltaTime;

        CheckEmpty();
    }

    public void Recharge()
    {
        currentCharge +=
            rechargeRate *
            Time.deltaTime;

        currentCharge =
            Mathf.Clamp(
                currentCharge,
                0,
                maxCharge
            );
    }

    private void CheckEmpty()
    {
        currentCharge =
            Mathf.Clamp(
                currentCharge,
                0,
                maxCharge
            );

        if (currentCharge <= 0)
        {
            CloseUmbrella();
        }
    }
}