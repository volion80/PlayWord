using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    private SceneController _controller;
    
    private float _startPosX = 0;
    private float _posY = 0;
    private float _posZ = 0;
    
    private float alphaEmpty = 0.1764f;
    private float alphaNonEmpty = 1.0f;

    public int ID { get; private set; }

    public int WordId { get; private set; }

    public Color colorActive = Color.magenta;
    public Color colorDefault = Color.white;
    public Color colorStart = Color.green;
    public Color colorStartUsed = new Color(138,138,138, 200);
    public Color textColorStart = Color.white;
    public Color textColorStartUsed = new Color(138,138,138, 200);

    private Animator _letterAnimator;
    private Animation _anim;

    void Awake()
    {
        GameObject controller = GameObject.Find("SceneController");
        _controller = controller.GetComponent<SceneController>();

        _letterAnimator = GetComponent<Animator>();
        
    }

    void Start()
    {
        
    }

    public void SetLetter(int lid, int wordId, string letter)
    {
        ID = lid;
        WordId = wordId;
        UpdateText(letter.ToUpper());

        float size = Managers.Challenge.GetLetterSize();
        float sizeCoef = size / Managers.Challenge.defaultLetterSize;
        int fontSize = GetComponentInChildren<Text>().fontSize;
        
        GetComponentInChildren<Text>().fontSize = Convert.ToInt32(Math.Round(fontSize * sizeCoef, 0, MidpointRounding.AwayFromZero));
        
        GetComponent<RectTransform>().sizeDelta = new Vector2(size,size);
        
        float posX = _startPosX + (ID * size);
        transform.position = new Vector3(posX, _posY, _posZ);
        
        Word parentWord = _controller.GetWord(WordId);
        transform.SetParent(parentWord.transform, false);
    }

    public void OnMouseDown()
    {
        _controller.LetterClicked(this);
    }

    public void SetHighlight(bool isActive)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            Color currentColor = image.color;
            Color newColor = isActive ? colorActive : colorDefault;
            newColor.a = currentColor.a;
            image.color = newColor;
        }
    }

    public bool IsEmpty()
    {
        string letterText = GetText();
        return letterText == "";
    }

    public bool IsLast()
    {
        Word startWord = _controller.GetWord(0);
        string wordText = startWord.GetWordText();
        
        return ID == wordText.Length - 1;
    }

    public void Reset()
    {
        UpdateText("");
    }

    public void UpdateText(string text)
    {
        GetComponentInChildren<Text>().text = text;
        
        Image image = GetComponent<Image>();
        if (image != null) {
            Color color = image.color;
            color.a = IsEmpty() ? alphaEmpty : alphaNonEmpty;
            image.color = color;
        }
    }

    public string GetText()
    {
        return GetComponentInChildren<Text>().text;
    }

    public void SetHighlightStart(bool isUsed)
    {
        Image image = GetComponent<Image>();
        if (image != null) {
            image.color = isUsed ? colorStartUsed : colorStart;
        }

        GetComponentInChildren<Text>().color = isUsed ? textColorStartUsed : textColorStart;
    }

    public void Blink(string type)
    {
        Animation anim = GetComponent<Animation>();
        switch (type)
        {
            case "start":
                anim.Play("LetterAnimation");
                break;
            case "active":
                anim.Play("ActiveLetter");
                break;
            default:
                anim.Play("LetterAnimation");
                break;
        }
        
    }

    public bool IsStartUsed()
    {
        return GetComponent<Image>().color == colorStartUsed;
    }
}
