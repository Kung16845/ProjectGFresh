using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerCharger : MonoBehaviour
{
    private Animator animator;
    public bool Isstunt;
    public bool IsDead;
    public bool Ischarged;
    public bool Isreach;
    public ZombieCharger zombieGrunt;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        zombieGrunt = GetComponent<ZombieCharger>();
        Isstunt = false;
        IsDead = false;
        Ischarged = false;
        Isreach = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieGrunt == null)
        {
            return;
        }
        animator.SetBool("Ischarged",  Ischarged);
        animator.SetBool("IsStunt",  Isstunt);
        animator.SetBool("Isreach",  Isreach);
        if(IsDead)
        {
            animator.SetTrigger("Isdead");
        }
    }
}
