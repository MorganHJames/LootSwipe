//\===========================================================================================
//\ File: UIManager.cs
//\ Author: Morgan James
//\ Brief: Controls the UI transitions and some button functionality.
//\===========================================================================================

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_gObjectsToHideOnStart;//The game objects that will be hidden when the start button is pressed.
	[SerializeField]
	private GameObject[] m_gObjectsToUnHideOnStart;//The game objects that will be revealed when the start button is pressed.

	[SerializeField]
	private GameObject[] m_gObjectsToHideOnMenu;//The game objects that will be hidden when the menu/pause button is pressed.
	[SerializeField]
	private GameObject[] m_gObjectsToUnHideOnMenu;//The game objects that will be revealed when the menu/pause button is pressed.

	private Text m_tLootText;
	private Text m_tDeathsText;

	void Start()
	{
		m_tLootText = GameObject.Find("Menu Loot").GetComponent<Text>();//Set the loot text by searching for it's name.
		m_tDeathsText = GameObject.Find("Menu Deaths").GetComponent<Text>();//Set the death text by searching for it's name.

		m_tLootText.text = "Loot: " + PlayerPrefs.GetInt("Loot", 0).ToString();//Update the displayed loot text.
		m_tDeathsText.text = "Deaths: " + PlayerPrefs.GetInt("Deaths", 0).ToString();//Update the displayed deaths text.
	}

	//The start button
	public void Begin()
	{
		//Hide every object in m_gObjectsToHideOnStart.
		foreach (GameObject gameObject in m_gObjectsToHideOnStart)
		{
			gameObject.SetActive(false);
		}

		//Reveal every object in m_gObjectsToUnHideOnStart.
		foreach (GameObject gameObject in m_gObjectsToUnHideOnStart)
		{
			gameObject.SetActive(true);
		}
	}

	//The return to title button.
	public void Title()
	{
		//Reveal every object in m_gObjectsToHideOnStart.
		foreach (GameObject gameObject in m_gObjectsToHideOnStart)
		{
			gameObject.SetActive(true);
		}

		//Hide every object in m_gObjectsToUnHideOnStart.
		foreach (GameObject gameObject in m_gObjectsToUnHideOnStart)
		{
			gameObject.SetActive(false);
		}

		Time.timeScale = 1.0f;//Resume the game.
	}

	//The pause/menu button.
	public void Menu()
	{
		//Hide every object in m_gObjectsToHideOnMenu.
		foreach (GameObject gameObject in m_gObjectsToHideOnMenu)
		{
			gameObject.SetActive(false);
		}

		//Reveal every object in m_gObjectsToUnHideOnMenu.
		foreach (GameObject gameObject in m_gObjectsToUnHideOnMenu)
		{
			gameObject.SetActive(true);
		}

		Time.timeScale = 0.0f;//Pauses the game.
		m_tLootText.text = "Loot: " + PlayerPrefs.GetInt("Loot", 0).ToString();//Update the displayed loot on the menu.
		m_tDeathsText.text = "Deaths: " + PlayerPrefs.GetInt("Deaths", 0).ToString();//Update the displayed deaths on the menu.
	}

	//The close menu / resume button.
	public void Resume()
	{
		//Reveal every object in m_gObjectsToHideOnMenu.
		foreach (GameObject gameObject in m_gObjectsToHideOnMenu)
		{
			gameObject.SetActive(true);
		}

		//Hide every object in m_gObjectsToUnHideOnMenu.
		foreach (GameObject gameObject in m_gObjectsToUnHideOnMenu)
		{
			gameObject.SetActive(false);
		}

		Time.timeScale = 1.0f;//Resume the game.
	}

	public void ResetScore()
	{
		PlayerPrefs.DeleteAll();//Deletes the players save.
		m_tLootText.text = "Loot: " + 0;//Set the displayed loot to be 0.
		m_tDeathsText.text = "Deaths: " + 0;//set the displayed deaths to be 0.
	}
}
