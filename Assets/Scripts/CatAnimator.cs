using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CatAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    private CatController cat;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        controller = GetComponentInParent<CharacterController>();
        cat = GetComponentInParent<CatController>();
    }

    private void Update()
    {
        UpdateMovement();
        UpdateStates();
    }

    private void UpdateMovement()
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        animator.SetFloat(
            "Speed",
            horizontalVelocity.magnitude
        );
    }

    private void UpdateStates()
    {
        animator.SetBool(
            "Grounded",
            controller.isGrounded
        );

        animator.SetBool(
            "Gliding",
            cat.IsGliding
        );
    }

    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }

    public void PlayClimb()
    {
        animator.SetTrigger("Climb");
    }
}