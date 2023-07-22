using UnityEngine;

public enum AnimationType
{
    Idle,
    Walking,
    Running,
    AttackMelee,
    Hit,
    Heal,
    Death
}

public class AnimationHelper
{
    private readonly Animator _animator;

    public AnimationHelper(Animator animator)
    {
        _animator = animator;
    }

    private void Reset()
    {
        foreach (var parameter in _animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                _animator.ResetTrigger(parameter.name);
            }
            else if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                _animator.SetBool(parameter.name, false);
            }
        }
    }

    public void PlayAnimation(AnimationType animationType, bool value = true)
    {
        if (_animator == null)
        {
            return;
        }
        Reset();

        switch (animationType)
        {
            case AnimationType.Idle:
                _animator.SetBool("Idle", true);
                break;
            case AnimationType.Walking:
                _animator.SetBool("Walking", value);
                _animator.SetBool("Action", true);
                break;
            case AnimationType.Running:
                _animator.SetBool("Running", value);
                _animator.SetBool("Action", true);
                break;
            case AnimationType.AttackMelee:
                _animator.SetTrigger("Attack Melee");
                _animator.SetBool("Action", true);
                break;
            case AnimationType.Hit:
                _animator.SetTrigger("Hit");
                break;
            case AnimationType.Heal:
                _animator.SetTrigger("Heal");
                break;
            case AnimationType.Death:
                _animator.SetBool("Idle", false);
                _animator.SetTrigger("Death");
                break;
        }
    }
}
