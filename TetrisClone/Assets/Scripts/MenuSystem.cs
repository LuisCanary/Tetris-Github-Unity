using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{

	public Text lastScore;

	public void PlayAgain()
	{
		Application.LoadLevel("MainMenu");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	private void Start()
	{
		lastScore.text = PlayerPrefs.GetInt("lastScore").ToString();
	}
}
