using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Humanoid", menuName = "Necromancer/Humanoid")]
public class Humanoid : ScriptableObject 
{
    public List<Sprite> idle;
    public List<Sprite> walk;
    public List<Sprite> run;
    public List<Sprite> attack;
    public List<Sprite> die;
    public List<Sprite> hit;
    public float walkSpeed;
    public float runSpeed;
    public float attackSpeed;
    public float attackRange;
    public float attackDamage;

    // 1-2 idle
    // 4-7 running
    // 24-27 Melee attack
    // 28-31 Archer attack
    // 37-39 death
    
    public List<Sprite> GetAnimation(string name)
    {
        switch (name)
        {
            case "idle":
                return idle;
            case "walk":
                return walk;
            case "run":
                return run;
            case "attack":
                return attack;
            case "die":
                return die;
            case "hit":
                return hit;
            default:
                return null;
        }
    }    
}
