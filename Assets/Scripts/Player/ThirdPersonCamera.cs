using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Camera")]
    public float distance = 5f;
    public float height = 2f;

    [Header("Look")]
    public float sensitivity = 120f;
    public float pitchMin = -20f;
    public float pitchMax = 60f;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        yaw = target.eulerAngles.y;
    }

    private void Update()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(
            pitch,
            pitchMin,
            pitchMax
        );
    }

    private void LateUpdate()
    {
        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 focusPoint =
            target.position +
            Vector3.up * height;

        Vector3 desiredPosition =
            focusPoint +
            rotation * new Vector3(
                0f,
                0f,
                -distance
            );

        transform.position = desiredPosition;

        transform.rotation = rotation;
    }
}