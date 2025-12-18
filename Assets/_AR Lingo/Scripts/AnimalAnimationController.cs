using UnityEngine;

/// <summary>
/// Controls animal animations (Walk, Attack, Idle)
/// Attach this to each animal 3D model that has animations
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimalAnimationController : MonoBehaviour
{
    [Header("Animation Names")]
    [SerializeField] private string idleAnimationName = "Idle";
    [SerializeField] private string walkAnimationName = "Walk";
    [SerializeField] private string attackAnimationName = "Attack";

    [Header("Settings")]
    [SerializeField] private float animationDuration = 3f; // How long to play Walk/Attack before returning to Idle

    private Animator animator;
    private bool isPlayingAnimation = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[AnimalAnimationController] No Animator found on {gameObject.name}!");
        }
    }

    void OnEnable()
    {
        // Play idle when model is enabled
        PlayIdle();
    }

    /// <summary>
    /// Play Idle animation
    /// </summary>
    public void PlayIdle()
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(idleAnimationName))
        {
            animator.Play(idleAnimationName);
            Debug.Log($"[AnimalAnimationController] Playing Idle animation on {gameObject.name}");
        }

        isPlayingAnimation = false;
    }

    /// <summary>
    /// Play Walk animation, then return to Idle
    /// </summary>
    public void PlayWalk()
    {
        if (animator == null || isPlayingAnimation) return;

        if (!string.IsNullOrEmpty(walkAnimationName))
        {
            animator.Play(walkAnimationName);
            Debug.Log($"[AnimalAnimationController] Playing Walk animation on {gameObject.name}");
            
            isPlayingAnimation = true;
            Invoke(nameof(PlayIdle), animationDuration);
        }
        else
        {
            Debug.LogWarning($"[AnimalAnimationController] Walk animation name not set for {gameObject.name}");
        }
    }

    /// <summary>
    /// Play Attack animation, then return to Idle
    /// </summary>
    public void PlayAttack()
    {
        if (animator == null || isPlayingAnimation) return;

        if (!string.IsNullOrEmpty(attackAnimationName))
        {
            animator.Play(attackAnimationName);
            Debug.Log($"[AnimalAnimationController] Playing Attack animation on {gameObject.name}");
            
            isPlayingAnimation = true;
            Invoke(nameof(PlayIdle), animationDuration);
        }
        else
        {
            Debug.LogWarning($"[AnimalAnimationController] Attack animation name not set for {gameObject.name}");
        }
    }

    /// <summary>
    /// Set custom animation names (called from ARScanFeature when WordData is loaded)
    /// </summary>
    public void SetAnimationNames(string walk, string attack, string idle = "Idle")
    {
        if (!string.IsNullOrEmpty(walk))
            walkAnimationName = walk;
        
        if (!string.IsNullOrEmpty(attack))
            attackAnimationName = attack;
        
        if (!string.IsNullOrEmpty(idle))
            idleAnimationName = idle;
    }

    /// <summary>
    /// Stop current animation and return to idle
    /// </summary>
    public void StopAnimation()
    {
        if (isPlayingAnimation)
        {
            CancelInvoke(nameof(PlayIdle));
            PlayIdle();
        }
    }
}