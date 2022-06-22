using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPopup : MonoBehaviour
{
    [SerializeField] private GameObject startWordInput;
    [SerializeField] private GameObject timerInput;
    
    [SerializeField] private AudioClip playSoundInvalid;
    [SerializeField] private AudioClip playSoundValid;
    [SerializeField] private AudioClip defaultSound;

    private void OnGUI()
    {
        string startWordText = startWordInput.GetComponentInChildren<InputField>().text.ToUpper();
        startWordInput.GetComponentInChildren<InputField>().text = Regex.Replace(startWordText, @"[^\p{L}]", "");
    }

    public void Open()
    {
        ResetInputs();
        gameObject.SetActive(true);
        Messenger.Broadcast(GameEvent.CHALLENGE_SETUP);
    }

    public void Close()
    {
        Managers.Audio.PlaySound(defaultSound);
        Hide();
        Messenger.Broadcast(GameEvent.CHALLENGE_INIT);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnGoClicked()
    {
        bool valid = true;
        string timerText = timerInput.GetComponentInChildren<InputField>().text;
        if (string.IsNullOrEmpty(timerText))
        {
            timerText = "0";
        }
        string startWordText = startWordInput.GetComponentInChildren<InputField>().text.Replace(" ", "");
        if (startWordText.Length < 2)
        {
            valid = false;
        }

        if (valid)
        {
            Managers.Audio.PlaySound(playSoundValid);
            Managers.Challenge.SetupNew(startWordText, timerText);    
        }
        else
        {
            Managers.Audio.PlaySound(playSoundInvalid);   
        }
    }

    private void ResetInputs()
    {
        startWordInput.GetComponentInChildren<InputField>().text = "";
        timerInput.GetComponentInChildren<InputField>().text = "0";
    }

    public bool IsTextInputInFocus()
    {
        return startWordInput.GetComponentInChildren<InputField>().isFocused;
    }
}
