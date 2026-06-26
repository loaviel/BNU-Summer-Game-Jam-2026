using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonHover :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private TMP_Text label;

    [Header("Colors")]
    [SerializeField]
    private Color normalColor =
        new Color32(76, 58, 58, 255);

    [SerializeField]
    private Color hoverColor =
        new Color32(102, 82, 82, 255);

    [Header("Animation")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float speed = 10f;

    [Header("Glow")]
    [SerializeField] private float hoverGlow = 0.18f;
    [SerializeField] private float normalGlow = 0f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        if (label == null)
            label = GetComponent<TMP_Text>();

        originalScale = transform.localScale;
        targetScale = originalScale;

        label.color = normalColor;
        label.fontMaterial.SetFloat("_GlowPower", normalGlow);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            speed * Time.unscaledDeltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        label.color = hoverColor;
        label.fontMaterial.SetFloat("_GlowPower", hoverGlow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;

        label.color = normalColor;
        label.fontMaterial.SetFloat("_GlowPower", normalGlow);
    }
}