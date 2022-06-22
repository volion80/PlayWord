//Unicode leading character: https://unicode-table.com/en/blocks/block-elements/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterUI : MonoBehaviour
{
	Text _text;
	TMP_Text _tmpProText;
	string writer;

	[SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;
	
	[SerializeField] bool removeEndingLeadChar = false;
	[SerializeField] string newLineChar = "";
	[SerializeField] float initialDelay = 2.0f;
	[SerializeField] float newLineDelay = 0.1f;
	
	

	// Use this for initialization
	void Start()
	{
		_text = GetComponent<Text>()!;
		_tmpProText = GetComponent<TMP_Text>()!;

		if(_text != null)
        {
			writer = _text.text;
			_text.text = "";

			StartCoroutine("TypeWriterText");
		}

		if (_tmpProText != null)
		{
			writer = _tmpProText.text;
			_tmpProText.text = "";

			StartCoroutine("TypeWriterTMP");
		}
	}

	IEnumerator TypeWriterText()
	{
		_text.text = leadingCharBeforeDelay ? leadingChar : "";

		yield return new WaitForSeconds(delayBeforeStart);

		foreach (char c in writer)
		{
			if (_text.text.Length > 0)
			{
				_text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
			}
			_text.text += c;
			_text.text += leadingChar;
			yield return new WaitForSeconds(timeBtwChars);
		}

		if(leadingChar != "")
        {
			_text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
		}
	}

	IEnumerator TypeWriterTMP()
    {
	    yield return new WaitForSeconds(initialDelay);
	    
	    Managers.Audio.PlayDefaultSound();
	    
	    _tmpProText.text = newLineChar + (leadingCharBeforeDelay ? leadingChar : "");

        yield return new WaitForSeconds(delayBeforeStart);

		foreach (char c in writer)
		{
			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
			}
			_tmpProText.text += c;

			bool isNewLine = c.ToString() == "\n"; 
			
			if (isNewLine)
			{
				_tmpProText.text += newLineChar;
			}
			
			_tmpProText.text += leadingChar;

			float timeToWait = !isNewLine ? timeBtwChars : newLineDelay;
			yield return new WaitForSeconds(timeToWait);
		}

		if (leadingChar != "" && removeEndingLeadChar)
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
		}
	}
}