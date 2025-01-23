using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator UpperBodyanimator;
    public SpriteRenderer spriteRenderer;
    public bool isgunequip;
    public bool iswalk;
    public bool isrun;
    public bool isfire;
    public bool isreload;
    public int Guntype;


    void Start()
    {
        UpperBodyanimator= GetComponent<Animator>();
        isgunequip = false;
        iswalk = false;
        isrun = false;
        isfire = false;
        isreload = false;
        Guntype = 0;
    }

    void Update()
    {

        // Update gun equip animation state
        UpperBodyanimator.SetBool("Isfire", isfire);
        UpperBodyanimator.SetBool("Gunequip", isgunequip);
        UpperBodyanimator.SetBool("Isreload", isreload);
        UpperBodyanimator.SetInteger("GunType",Guntype);
        // Update movement animation states
        UpperBodyanimator.SetBool("Iswalk", iswalk);
        UpperBodyanimator.SetBool("Isrun", isrun);
    }
    
}
