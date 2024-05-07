using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBurst : MonoBehaviour
{
    public Sprite[] sprites;
    public float minSpin, maxSpin;
    public float minLaunchForce, maxLaunchForce;
    public float minWeight, maxWeight;
    public float size;
    public LayerMask groundLayer;

    void Start()
    {
        BurstSprites();
    }

    void BurstSprites()
    {
        foreach (Sprite sprite in sprites)
        {
            GameObject spriteObj = new GameObject();
            spriteObj.transform.position = transform.position;
            spriteObj.transform.localScale = new Vector3(size, size, size); // Adjust scale for 3D

            SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();
            Rigidbody rb = spriteObj.AddComponent<Rigidbody>();
            BoxCollider col = spriteObj.AddComponent<BoxCollider>(); // Using SphereCollider for simplicity

            // Apply sprite
            sr.sprite = sprite;

            // Apply random spin
            float spin = Random.Range(minSpin, maxSpin);
            rb.angularVelocity = new Vector3(0, 0, spin); // Use Vector3 for 3D rotation

            // Apply random launch force
            float launchForce = Random.Range(minLaunchForce, maxLaunchForce);
            Vector3 launchDirection = Random.onUnitSphere.normalized; // Use Random.onUnitSphere for 3D launch direction
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse); // Use ForceMode.Impulse for 3D force

            // Apply random weight
            float weight = Random.Range(minWeight, maxWeight);
            rb.mass = weight;

            // Set collision detection
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Use CollisionDetectionMode for 3D

            // Set collision layer
            Physics.IgnoreLayerCollision(gameObject.layer, 6, false); // Use Physics.IgnoreLayerCollision for 3D

            // Set layer to Particle Layer (layer 3)
            spriteObj.layer = 12;

            col.size = new Vector3(.2f, .2f, .2f);

            // Add collision handler
            col.isTrigger = false;

            spriteObj.AddComponent<SpriteBlinker>();
        }
    }
}
