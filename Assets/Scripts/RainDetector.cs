using UnityEngine;

public class RainDetector : MonoBehaviour
{
    public bool IsCovered { get; private set; }

    [SerializeField] private float checkHeight = 20f;
    [SerializeField] private LayerMask coverLayer;

    [SerializeField] private float rechargeDelay = 1f;

    private float coverTimer;

    private UmbrellaSystem umbrellaSystem;
    private CatWetness wetness;
    private RainManager rainManager;

    private void Awake()
    {
        umbrellaSystem =
            GetComponent<UmbrellaSystem>();

        wetness =
            GetComponent<CatWetness>();

        rainManager = FindFirstObjectByType<RainManager>();
    }

    private void Update()
    {
        IsCovered = Physics.Raycast(
            transform.position,
            Vector3.up,
            checkHeight,
            coverLayer
        );

        //Debug.Log($"Covered: {IsCovered} | Umbrella: {umbrellaSystem.IsOpen}");

        if (IsCovered)
        {
            umbrellaSystem.Recharge();
            wetness.DryOff();
        }
        else
        {
            if (umbrellaSystem.currentCharge > 0)
            {
                umbrellaSystem.DrainForRain();
            }
            else
            {
                wetness.AddWetness();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color =
            IsCovered
            ? Color.green
            : Color.red;

        Gizmos.DrawRay(
            transform.position,
            Vector3.up * checkHeight
        );
    }
}