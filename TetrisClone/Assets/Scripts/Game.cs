using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public static int gridWidth = 10;
	public static int gridHeight = 20;

	public static Transform[,] grid = new Transform[gridWidth, gridHeight];

	public static bool startingAtLevelZero;
	public static int startingLevel;

	public int scoreOneLine = 40;
	public int scoreTwoLine = 100;
	public int scoreThreeLine = 300;
	public int scoreFourLine = 1200;

	public static float fallSpeed = 1.0f;

	public int currentLevel = 0;
	private int numLinesCleared = 0;

	public Text scoreText;
	public Text levelText;
	public Text linesText;

	public AudioClip clearedLineSound;
	private AudioSource audioSource;

	private int numberOfRowsThisTurn = 0;
    public static int currentScore=0;

	private int startingHighScore;
	private int startingHighScore2;
	private int startingHighScore3;

	private GameObject previewTetromino;
	private GameObject nextTetromino;
	private GameObject savedTetromino;
	private GameObject ghostTetromino;

	private bool gameStarted = false;

	public static bool isPaused = false;

	private Vector2 previewTetrominoPosition = new Vector2(-6.5f,16);
	private Vector2 savedTetrominoPosition = new Vector2(-6.5f, 10);

	public Canvas hud_Canvas;
	public Canvas pause_Canvas;

	public int maxSwaps = 2;
	private int currentSwaps = 0;

	void Start()
    {
		currentScore = 0;

		scoreText.text = "0";
		
		currentLevel = startingLevel;

		levelText.text = currentLevel.ToString();

		linesText.text = "0";

		SpawnNextTetromino();

		audioSource = GetComponent<AudioSource>();
		startingHighScore = PlayerPrefs.GetInt("highScore");
		startingHighScore2 = PlayerPrefs.GetInt("highScore2");
		startingHighScore3 = PlayerPrefs.GetInt("highScore3");


	}

	void Update()
	{
		UpdateScore();
		UpdateUI();
		UpdateLevel();
		UpdateSpeed();

		CheckUserInput();
	}

	void CheckUserInput()
	{
		if (Input.GetKeyUp(KeyCode.P))
		{
			if (Time.timeScale==1)
			{
				PauseGame();
				
			}
			else
			{
				ResumeGame();
				
			}
		}
		if (Input.GetKeyUp(KeyCode.LeftShift ) || (Input.GetKeyUp(KeyCode.RightShift)))
		{
			GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("CurrentActiveTetromino");
			SaveTetromino(tempNextTetromino.transform);
		}
	}
	void PauseGame()
	{
		Time.timeScale = 0;
		isPaused = true;
		audioSource.Pause();
		hud_Canvas.enabled = false;
		pause_Canvas.enabled = true;
	}
	void ResumeGame()
	{
		Time.timeScale = 1;
		isPaused = false;
		audioSource.Play();
		hud_Canvas.enabled = true;
		pause_Canvas.enabled = false;
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void UpdateHighScore()
	{
		if (currentScore > startingHighScore)
		{
			PlayerPrefs.SetInt("highScore3",startingHighScore2);
			PlayerPrefs.SetInt("highScore2", startingHighScore);
			PlayerPrefs.SetInt("highScore", currentScore);
		}
		else if(currentScore > startingHighScore2)
		{
			PlayerPrefs.SetInt("highScore3", startingHighScore2);
			PlayerPrefs.SetInt("highScore2", currentScore);
		}
		else if (currentScore > startingHighScore3)
		{
			PlayerPrefs.SetInt("highScore3", currentScore);
		}

		PlayerPrefs.SetInt("lastScore", currentScore);
	}


	public void SpawnGhostTetromino()
	{
		if (GameObject.FindGameObjectsWithTag("CurrentGhostTetromino")!=null)
		{
			Destroy(GameObject.FindGameObjectWithTag("CurrentGhostTetromino"));
		}

		ghostTetromino = (GameObject)Instantiate(nextTetromino, nextTetromino.transform.position,Quaternion.identity);

		Destroy(ghostTetromino.GetComponent<Tetromino>());
		ghostTetromino.AddComponent<GhostTetromino>();
	}

	void UpdateLevel()
	{
		if ((startingAtLevelZero==true) || startingAtLevelZero==false && (numLinesCleared /10 > startingLevel))
			currentLevel = numLinesCleared / 10;
	}
	void UpdateSpeed()
	{
		fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
	}

	public void UpdateUI()
	{
		scoreText.text = currentScore.ToString();
		levelText.text = currentLevel.ToString();
		linesText.text = numLinesCleared.ToString();
	}

	public void PlayLineClearedSound()
	{
		audioSource.PlayOneShot(clearedLineSound);
	}

	public void UpdateScore()
	{
		if (numberOfRowsThisTurn>0)
		{
			if (numberOfRowsThisTurn==1)
			{
				ClearedOneLine();
			}
			else if (numberOfRowsThisTurn==2)
			{
				ClearedTwoLines();
			}
			else if (numberOfRowsThisTurn==3)
			{
				ClearedThreeLines();
			}
			else if (numberOfRowsThisTurn==4)
			{
				ClearedFourLines();
			}

			numberOfRowsThisTurn = 0;

			PlayLineClearedSound();
		}
	}

	public void ClearedOneLine()
	{
		currentScore += scoreOneLine + (currentLevel * 20);
		numLinesCleared++;
	}
	public void ClearedTwoLines()
	{
		currentScore += scoreTwoLine + (currentLevel * 25);
		numLinesCleared += 2;
	}

	public void ClearedThreeLines()
	{
		currentScore += scoreThreeLine + (currentLevel * 30);
		numLinesCleared += 3;
	}

	public void ClearedFourLines()
	{
		currentScore += scoreFourLine + (currentLevel * 35);
		numLinesCleared += 4;
	}

	bool CheckIsValidPosition(GameObject tetromino)
	{
		foreach (Transform mino in tetromino.transform)
		{
			Vector2 pos = Round(mino.position);

			if (!CheckIsInsideGrid(pos))
			{
				return false;
			}
			if (GetTransformAtGridPosition(pos)!=null && GetTransformAtGridPosition(pos).parent!= tetromino.transform)
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckIsAboveGrid(Tetromino tetromino)
	{
		for (int x = 0; x < gridWidth; x++)
		{
			foreach (Transform mino in tetromino.transform)
			{
				Vector2 pos = Round(mino.position);

				if (pos.y> gridHeight-1)
				{
					return true;
				}
			}
		}

		return false;
	}

	public bool IsFullRowAt(int y)
	{
		for (int x = 0; x < gridWidth; x++)
		{
			if (grid[x,y]==null)
			{
				return false;
			}
		}

		numberOfRowsThisTurn++;

		return true;
	}

	public void DeleteMinoAt(int y)
	{
		for (int x = 0; x < gridWidth; ++x)
		{
			Destroy(grid[x, y].gameObject);

			grid[x, y] = null;
		}
	}

	public void MoveRowDown(int y)
	{
		for (int x = 0; x < gridWidth; ++x)
		{
			if (grid[x,y]!=null)
			{
				grid[x, y - 1] = grid[x, y];

				grid[x, y] = null;

				grid[x, y - 1].position += new Vector3(0, -1, 0);
			}
		}
	}

	public void MoveAllRowsDown(int y)
	{
		for (int i = y; i < gridHeight; ++i)
		{
			MoveRowDown(i);
		}
	}

	public void DeleteRow()
	{
		for (int y = 0; y < gridHeight; ++y)
		{
			if (IsFullRowAt(y))
			{
				DeleteMinoAt(y);

				MoveAllRowsDown(y + 1);

				--y;
			}
		}
	}

	public void UpdateGrid(Tetromino tetromino)
	{
		for (int y = 0; y < gridHeight; y++)
		{
			for (int x = 0; x < gridWidth; x++)
			{
				if (grid[x,y] !=null)
				{
					if (grid[x,y].parent==tetromino.transform)
					{
						grid[x, y] = null;
					}
				}
			}
		}
		foreach (Transform mino in tetromino.transform)
		{
			Vector2 pos = Round(mino.position);
			if (pos.y<gridHeight)
			{
				grid[(int)pos.x, (int)pos.y] = mino;
			}
		}
	}

	public Transform GetTransformAtGridPosition(Vector2 pos)
	{
		if (pos.y>gridHeight-1)
		{
			return null;
		}
		else
		{
			return grid[(int)pos.x,(int)pos.y];
		}
	}

	public void SpawnNextTetromino()
	{
		if (!gameStarted)
		{
			gameStarted = true;

			nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5f, 20f), Quaternion.identity);
			previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino>().enabled = false;
			nextTetromino.tag = "CurrentActiveTetromino";

			SpawnGhostTetromino();
		}
		else
		{
			previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
			nextTetromino = previewTetromino;
			nextTetromino.GetComponent<Tetromino>().enabled = true;
			nextTetromino.tag = "CurrentActiveTetromino";
			previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino>().enabled = false;

			SpawnGhostTetromino();
		}

		currentSwaps = 0;
	}

	public void SaveTetromino(Transform t)
	{
		currentSwaps++;
		if (currentSwaps>maxSwaps)
		{
			return;
		}

		if (savedTetromino!=null)
		{
			GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("CurrentSavedTetromino");
			tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

			if (!CheckIsValidPosition(tempSavedTetromino))
			{
				tempSavedTetromino.transform.localPosition = savedTetrominoPosition;

				return;
			}

			savedTetromino = (GameObject)Instantiate(t.gameObject);
			savedTetromino.GetComponent<Tetromino>().enabled = false;
			savedTetromino.transform.localPosition = savedTetrominoPosition;
			savedTetromino.tag = "CurrentSavedTetromino";

			nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
			nextTetromino.GetComponent<Tetromino>().enabled = true;
			nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
			nextTetromino.tag = "CurrentActiveTetromino";

			Destroy(t.gameObject);
			Destroy(tempSavedTetromino);

			SpawnGhostTetromino();
		}
		else
		{
			savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("CurrentActiveTetromino"));
			savedTetromino.GetComponent<Tetromino>().enabled = false;
			savedTetromino.transform.localPosition = savedTetrominoPosition;
			savedTetromino.tag = "CurrentSavedTetromino";

			Destroy(GameObject.FindGameObjectWithTag("CurrentActiveTetromino"));

			SpawnNextTetromino();
		}
	}

	public bool CheckIsInsideGrid(Vector2 pos)
	{
		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
	}

	public Vector2 Round(Vector2 pos)
	{
		return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
	}

	public string GetRandomTetromino()
	{
		int randomTetromino = Random.Range(1, 8);

		string randomTetrominoName = "Prefabs/Tetromino";

		switch (randomTetromino)
		{
			case 1:
				randomTetrominoName = "Prefabs/Tetromino_T";
				break;

			case 2:
				randomTetrominoName = "Prefabs/Tetromino_Long";
				break;
			case 3:
				randomTetrominoName = "Prefabs/Tetromino_Square";
				break;
			case 4:
				randomTetrominoName = "Prefabs/Tetromino_J";
				break;
			case 5:
				randomTetrominoName = "Prefabs/Tetromino_L";
				break;
			case 6:
				randomTetrominoName = "Prefabs/Tetromino_S";
				break;
			case 7:
				randomTetrominoName = "Prefabs/Tetromino_Z";
				break;
		}
		return randomTetrominoName;
	}

	public void GameOver()
	{
		UpdateHighScore();
		Application.LoadLevel("GameOver");
	}

}
