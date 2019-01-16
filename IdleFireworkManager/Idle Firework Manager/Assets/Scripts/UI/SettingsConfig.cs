using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsConfig : MonoBehaviour
{
    Animator settingsAnim;    //main settings panel animator 

    //music and SFX settings
    bool hasMusic = true;
    public Image musicBtnImg;
    public Sprite musicOn;
    public Sprite musicOff;
    public AudioMixer musicMixer;

    bool hasSFX = true;
    public Image sfxBtnImg;
    public Sprite SFXOn;
    public Sprite SFXOff;
    public AudioMixer SFXMixer;

    //notificaiton settings
    public static bool hasNotification = true;
    public Image notificationBtnImg;
    public Sprite notificationOn;
    public Sprite notificationOff;
    public TextMeshProUGUI notificationTxt;

    // Use this for initialization
    void Start ()
    {
        settingsAnim = GetComponent<Animator>();
        
        //init settings
        if (PlayerPrefs.HasKey("hasMusic"))
        {
            if (PlayerPrefs.GetInt("hasMusic") == 1)
                hasMusic = true;
            else
            {
                musicBtnImg.sprite = musicOff;                
                musicMixer.SetFloat("volume", -80); //0 = normal volume
            }

            if (PlayerPrefs.GetInt("hasSFX") == 1)
                hasSFX = true;
            else
            {
                sfxBtnImg.sprite = SFXOff;                
                SFXMixer.SetFloat("volume", -80); //0 = normal volume
            }

            if (PlayerPrefs.GetInt("hasNotification") == 1)
                hasNotification = true;
            else
                hasNotification = false;
        }
	}             

    public void ToggleMusic()
    {
        hasMusic = !hasMusic;
        if (hasMusic)
        {
            musicBtnImg.sprite = musicOn;
            //set music on
            musicMixer.SetFloat("volume", 20); //0 = normal volume            
        }
        else
        {
            musicBtnImg.sprite = musicOff;
            //set music off
            musicMixer.SetFloat("volume", -80); //0 = normal volume
        }
    }

    public void ToggleSFX()
    {
        hasSFX = !hasSFX;
        if (hasSFX)
        {
            sfxBtnImg.sprite = SFXOn;
            //set SFX on
            SFXMixer.SetFloat("volume", 20); //0 = normal volume
        }
        else
        {
            sfxBtnImg.sprite = SFXOff;
            //set SFX off
            SFXMixer.SetFloat("volume", -80); //0 = normal volume
        }
    }

    public void ToggleNotification()
    {
        hasNotification = !hasNotification;
        if (hasNotification)
        {
            notificationBtnImg.sprite = notificationOn;
            notificationTxt.SetText("Notification");
        }
        else
        {
            notificationBtnImg.sprite = notificationOff;
            notificationTxt.SetText("<s>Notification</s>");
        }
    }

    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }
    private void OnDestroy()
    {
        OnApplicationPause(true);

    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //save settings
            if (hasMusic)
                PlayerPrefs.SetInt("hasMusic", 1);
            else
                PlayerPrefs.SetInt("hasMusic", 0);

            if (hasSFX)
                PlayerPrefs.SetInt("hasSFX", 1);
            else
                PlayerPrefs.SetInt("hasSFX", 0);

            if (hasNotification)
                PlayerPrefs.SetInt("hasNotification", 1);
            else
                PlayerPrefs.SetInt("hasNotification", 0);
        }
    }

    public void openBrowser(string URL)
    {
        Application.OpenURL(URL);
    }
}
