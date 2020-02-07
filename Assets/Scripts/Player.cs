//\===========================================================================================
//\ File: Player.cs
//\ Author: Morgan James
//\ Brief: Contains the player functionality including: controls, audio and animations.
//\===========================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]//Make sure the game object has an audio source also attached.
public class Player : MonoBehaviour
{
	[HideInInspector]
	public Node m_nCurrentNode;//The node that the player is currently over.

	[HideInInspector]
	public float m_fTimeRemaining;//How long the player has to complete the maze.

	private Text m_tTimerText;//The UI element that displays the timer left.
	private Text m_tLootText;//The UI element that displays how many successes the player has had.
	private Text m_tDeathsText;//The UI element that displays the amount of deaths the player has endured.

	private AudioSource m_AudioSource;//The audio source that the player sounds will play from.

	[SerializeField]
	private AudioClip m_MoveSound;//The sound the player makes when moving from node to node.
	[SerializeField]
	private AudioClip m_WinSound;//The sound that plays when the player reaches the end node.
	[SerializeField]
	private AudioClip m_SplashSound;//The sound the player makes when moving to a node that has not been set.
	[SerializeField]
	private AudioClip m_DieSound;//The sound the player makes when they ran out of time.
	[SerializeField]
	private AudioClip m_SpawnSound;//The sound that plays when the player is spawned.
	[SerializeField]
	private AudioClip m_LowTime;//The sound that plays when the timer gets low.

	private bool m_bIsEnding = false;//True when the player has lost to stop the occurrence of repetitive functions.
	private bool m_bIsLowTime = false;//True when the time is low to stop the occurrence of repetitive functions.

	private Animator m_Animator;//The animator that controls the players animations.

	void Start()
	{
		m_tTimerText = GameObject.Find("Timer").GetComponent<Text>();//Set the timer text by searching for the game objects name.
		m_tLootText = GameObject.Find("Loot").GetComponent<Text>();//Set the loot text by searching for the game objects name.
		m_tDeathsText = GameObject.Find("Deaths").GetComponent<Text>();//Set the deaths text by searching for the game objects name.

		m_tLootText.text = "Loot: " + PlayerPrefs.GetInt("Loot", 0).ToString();//Set the loot displayed to be equal to the saved loot data.
		m_tDeathsText.text = "Deaths: " + PlayerPrefs.GetInt("Deaths", 0).ToString();//Set the deaths displayed to be equal to the saved deaths data.

		m_Animator = GetComponent<Animator>();//Set the animator.

		m_AudioSource = GetComponent<AudioSource>();//Set the audio source.
		PlayOneShotSound(m_SpawnSound);//Play the spawn sound(ship wreck).
	}

