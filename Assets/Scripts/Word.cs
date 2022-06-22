using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Word : MonoBehaviour
{
    [SerializeField] private Letter letter;

    public const int MinLetters = 2;
    
    private SceneController _controller;
    
    private float _startPosY = 0;
    private float _posZ = 0;

    public int ID { get; private set; }
    public bool IsActive { get; private set; }


    private void Awake()
    {
        GameObject sceneController = GameObject.Find("SceneController");
        _controller = sceneController.GetComponent<SceneController>();
    }

    public void SetWord(int wordId)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject panel = GameObject.Find("Panel");
        
        ID = wordId;
        bool isStartWord = ID == 0;
        string startWordText = Managers.Challenge.GetStartWordText();

        
        float size = Managers.Challenge.GetLetterSize();
        float width = size * startWordText.Length;
        GetComponent<RectTransform>().sizeDelta = new Vector2(width,size);

        float posX = -(width / 2);

        float posY;
        if (isStartWord)
        {
            float sizeDiff = Managers.Challenge.defaultLetterSize - size;
            posY = _startPosY - 20f - sizeDiff;
        }
        else if (ID == 1)
            posY = _startPosY;
        
        else
            posY = _startPosY - (size * (ID - 1));
        

        transform.position = new Vector3(posX, posY, _posZ);
        

        Transform parentTransform = isStartWord ? canvas.transform : panel.transform;
        transform.SetParent(parentTransform, false);
        if (isStartWord)
        {
            transform.SetAsFirstSibling();
        }

        for (int i = 0; i < startWordText.Length; i++)
        {
            string letterText = isStartWord ? startWordText[i].ToString() : "";
            AddLetter(i, letterText);
        }
    }

    public string GetWordText()
    {
        List<string> letterList = new List<string>();
        List<Letter> letters = GetLetters();
        foreach (Letter l in letters)
        {
            letterList.Add(l.GetText());
        }

        return string.Join("", letterList);
    }
    
    private void AddLetter(int letterId, string text)
    {
        Letter letterObj = Instantiate(letter);
        
        letterObj.SetLetter(letterId, ID, text);
    }

    public bool IsStartWord()
    {
        return ID == 0;
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        
        HighlightActive(active);
    }

    public List<Letter> GetLetters()
    {
        List<Letter> letters = FindObjectsOfType<Letter>().ToList();
        letters = letters.Where(o => o.WordId == ID).OrderBy(o => o.ID).ToList();
        
        return letters;
    }
    
    public bool PopulateLetter(Letter startWordLetter)
    {
        List<Letter> letters = GetLetters();
        Letter l = letters.FirstOrDefault(o => o.GetComponentInChildren<Text>().text == "");

        if (l == null)
            return false;
        
        string letterText = startWordLetter.GetText();
        l.UpdateText(letterText);
        l.Blink("active");
        return true;
    }
    
    private void HighlightActive(bool active)
    {
        List<Letter> letters = GetLetters();
        
        foreach (Letter l in letters)
        {
            l.SetHighlight(active);
        }
    }

    public void InitHighlightStart()
    {
        List<Letter> letters = GetLetters();
        foreach (Letter l in letters)
        {
            l.SetHighlightStart(false);
        }
    }

    public void RefreshHighlightStart()
    {
        List<Letter> usedLetters = new List<Letter>();
        Word activeWord = _controller.GetActiveWord();
        List<Letter> startWordLetters = GetLetters();
        if (activeWord != null)
        {
            foreach (Letter al in activeWord.GetLetters())
            {
                if (al.GetComponentInChildren<Text>().text == "")
                    continue;

                Letter startLetterFound = startWordLetters.FirstOrDefault(o => o.GetText() == al.GetText() && !usedLetters.Contains(o));
                if (startLetterFound != null)
                    usedLetters.Add(startLetterFound);
            }
        }

        foreach (Letter l in startWordLetters)
        {
            bool isUsed = usedLetters.Contains(l);
            l.SetHighlightStart(isUsed);
        }
    }
    
    public bool IsEmptyNextLetter(Letter l)
    {
        Letter nextLetter = GetLetters().First(o => o.ID == l.ID + 1);
        
        return nextLetter.IsEmpty();
    }
    
    public bool IsLetterAvailable(Letter l)
    {
        string letterText = l.GetText();
        int countInStart = GetLetters().Count(o => o.GetComponentInChildren<Text>().text == letterText);
        Word activeWord = _controller.GetActiveWord();
        if (activeWord == null)
            return false;
        
        int countInActive = activeWord.GetLetters().Count(o => o.GetComponentInChildren<Text>().text == letterText);

        return (countInActive + 1) <= countInStart;
    }
}
