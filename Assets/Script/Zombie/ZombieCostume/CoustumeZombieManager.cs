using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class CoustumeZombieManager : MonoBehaviour
{
    public List<CoustumeZombie> listCoustumeZombie = new List<CoustumeZombie>();
    public void SetCostumeZombie(Zombie zombie,ZombieCostumeSet zombieCostumeSet)
    {
        CoustumeZombie coustumeZombie = listCoustumeZombie.FirstOrDefault(coustumeZombie => coustumeZombie.idZombieCoustume == zombie.idZombieCoustume);
        zombieCostumeSet.spriteHead.sprite = coustumeZombie.spriteHead;
        zombieCostumeSet.spriteBody.sprite = coustumeZombie.spriteBody;
        zombieCostumeSet.spriteTopArm.sprite = coustumeZombie.spriteTopArm;
        zombieCostumeSet.spriteBackArm.sprite = coustumeZombie.spriteBackArm;
        zombieCostumeSet.spriteTopLeg.sprite = coustumeZombie.spriteTopLeg;
        zombieCostumeSet.spriteBackLeg.sprite = coustumeZombie.spriteBackLeg;
        
    }
}
[Serializable]
public class CoustumeZombie
{   
    public string idZombieCoustume;
    public Sprite spriteHead;
    public Sprite spriteBody;
    public Sprite spriteTopArm;
    public Sprite spriteBackArm;
    public Sprite spriteTopLeg;
    public Sprite spriteBackLeg;
    
}
