using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaGate : MonoBehaviour
{
    public float duration = 10f;
    private DeleteAfterDelay DAD;
    public float launchSpeed;
    private AbilityGashaPassive AGP;

    // Start is called before the first frame update
    void Start()
    {
        DAD = GetComponent<DeleteAfterDelay>();
        DAD.NewTimer(duration);
        AGP = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilityGashaPassive>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // check for orb collision
        SoulOrb SO = other.GetComponent<SoulOrb>();
        Vector3 dir = new Vector3(1, 0, 0);
        if (SO != null)
        {
            Debug.Log("orb collided with gate");

            // get all warriors and find closest one
            GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
            if (warriors.Length > 0)
            {
                GameObject closest = warriors[0];
                for (int i = 0; i < warriors.Length; i++)
                {
                    if (Vector3.Distance(transform.position, warriors[i].transform.position)
                        < Vector3.Distance(transform.position, closest.transform.position))
                    {
                        closest = warriors[i];
                    }
                }
                dir = (closest.transform.position - SO.transform.position);
                dir.y = 0;
                dir = dir.normalized;
            }

            // launch orb toward closest warrior
            Debug.Log("launching orb from gate");
            SO.Launch(dir * launchSpeed);
            SO.SetTeam(true);

            // add to gasha passive
            Debug.Log("AGP: " + AGP);
            if (AGP != null) AGP.Add(AGP.bonusOnOrb);
        }
    }
}
