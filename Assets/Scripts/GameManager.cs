using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public AudioMixer mixer;
    public List<UnityEngine.UI.Button> levels;
    public Slider audioLevel;
    public Dropdown grahicLevel;


    public void SetVolume(float volume)
    {
        mixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("VOLUME", volume);
    }

    public void SetGraphics(int graphicIndex)
    {
        QualitySettings.SetQualityLevel(graphicIndex);
        PlayerPrefs.SetInt("QUALITY", graphicIndex);
    }

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("VOLUME", 0);
        mixer.SetFloat("volume", volume);
        audioLevel.value = volume;

        int graphicIndex = PlayerPrefs.GetInt("QUALITY", 1);
        QualitySettings.SetQualityLevel(graphicIndex);
        grahicLevel.value = graphicIndex;

        int index = PlayerPrefs.GetInt("LEVEL", 1);
        for(int i =1; i < index; i++)
        {
            levels[i].interactable = true;
        }
    }


}
