using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VolumeManager : MonoBehaviour
{
    // Start is called before the first frame update
    private int selectedOption = -1;
    int musicVolume;
    int effectsVolume;
    [SerializeField] private GameObject[] musicButtonTexts;
    [SerializeField] private GameObject[] musicButtonArrows;

    void Start() {
        musicVolume = PlayerPrefs.GetInt("musicVolume", 100);
        effectsVolume = PlayerPrefs.GetInt("effectsVolume", 100);
        musicButtonTexts[0].GetComponent<TextMeshProUGUI>().text = musicVolume.ToString();
        musicButtonTexts[1].GetComponent<TextMeshProUGUI>().text = effectsVolume.ToString();
    }
    public void select(int index) {
        //0: Music
        //1: SFX
        selectedOption = index;
        musicButtonArrows[index].SetActive(true);
    }

    public void unselect() {
        musicButtonArrows[selectedOption].SetActive(false);
    }

    public void pageLeft() {
        if (selectedOption == 0) {
            if (musicVolume > 0) {
                musicVolume -= 10;
                musicButtonTexts[0].GetComponent<TextMeshProUGUI>().text = musicVolume.ToString();
                PlayerPrefs.SetInt("musicVolume", musicVolume);
            }
        } else if (selectedOption == 1) {
            if (effectsVolume > 0) {
                effectsVolume -= 10;
                musicButtonTexts[1].GetComponent<TextMeshProUGUI>().text = effectsVolume.ToString();
                PlayerPrefs.SetInt("effectsVolume", musicVolume);
            }
        }
    }

    public void pageRight() {
        if (selectedOption == 0) {
            if (musicVolume < 100) {
                musicVolume += 10;
                musicButtonTexts[0].GetComponent<TextMeshProUGUI>().text = musicVolume.ToString();
                PlayerPrefs.SetInt("musicVolume", musicVolume);
            }
        } else if (selectedOption == 1) {
            if (effectsVolume < 100) {
                effectsVolume += 10;
                musicButtonTexts[1].GetComponent<TextMeshProUGUI>().text = effectsVolume.ToString();
                PlayerPrefs.SetInt("effectsVolume", musicVolume);
            }
        }
    }
}
