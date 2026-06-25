using UnityEngine;

public class RainDetector : MonoBehaviour
{
    public bool IsCovered { get; private set; }

    [SerializeField] private float checkHeight = 20f;
    [SerializeField] private LayerMask coverLayer;

    [SerializeField] private float rechargeDelay = 1f;

    private float coverTimer;

    private UmbrellaSystem umbrellaSystem;

    private void Awake()
    {
        umbrellaSystem = GetComponent<UmbrellaSystem>();
    }

    private void Update()
    {
        IsCovered = Physics.Raycast(
            transform.position,
            Vector3.up,
            checkHeight,
            coverLayer
        );

        if (IsCovered)
        {
            coverTimer += Time.deltaTime;

            if (coverTimer >= rechargeDelay)
            {
                umbrellaSystem.Recharge();
            }
        }
        else
        {
            coverTimer = 0f;
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