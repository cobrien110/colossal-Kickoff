using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class MonsterCharSelectOption: MonoBehaviour
{
    [SerializeField] private GameObject[] monsterElements;

    void Awake() {
        changeMonster(0);
        StartCoroutine(AttackCycle());
    }

    public void updateSprite(int index) {
        changeMonster(index);
    }

    private void changeMonster(int index)
    {
        foreach (GameObject monsterDude in monsterElements)
        {
            monsterDude.SetActive(false);
        }
        monsterElements[index].SetActive(true);
    }

    private IEnumerator AttackCycle()
    {
        while (true)
        {
            
            float waitTime = UnityEngine.Random.Range(10f, 15f);
            yield return new WaitForSeconds(waitTime);

            AnimateAttack();
        }
    }

    private void AnimateAttack()
    {
        for (int i = 0; i < monsterElements.Length; i++)
        {
            //Skip index 2 and inactive ones
            if (i == 2 || !monsterElements[i].activeSelf)
                continue;

            Animator anim = monsterElements[i].GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }
    }
}
