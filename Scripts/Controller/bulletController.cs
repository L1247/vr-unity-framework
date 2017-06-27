using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    private Rigidbody rb;
    // Use this for initialization
    public BloodEffectID bloodEffectID { get; set; }
    public DontGoThroughThings script;
    public TrailRenderer trail;
    public BoxCollider boxcollider;

    private Vector3 initposi;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initposi = transform.parent.position;
        bulletInit();
    }

    public void bulletInit()
    {
        boxcollider.enabled = true;
        trail.enabled = true;
        script.enabled = true;
    }

    void bulletDestroy()
    {
        rb.velocity = new Vector3(0, 0, 0);
        transform.position = initposi;
        script.Init();
    }
    public void DestroyBullet()
    {
        StartCoroutine(destroy());
    }

    public void StopCoroutine()
    {
        boxcollider.enabled = false;
        script.enabled = false;
        trail.enabled = false;
    }

    void Destroy()
    {
        bulletDestroy();
        SmartPool.Despawn(gameObject);
    }

    IEnumerator destroy()
    {
        yield return new WaitForSeconds(1f);
        StopCoroutine();
        yield return new WaitForSeconds(trail.time);
        Destroy();
    }
}

public enum BloodEffectID
{
    blood1,
    blood2,
    blood3
}