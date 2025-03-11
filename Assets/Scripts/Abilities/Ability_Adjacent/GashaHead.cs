using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaHead : MonoBehaviour
{
    public Transform myMonster;
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private float xDif;

    private void Start()
    {
        startingPos = transform.position;
        myMonster = GameObject.FindGameObjectWithTag("Monster").transform;
        xDif = myMonster.position.x - startingPos.x;
    }

    void LateUpdate()
    {
        Vector3 newPos = startingPos;
        newPos.z = myMonster.position.z;
        newPos.x = myMonster.position.x - (xDif * 3.5f);
        if (newPos.x < startingPos.x) newPos.x = startingPos.x;
        transform.position = newPos;
    }
}
