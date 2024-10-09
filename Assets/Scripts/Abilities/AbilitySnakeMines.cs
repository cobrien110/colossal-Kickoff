using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySnakeMines : AbilityScript
{
    private AbilitySnakeSegments ASS;
    public float radius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        ASS = GetComponent<AbilitySnakeSegments>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
    }

    public override void Activate()
    {
        if (timer >= cooldown && ASS.cutSegments.Count >= 1)
        {
            timer = 0;
            for (int i = 0; i < ASS.cutSegments.Count; i++)
            {
                Vector3 center = ASS.cutSegments[i].transform.position;
                //center.y = 0;

                Collider[] objectsInRange = Physics.OverlapSphere(center, radius);
                foreach (Collider obj in objectsInRange)
                {
                    // Check for warriors
                    if (obj.GetComponent<WarriorController>() != null)
                    {
                        // Damage warrior
                        obj.GetComponent<WarriorController>().Die();
                        // Debug.Log("Stunned Warrior: " + obj.name);
                    }
                }
            }
            for (int i = ASS.cutSegments.Count - 1; i >= 0; i--)
            {
                GameObject bombToDestroy = ASS.cutSegments[i];
                ASS.cutSegments.RemoveAt(i);
                Destroy(bombToDestroy);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the object's position with the radius
        if (ASS == null) return;
        if (ASS.cutSegments == null) return;
        for (int i = 0; i < ASS.cutSegments.Count; i++)
        {
            Gizmos.DrawWireSphere(ASS.cutSegments[i].transform.position, radius);
        }      
    }
}
