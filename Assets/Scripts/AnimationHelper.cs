using UnityEngine;
public class AnimationHelper
{
    private Animator _animator;

    public AnimationHelper(Animator animator)
    {
        _animator = animator;
    }

    public void Idle()
    {
        _animator.SetBool("Action", false);
        _animator.SetBool("Walking", false);
        _animator.SetBool("Running", false);
        _animator.SetBool("Idle", true);
    }

    public void Walking(bool isWalking)
    {
        _animator.SetBool("Action", true);
        _animator.SetBool("Walking", isWalking);
    }

    public void Running(bool isRunning)
    {
        _animator.SetBool("Action", true);
        _animator.SetBool("Running", isRunning);
    }

    public void AttackMelee()
    {
        _animator.SetBool("Action", true);
        _animator.SetTrigger("Attack Melee");
    }

    public void Hit()
    {
        _animator.SetTrigger("Hit");
    }

    public void Heal()
    {
        _animator.SetTrigger("Heal");
    }

    public void Death()
    {
        Idle();
        _animator.SetBool("Action", false);
        _animator.SetBool("Idle", false);
        _animator.SetBool("Death", true);
    }
}