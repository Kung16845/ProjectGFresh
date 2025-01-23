using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
public class FakeHeightObject : MonoBehaviour
{
    public GameObject areaBombDamage;
    public Transform trnsObject;
    public Transform trnsBody;
    public Transform trnsShadow;
    public float gravity = -10;
    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float lastInitialVerticalVelocity;
    public bool isGrounded;
    public float artTime; // Detonation time
    public ShootBomb shootBomb;

    private void Update()
    {
        UpdatePosition();
        CheckGroundHit();
    }

    public void Initialize(Vector2 groundVelocity, float verticalVelocity)
    {
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
        lastInitialVerticalVelocity = verticalVelocity;
    }

    private void UpdatePosition()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            trnsBody.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        }
        trnsObject.position += (Vector3)groundVelocity * Time.deltaTime;
    }

    private void CheckGroundHit()
    {
        if (trnsBody.position.y < trnsObject.position.y && !isGrounded)
        {
            trnsBody.position = trnsObject.position;
            isGrounded = true;
            Stick();
        }
    }

    private void Stick()
    {
        groundVelocity = Vector2.zero;
        StartCoroutine(DetonateAfterDelay(artTime));
    }

    private IEnumerator DetonateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnAreaDamage();
        shootBomb.DestroyBomb(gameObject);
    }

    private void SpawnAreaDamage()
    {
        Debug.Log("Area Bomb Damage Spawned");
        Instantiate(areaBombDamage, transform.position, Quaternion.identity);
    }
}

