using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfoManager : MonoBehaviour
{
    /*
     * Particles will be diveded by what they do. 
     * Each array holding particles for multiple stages.
     */
    /*
     * 0 - Greece Stage
     * 1 - Canada Stage
     * 2 - Japan Stage
     * 3 - Mexico Stage
     * 4 - Egypt Stage
     */

    [Tooltip("Select the stage using the slider. \n0 - Greece\n1 - Canada\n2 - Japan\n3 - Mexico\n4 - Egypt")]
    [Range(0, 4)] public int stageID = 0;

    [SerializeField] private GameObject[] SlamParticles;

    [SerializeField] private GameObject[] DiveParticles;

    //[SerializeField] private GameObject[] StageVoices;

    public GameObject GetDiveParticle()
    {
        return DiveParticles[stageID];
    }

    public GameObject GetSlamParticles()
    {
        return SlamParticles[stageID];
    }

}
