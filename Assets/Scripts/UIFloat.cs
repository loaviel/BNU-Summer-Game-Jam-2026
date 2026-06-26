using UnityEngine;

public class UIFloat : MonoBehaviour
{
    public float amplitude = 6f;
    public float speed = 2f;

    Vector2 startPos;

    void Awake()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        Vector2 pos = startPos;

        pos.y += Mathf.Sin(Time.unscaledTime * speed) * amplitude;

        transform.localPosition = pos;
    }
}