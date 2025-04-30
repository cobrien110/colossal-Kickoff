using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSetEmissionRate : MonoBehaviour
{
    private ParticleSystem PS;

    [SerializeField] private float minRange = 0f;
    [SerializeField] private float maxRange = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        PS = GetComponent<ParticleSystem>();
        float v = Random.Range(minRange, maxRange);

        var emission = PS.emission; 
        emission.rateOverTime = v;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
