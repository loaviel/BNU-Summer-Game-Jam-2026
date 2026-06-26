using UnityEngine;

public class RainManager : MonoBehaviour
{
    public bool IsRaining = true;

    public RainVFX rainVFX;

    private void Start()
    {
        rainVFX.SetRain(IsRaining);
    }

    private void Update()
    {
        rainVFX.SetRain(IsRaining);
    }
}