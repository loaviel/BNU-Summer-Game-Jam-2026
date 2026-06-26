using UnityEngine;

public class RainVFX : MonoBehaviour
{
    [Header("Follow Player")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 10f, 0);

    [Header("Rain Settings")]
    public ParticleSystem rainParticles;
    public float followSpeed = 10f;

    private void Start()
    {
        if (rainParticles != null)
            rainParticles.Play();
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followSpeed * Time.deltaTime
        );
    }

    public void SetRain(bool value)
    {
        if (rainParticles == null) return;

        if (value) rainParticles.Play();
        else rainParticles.Stop();
    }
}