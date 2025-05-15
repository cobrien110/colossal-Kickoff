using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class Car : MonoBehaviour
{
    [SerializeField] GameplayManager GM = null;
    public bool isLeft = false;
    [SerializeField] private float speed = -15.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (isLeft)
        {
            transform.Rotate(0, 180, 0);
            speed = speed * -1;
        }
        StartCoroutine(DelayedDestroy());

        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;

        if (!GM.IsPlayingGet())
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Warrior"))
        {
            WarriorController WC = collision.GetComponent<WarriorController>();
            WC.Die();
            // Steam Achievement Stuff
            if (SteamManager.Initialized)
            {
                Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("HIT_BY_CAR"));
                Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
            }
        } else if (collision.CompareTag("Monster"))
        {
            MonsterController MC = collision.GetComponent<MonsterController>();
            MC.Stun();
        } else if (collision.CompareTag("Mummy"))
        {
            AIMummy AIM = collision.GetComponent<AIMummy>();
            AIM.Die(true);
        }
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
}
