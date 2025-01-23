using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCostumeSet : MonoBehaviour
{
    public SpriteRenderer spriteHead;
    public SpriteRenderer spriteBody;
    public SpriteRenderer spriteTopArm;
    public SpriteRenderer spriteBackArm;
    public SpriteRenderer spriteTopLeg;
    public SpriteRenderer spriteBackLeg;
    public Zombie zombie;
    public CoustumeZombieManager coustumeZombieManager;
    private void Awake()
    {
        coustumeZombieManager = FindObjectOfType<CoustumeZombieManager>();
        zombie  = GetComponent<Zombie>();
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);// Wait for the next frame
        coustumeZombieManager.SetCostumeZombie(zombie, this);
    }
}
