using UnityEngine;

public enum AnimationType {
    Idle,
    Walking,
    Running,
    AttackMelee,
    AttackArcher,
    AttackZombie,
    Hit,
    Heal,
    Death
}

public class AnimationHelper {
    private readonly Animator _animator;

    public AnimationHelper(Animator animator) {
        _animator = animator;
    }

    private void Reset() {
        foreach (var parameter in _animator.parameters) {
            if (parameter.type == AnimatorControllerParameterType.Bool) {
                _animator.SetBool(parameter.name, false);
            }
            if (parameter.type == AnimatorControllerParameterType.Trigger) {
                _animator.ResetTrigger(parameter.name);
            }
        }
    }

    public void PlayAnimation(AnimationType animationType, bool value = true) {
        if (_animator == null) {
            return;
        }
        Reset();

        switch (animationType) {
            case AnimationType.Idle:
                _animator.SetBool("Idle", true);
                break;
            case AnimationType.Walking:
                _animator.SetBool("Walking", value);
                break;
            case AnimationType.Running:
                _animator.SetBool("Running", value);
                break;
            case AnimationType.AttackMelee:
                _animator.SetTrigger("Attack Melee");
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
            case AnimationType.AttackArcher:
                _animator.SetTrigger("Attack Archer");
                break;
            case AnimationType.AttackZombie:
                _animator.SetTrigger("Attack Zombie");
                break;
        }
    }

    public void SetAttackSpeed(float attackSpeed) {
        _animator.SetFloat("Attack Speed", attackSpeed);
    }
}
