using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerGrunt : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool IsAttack;
    public bool IsDead;
    public bool IsWalk;
    public bool Isreach;
    public Zombie zombieGrunt;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        zombieGrunt = GetComponent<Zombie>();
        IsAttack = false;
        IsDead = false;
        IsWalk = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieGrunt == null)
        {
            return;
        }
        animator.SetBool("Isattack",  IsAttack);
        animator.SetBool("Isreach",  Isreach);
        if(IsDead)
        {
            animator.SetTrigger("Isdead");
        }
    }
}