	//Move forwards.
	private void MoveForwards()
	{
		//For every node.
		for (int x = 0; x < PrimsMazeGenerator.instance.m_iGridSizeX; ++x)
		{
			for (int y = 0; y < PrimsMazeGenerator.instance.m_iGridSizeY; ++y)
			{
				//If the node is the current node and the current node isn't the end node.
				if (PrimsMazeGenerator.instance.m_nGrid[x, y] == m_nCurrentNode && !m_nCurrentNode.m_bIsEnd)
				{
					//Make the player face the correct direction.
					transform.rotation = Quaternion.Euler(0, 0, 0);

					//If there is a Node in front move to it.
					if (y + 1 < PrimsMazeGenerator.instance.m_iGridSizeY && PrimsMazeGenerator.instance.m_lSetNodes.Contains(PrimsMazeGenerator.instance.m_nGrid[x, y + 1]))
					{
						//Move to the node.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x, y + 1].m_v3WorldPosition.x, 
							PrimsMazeGenerator.instance.m_nGrid[x, y + 1].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y + 1].m_v3WorldPosition.z);

						//Set the current node to be equal to the node the player moved to.
						m_nCurrentNode = PrimsMazeGenerator.instance.m_nGrid[x, y + 1];

						//If the player has reached the end.
						if (PrimsMazeGenerator.instance.m_nGrid[x, y + 1].m_bIsEnd)
						{
							StartCoroutine(Win());//Signal that the play has beaten the maze.
						}
						//If the new node isn't the end.
						else
						{
							PlayOneShotSound(m_MoveSound);//Play a move sound.
						}
					}
					//If there is no node to move to.
					else
					{
						//Move the player.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.x, 
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.z + 1);

						m_nCurrentNode = null;//Set the current node to be equal to null
					}
					return;
				}
			}
		}
	}

	//Move backwards.
	private void MoveBackwards()
	{
		//For every node.
		for (int x = 0; x < PrimsMazeGenerator.instance.m_iGridSizeX; ++x)
		{
			for (int y = 0; y < PrimsMazeGenerator.instance.m_iGridSizeY; ++y)
			{
				//If the node is the current node and the current node isn't the end node.
				if (PrimsMazeGenerator.instance.m_nGrid[x, y] == m_nCurrentNode && !m_nCurrentNode.m_bIsEnd)
				{
					//Make the player face the correct direction.
					transform.rotation = Quaternion.Euler(0, 180, 0);

					//If there is a Node behind move to it.
					if (y - 1 >= 0 && PrimsMazeGenerator.instance.m_lSetNodes.Contains(PrimsMazeGenerator.instance.m_nGrid[x, y - 1]))
					{
						//Move to the node.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x, y - 1].m_v3WorldPosition.x,
							PrimsMazeGenerator.instance.m_nGrid[x, y - 1].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y - 1].m_v3WorldPosition.z);

						//Set the current node to be equal to the node the player moved to.
						m_nCurrentNode = PrimsMazeGenerator.instance.m_nGrid[x, y - 1];

						//If the player has reached the end.
						if (PrimsMazeGenerator.instance.m_nGrid[x, y - 1].m_bIsEnd)
						{
							StartCoroutine(Win());//Signal that the play has beaten the maze.
						}
						//If the new node isn't the end.
						else
						{
							PlayOneShotSound(m_MoveSound);//Play a move sound.
						}
					}
					else
					{
						//Move the player.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.x,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.z - 1);

						m_nCurrentNode = null;//Set the current node to be equal to null
					}
					return;
				}
			}
		}
	}
	
	//Move right.
	private void MoveRight()
	{
		//For every node.
		for (int x = 0; x < PrimsMazeGenerator.instance.m_iGridSizeX; ++x)
		{
			for (int y = 0; y < PrimsMazeGenerator.instance.m_iGridSizeY; ++y)
			{
				//If the node is the current node and the current node isn't the end node.
				if (PrimsMazeGenerator.instance.m_nGrid[x, y] == m_nCurrentNode && !m_nCurrentNode.m_bIsEnd)
				{
					//Make the player face the correct direction.
					transform.rotation = Quaternion.Euler(0, 90, 0);

					//If there is a Node to the right move to it.
					if (x + 1 < PrimsMazeGenerator.instance.m_iGridSizeX && PrimsMazeGenerator.instance.m_lSetNodes.Contains(PrimsMazeGenerator.instance.m_nGrid[x + 1, y]))
					{
						//Move to the node.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x + 1, y].m_v3WorldPosition.x,
							PrimsMazeGenerator.instance.m_nGrid[x + 1, y].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x + 1, y].m_v3WorldPosition.z);

						//Set the current node to be equal to the node the player moved to.
						m_nCurrentNode = PrimsMazeGenerator.instance.m_nGrid[x + 1, y];

						//If the player has reached the end.
						if (PrimsMazeGenerator.instance.m_nGrid[x + 1, y].m_bIsEnd)
						{
							StartCoroutine(Win());//Signal that the play has beaten the maze.
						}
						//If the new node isn't the end.
						else
						{
							PlayOneShotSound(m_MoveSound);//Play a move sound.
						}
					}
					else
					{
						//Move the player.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.x + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.z);

						m_nCurrentNode = null;//Set the current node to be equal to null
					}
					return;
				}
			}
		}
	}

	//Move left.
	private void MoveLeft()
	{
		//For every node.
		for (int x = 0; x < PrimsMazeGenerator.instance.m_iGridSizeX; ++x)
		{
			for (int y = 0; y < PrimsMazeGenerator.instance.m_iGridSizeY; ++y)
			{
				//If the node is the current node and the current node isn't the end node.
				if (PrimsMazeGenerator.instance.m_nGrid[x, y] == m_nCurrentNode && !m_nCurrentNode.m_bIsEnd)
				{
					//Make the player face the correct direction.
					transform.rotation = Quaternion.Euler(0, 270, 0);

					//If there is a Node to the right move to it.
					if (x - 1 >= 0 && PrimsMazeGenerator.instance.m_lSetNodes.Contains(PrimsMazeGenerator.instance.m_nGrid[x - 1, y]))
					{
						//Move to the node.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x - 1, y].m_v3WorldPosition.x,
							PrimsMazeGenerator.instance.m_nGrid[x - 1, y].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x - 1, y].m_v3WorldPosition.z);

						//Set the current node to be equal to the node the player moved to.
						m_nCurrentNode = PrimsMazeGenerator.instance.m_nGrid[x - 1, y];

						//If the player has reached the end.
						if (PrimsMazeGenerator.instance.m_nGrid[x - 1, y].m_bIsEnd)
						{
							StartCoroutine(Win());//Signal that the play has beaten the maze.
						}
						//If the new node isn't the end.
						else
						{
							PlayOneShotSound(m_MoveSound);//Play a move sound.
						}
					}
					else
					{
						//Move the player.
						transform.position = new Vector3(
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.x - 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.y + 1,
							PrimsMazeGenerator.instance.m_nGrid[x, y].m_v3WorldPosition.z);

						m_nCurrentNode = null;//Set the current node to be equal to null
					}
					return;
				}
			}
		}
	}

	private void Update()
	{
		//If the game is not paused.
		if (Time.timeScale != 0)
		{
			if (m_bIsEnding == false)//If the game is not ending.
			{
				m_fTimeRemaining -= Time.deltaTime;//Decrease the time remaining.

				m_tTimerText.text = "Time: " + m_fTimeRemaining.ToString("F0");//Set the new time.

				//If the player pressed the up arrow/w/swipes up.
				if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || SwipeManager.instance.IsSwiping(SwipeDirection.Up))
					MoveForwards();//Move the player forwards.

				//If the player pressed the down arrow/s/swipes down.
				if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || SwipeManager.instance.IsSwiping(SwipeDirection.Down))
					MoveBackwards();//Move the player backwards.

				//If the player pressed the right arrow/d/swipes right.
				if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || SwipeManager.instance.IsSwiping(SwipeDirection.Right))
					MoveRight();//Move the player right.

				//If the player pressed the left arrow/a/swipes left.
				if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || SwipeManager.instance.IsSwiping(SwipeDirection.Left))
					MoveLeft();//Move the player left.
			}

			//If the player is not on a set node and the game is not ending.
			if (m_nCurrentNode == null && m_bIsEnding == false)
			{
				StartCoroutine(LoseWater());//Start ending the game by way of drowning.
				m_bIsEnding = true;//Declare that the game is ending.
			}

			//If the timer runs out and the game is not ending.
			if (m_fTimeRemaining <= 0 && m_bIsEnding == false)
			{
				StartCoroutine(LoseTime());//Start ending the game by way of starvation.
				m_bIsEnding = true;//Declare that the game is ending.
			}

			//If the time remaining is less than or equal to 9 and the low time boolean hasn't been set to true.
			if (m_fTimeRemaining <= 9 && m_bIsLowTime == false)
			{
				m_AudioSource.PlayOneShot(m_LowTime, 1.0F);//Play the low time sound.
				m_bIsLowTime = true;//Declare that the time is low.
			}
		}
	}

	//Plays the audio clip passed in.
	private void PlayOneShotSound(AudioClip a_AudioClip)
	{
		m_AudioSource.PlayOneShot(a_AudioClip, 1.0F);//Plays the audio clip that was passed in.
	}

	//The lose state from falling off the crates.
	IEnumerator LoseWater()
	{
		PlayerPrefs.SetInt("Deaths", PlayerPrefs.GetInt("Deaths", 0) + 1);//Increase the amount of deaths.
		PlayOneShotSound(m_SplashSound);//Play a splash sound.
		transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);//Move the player into the water.
		m_Animator.SetInteger("State", 1);//Set the animation for the player to be that of drowning.
		GetComponentInChildren<ParticleSystem>().Play();//Play the splashing particle effect.
		yield return new WaitForSeconds(1);//Wait for a second.
		PrimsMazeGenerator.instance.Generate();//Restart the maze.
	}

	//The lose state for running out of time.
	IEnumerator LoseTime()
	{
		PlayerPrefs.SetInt("Deaths", PlayerPrefs.GetInt("Deaths", 0) + 1);//Increase the amount of deaths.
		PlayOneShotSound(m_DieSound);//Play a death sound.
		m_Animator.SetInteger("State", 3);//Set the animation for the player to be falling over.
		yield return new WaitForSeconds(1);//Wait for a second.
		PrimsMazeGenerator.instance.Generate();//Restart the maze.
	}

	//The win state for reaching the island within the time limit.
	IEnumerator Win()
	{
		PlayerPrefs.SetInt("Loot", PlayerPrefs.GetInt("Loot", 0) + 1);//Increase the loot.
		PlayOneShotSound(m_WinSound);//Play the win sound.
		m_Animator.SetInteger("State", 2);//Set the animation for the player to be that of cheering.
		yield return new WaitForSeconds(1);//Wait for a second.
		PrimsMazeGenerator.instance.Generate();//Restart the maze.
	}
}
