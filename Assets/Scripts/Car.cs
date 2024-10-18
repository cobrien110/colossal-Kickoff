using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] GameplayManager GM = null;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedDestroy());

        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(15.0f, 0.0f, 0.0f) * Time.deltaTime;

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
