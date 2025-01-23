using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcCoutume : MonoBehaviour
{
    [Header("HeadCoutuneNpc")]
    public SpriteRenderer coutumeHead;
    [Header("BodyCoutuneNpc")]
    public SpriteRenderer coutumeBody;
    public SpriteRenderer coutumeLeftArm;
    public SpriteRenderer coutumeRightArm;
    public SpriteRenderer coutumeShoulderLeft;
    public SpriteRenderer coutumeShoulderRight;
    [Header("FeedCoutuneNpc")]
    public SpriteRenderer coutumeUpperLagLeft;
    public SpriteRenderer coutumeeUpperLagRight;
    public SpriteRenderer coutumeLowerLagLeft;
    public SpriteRenderer coutumeLowerLagRight;

    private void Awake()
    {

    }
    public void SetCostume(HeadCoutume headCoutume, BodyCoutume bodyCoutume, FeedCoutume feedCoutume)
    {
        coutumeHead.sprite = headCoutume.spriteHead;

        coutumeBody.sprite = bodyCoutume.spriteBody;
        coutumeLeftArm.sprite = bodyCoutume.spriteLeftArm;
        coutumeRightArm.sprite = bodyCoutume.spriteRightArm;
        coutumeShoulderLeft.sprite = bodyCoutume.spriteShoulderLeft;
        coutumeShoulderRight.sprite = bodyCoutume.spriteShoulderRight;

        coutumeUpperLagLeft.sprite = feedCoutume.spriteUpperLagLeft;
        coutumeeUpperLagRight.sprite = feedCoutume.spriteUpperLagRight;
        coutumeLowerLagLeft.sprite = feedCoutume.spriteLowerLagLeft;
        coutumeLowerLagRight.sprite = feedCoutume.spriteLowerLagRight;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
