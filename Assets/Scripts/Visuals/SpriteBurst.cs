using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBurst : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite[] sprites;
    public float size = 1f;
    public Material spriteMaterial;
    public Color spriteColor = Color.white;

    [Header("Physics")]
    public float minSpin = -10f, maxSpin = 10f;
    public float minLaunchForce = 2f, maxLaunchForce = 5f;
    public float minWeight = 0.5f, maxWeight = 2f;
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioClip bounceSound;

    private AudioPlayer AP;

    void Start()
    {
        BurstSprites();
        AP = GetComponent<AudioPlayer>();
        if (AP != null)
            AP.PlaySoundRandomPitch(AP.Find("corpseExplosion"));
    }

    void BurstSprites()
    {
        foreach (Sprite sprite in sprites)
        {
            GameObject spriteObj = new GameObject("BurstSprite");
            spriteObj.transform.position = transform.position;
            spriteObj.transform.localScale = Vector3.one * size;

            //SpriteRenderer setup
            SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            //Instance and apply material
            if (spriteMaterial != null)
            {
                Material instancedMaterial = Instantiate(spriteMaterial);
                instancedMaterial.color = spriteColor;
                sr.material = instancedMaterial;
            }

            //Rigidbody setup
            Rigidbody rb = spriteObj.AddComponent<Rigidbody>();
            rb.mass = Random.Range(minWeight, maxWeight);
            rb.angularVelocity = new Vector3(0, 0, Random.Range(minSpin, maxSpin));
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.AddForce(Random.onUnitSphere.normalized * Random.Range(minLaunchForce, maxLaunchForce), ForceMode.Impulse);

            //Collider setup
            BoxCollider col = spriteObj.AddComponent<BoxCollider>();
            col.size = Vector3.one * 0.2f;
            col.isTrigger = false;

            //Layer setup
            spriteObj.layer = 12;

            //Additional scripts
            spriteObj.AddComponent<SpriteBlinker>();

            //Audio setup
            if (bounceSound != null)
            {
                AudioSource AS = spriteObj.AddComponent<AudioSource>();
                AS.clip = bounceSound;
                AS.volume = 0.25f;
                AS.spatialBlend = 0.5f;
                AS.rolloffMode = AudioRolloffMode.Linear;
                AS.maxDistance = 100;
                AS.minDistance = 0;
                AS.playOnAwake = false;

                AudioPlayerUncalled APU = spriteObj.AddComponent<AudioPlayerUncalled>();
                GoreBounceSound GBS = spriteObj.AddComponent<GoreBounceSound>();
                GBS.AS = AS;
            }
        }
    }
}