﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
	float fall = 0;
	private float fallSpeed;
	public bool allowRotation = true;
	public bool limitRotation = false;
	public int individualScore = 100;

	private float individualScoreTime;

	public AudioClip moveSound;
	public AudioClip rotateSound;
	public AudioClip landSound;

	private float continuousVerticalSpeed = 0.05f;
	private float continuousHorizontalSpeed = 0.1f;

	private float buttonDownWaitMax = 0.2f;
	private float buttonDownWaitTimerHorizontal = 0;
	private float buttonDownWaitTimerVertical = 0;

	private float verticalTimer = 0;
	private float horizontalTimer = 0;

	private bool movedInmediateHorizontal = false;
	private bool movedInmediateVertical = false;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();

		fallSpeed = GameObject.Find("GameScript").GetComponent<Game>().fallSpeed;
	}

	private void Update()
	{
		CheckUserInput();
		UpdateIndividualScore();
	}

	void UpdateIndividualScore()
	{
		if (individualScoreTime < 1)
		{
			individualScoreTime += Time.deltaTime;
		}
		else
		{
			individualScoreTime = 0;

			individualScore = Mathf.Max(individualScore - 10, 0);
		}
	}


	void MoveLeft()
	{
		if (movedInmediateHorizontal)
		{

			if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
			{
				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}
			if (horizontalTimer < continuousHorizontalSpeed)
			{
				horizontalTimer += Time.deltaTime;
				return;
			}
		}
		if (!movedInmediateHorizontal)
			movedInmediateHorizontal = true;

		horizontalTimer = 0;
		transform.position += new Vector3(-1, 0, 0);

		if (CheckIsValidPosition())
		{
			FindObjectOfType<Game>().UpdateGrid(this);
			PlayMoveAudio();

		}
		else
		{
			transform.position += new Vector3(1, 0, 0);
		}
	}
	void MoveRight()
	{
		if (movedInmediateHorizontal)
		{
			if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
			{
				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}
			if (horizontalTimer < continuousHorizontalSpeed)
			{
				horizontalTimer += Time.deltaTime;
				return;
			}
		}
		if (!movedInmediateHorizontal)
			movedInmediateHorizontal = true;

		horizontalTimer = 0;
		transform.position += new Vector3(1, 0, 0);

		if (CheckIsValidPosition())
		{
			FindObjectOfType<Game>().UpdateGrid(this);
			PlayMoveAudio();
		}
		else
		{
			transform.position += new Vector3(-1, 0, 0);
		}
	}
	void MoveDown()
	{
		if (movedInmediateVertical)
		{
			if (buttonDownWaitTimerVertical < buttonDownWaitMax)
			{
				buttonDownWaitTimerVertical += Time.deltaTime;
				return;
			}
			if (verticalTimer < continuousVerticalSpeed)
			{
				verticalTimer += Time.deltaTime;
				return;
			}
		}
		if (!movedInmediateVertical)
			movedInmediateVertical = true;

		verticalTimer = 0;
		transform.position += new Vector3(0, -1, 0);
		if (CheckIsValidPosition())
		{
			FindObjectOfType<Game>().UpdateGrid(this);
			if (Input.GetKey(KeyCode.DownArrow))
			{
				PlayMoveAudio();
			}
		}
		else
		{
			transform.position += new Vector3(0, 1, 0);

			FindObjectOfType<Game>().DeleteRow();

			if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
			{
				FindObjectOfType<Game>().GameOver();
			}

			PlayLandAudio();
			FindObjectOfType<Game>().SpawnNextTetromino();

			Game.currentScore += individualScore;

			enabled = false;

		}
		fall = Time.time;
	}
	void Rotate()
	{
		if (allowRotation)
		{
			if (limitRotation)
			{
				if (transform.rotation.eulerAngles.z >= 90)
				{
					transform.Rotate(0, 0, -90);
				}
				else
				{
					transform.Rotate(0, 0, 90);
				}
			}
			else
			{
				transform.Rotate(0, 0, 90);
			}

			if (CheckIsValidPosition())
			{
				FindObjectOfType<Game>().UpdateGrid(this);
				PlayRotateAudio();
			}
			else
			{
				if (limitRotation)
				{

					if (transform.rotation.eulerAngles.z >= 90)
					{
						transform.Rotate(0, 0, -90);
					}
					else
					{
						transform.Rotate(0, 0, 90);
					}

				}
				else
				{
					transform.Rotate(0, 0, -90);
				}
			}

		}
	}



	void PlayMoveAudio()
	{
		audioSource.PlayOneShot(moveSound);
	}
	void PlayRotateAudio()
	{
		audioSource.PlayOneShot(rotateSound);
	}
	void PlayLandAudio()
	{
		audioSource.PlayOneShot(landSound);
	}
	void CheckUserInput()
	{
		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
		{
			movedInmediateHorizontal = false;
			
			horizontalTimer = 0;			
			buttonDownWaitTimerHorizontal = 0;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			MoveLeft();
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			movedInmediateVertical = false;
			verticalTimer = 0;
			buttonDownWaitTimerVertical = 0;
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			MoveRight();
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Rotate();
		}

		if (Input.GetKey(KeyCode.DownArrow) || (Time.time - fall >= fallSpeed))
		{
			MoveDown();
		}
	}
	bool CheckIsValidPosition()
		{
			foreach (Transform mino in transform)
			{
				Vector2 pos = FindObjectOfType<Game>().Round(mino.position);

				if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
				{
					return false;
				}
				if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
				{
					return false;
				}
			}

			return true;
		}
}

