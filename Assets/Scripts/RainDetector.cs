using UnityEngine;

public class RainDetector : MonoBehaviour
{
    public bool IsCovered { get; private set; }

    [SerializeField] private float checkHeight = 20f;
    [SerializeField] private LayerMask coverLayer;

    private UmbrellaSystem umbrellaSystem;
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
            umbrellaSystem.Recharge();
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