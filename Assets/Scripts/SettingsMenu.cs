using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterSlide;
    public Slider musicSlide;
    public Slider fxSlide;

    public Toggle leftyToggle;

    public TMP_Dropdown turnType;
    public Slider snapSlide;
    public TextMeshProUGUI snapAmount;

    public TMP_Dropdown priorityType;

    public AccessibilityModifier accessibility;

    public UIScript uiScript;

    // Start is called before the first frame update
    void Start()
    {
        masterSlide.value = SaveLoad.masterVolume;
        musicSlide.value = SaveLoad.musicVolume;
        fxSlide.value = SaveLoad.sounfFXVolume;

        leftyToggle.isOn = SaveLoad.lefty;

        snapAmount.text = SaveLoad.snapAmount.ToString();
        if (SaveLoad.snapTurn)
        {
            turnType.value = 0;
            snapSlide.gameObject.SetActive(true);
            snapSlide.value = SaveLoad.snapAmount / 15;
        }
        else
        {
            turnType.value = 1;
            snapSlide.gameObject.SetActive(false);
        }

        priorityType.value = SaveLoad.priority - 1;
    }

    float priorityWait = 0;
    bool priorityDisabled = false;
    private void Update()
    {
        if (priorityDisabled && priorityWait > 0)
        {
            priorityWait -= Time.unscaledDeltaTime;

            if (priorityWait < 0)
            {
                priorityDisabled = false;
                priorityType.interactable = true;
            }
        }
    }

    public void SetMaster()
    {
        SaveLoad.masterVolume = masterSlide.value;
        SoundManager.instance.SetMasterVolume(SaveLoad.masterVolume);
    }

    public void SetMusic()
    {
        SaveLoad.musicVolume = musicSlide.value;
        SoundManager.instance.SetMusicVolume(SaveLoad.musicVolume);
    }

    public void SetFx()
    {
        SaveLoad.sounfFXVolume = fxSlide.value;
        SoundManager.instance.SetFXVolume(SaveLoad.sounfFXVolume);
    }

    public void SetLefty()
    {
        SaveLoad.lefty = leftyToggle.isOn;
        accessibility.Set();
    }

    public void SetTurn()
    {
        if (turnType.value == 0)
        {
            SaveLoad.snapTurn = true;
            snapSlide.gameObject.SetActive(true);
        }
        else
        {
            SaveLoad.snapTurn = false;
            snapSlide.gameObject.SetActive(false);
        }
        accessibility.Set();
    }

    public void SetSnapAmount()
    {
        SaveLoad.snapAmount = (int)snapSlide.value * 15;
        snapAmount.text = SaveLoad.snapAmount.ToString();
        accessibility.Set();
    }

    public void SetPriority()
    {
        if (SaveLoad.priority != priorityType.value + 1)
        {
            SaveLoad.priority = priorityType.value + 1;
            priorityType.interactable = false;
            priorityWait = 3f;
            priorityDisabled = true;

            uiScript.ChangeGraphicsQuality();
        }
    }
}
