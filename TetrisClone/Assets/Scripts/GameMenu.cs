using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
	public Text levelText;

	public Text highScoreText;
	public Text highScoreText2;
	public Text highScoreText3;


	private void Start()
	{
		levelText.text = "0";

		highScoreText.text = (PlayerPrefs.GetInt("highScore")).ToString();

		highScoreText2.text = (PlayerPrefs.GetInt("highScore2")).ToString();

		highScoreText3.text = (PlayerPrefs.GetInt("highScore3")).ToString();
		
	}

	public void  Play()
	{
		if (Game.startingLevel==0)
		{
			Game.startingAtLevelZero = true;
		}
		else
		{
			Game.startingAtLevelZero = false;
		}

		SceneManager.LoadScene("Level");
	}
	public void ChangedValue (float Value)
	{
		Game.startingLevel = (int)Value;
		levelText.text = Value.ToString();
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
