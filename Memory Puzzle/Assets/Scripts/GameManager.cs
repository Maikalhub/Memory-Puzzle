using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;  // ���������� TextMeshPro

public class GameManager : MonoBehaviour
{
	public List<Button> btns = new List<Button>();

	[SerializeField] private Sprite bgImage;  // ������� ����������� ��� ������ (�������� ����)
	[SerializeField] private Sprite cardBackground;  // ��� ��� ����
	[SerializeField] private TextMeshProUGUI scoreText;  // ���������� TextMeshProUGUI ��� ����������� �����
	[SerializeField] private TextMeshProUGUI timeText;   // ��� ����������� ����������� �������
	[SerializeField] private TextMeshProUGUI resultText; // ��� ����������� ���������� (������/���������)

	// ������ �� �����
	[SerializeField] private AudioClip buttonClickSound;    // ���� ������� �� ������ � ������
	[SerializeField] private AudioClip menuButtonClickSound; // ���� ������� �� ������ ���� (�����, �����)
	[SerializeField] private AudioClip victorySound;         // ���� ������
	[SerializeField] private AudioClip defeatSound;          // ���� ���������
	[SerializeField] private AudioClip cardFlipSound;        // ���� �������� �����
	[SerializeField] private AudioClip cardShowSound;        // ���� ��������� ����� (��������, ����� ����� ���������� �������)
	[SerializeField] private AudioClip backgroundMusic;      // ������� ������

	// ��� �����
	[SerializeField] private GameObject pauseMenu;  // ������ � ���� �����
	[SerializeField] private Button pauseButton;    // ������ ��� �����

	private bool isPaused = false;  // ���� �����


	private AudioSource audioSource;  // �������� �����

	private Sprite[] puzzleImages1;
	private Sprite[] puzzleImages2;
	private Sprite[] puzzleImages3;
	private Sprite[] puzzleImages4;
	private Sprite[] puzzleImages5;
	private Sprite[] puzzleImages6;
	private Sprite[] puzzleImages7;
	private Sprite[] puzzleImages8;




	public List<Sprite> gamePuzzles = new List<Sprite>();

	private bool firstGuess, secondGuess;
	private int countGuesses;
	private int gameGuesses;

	private int firstGuessIndex, secondGuessIndex;

	private int score = 0;  // ���� ����
	private float timeRemaining = 45f;  // ����� ������� (��������, 60 ������)
	private bool isGameOver = false;   // ���� ���������� ����

	private void Awake()
	{
		// �������� ��������� AudioSource
		audioSource = GetComponent<AudioSource>();

		// ���������, ���� AudioSource �� ������
		if (audioSource == null)
		{
			Debug.LogError("AudioSource component not found on this GameObject.");
		}

		// ��������� ����������� ��� ������
		puzzleImages1 = Resources.LoadAll<Sprite>("Images/1");
		puzzleImages2 = Resources.LoadAll<Sprite>("Images/2");
		puzzleImages3 = Resources.LoadAll<Sprite>("Images/3");
		puzzleImages4 = Resources.LoadAll<Sprite>("Images/4");
		puzzleImages5 = Resources.LoadAll<Sprite>("Images/5");
		puzzleImages6 = Resources.LoadAll<Sprite>("Images/6");
		puzzleImages7 = Resources.LoadAll<Sprite>("Images/7");
		puzzleImages8 = Resources.LoadAll<Sprite>("Images/8");

	}

	void Start()
	{
		// ���� ������ �� scoreText �� ����������� � ����������, �������� ����� ��� ����������
		if (scoreText == null)
		{
			scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
			if (scoreText == null)
			{
				Debug.LogError("�� ������� ����� ������ 'ScoreText'. ��������� ��� � ���������.");
			}
		}

		if (timeText == null)
		{
			timeText = GameObject.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
			if (timeText == null)
			{
				Debug.LogError("�� ������� ����� ������ 'TimeText'. ��������� ��� � ���������.");
			}
		}

		if (resultText == null)
		{
			resultText = GameObject.Find("ResultText")?.GetComponent<TextMeshProUGUI>();
			if (resultText == null)
			{
				Debug.LogError("�� ������� ����� ������ 'ResultText'. ��������� ��� � ���������.");
			}
		}

		// ���������� ������ ����� ���������� (�� ����� ������������ � ����� ����)
		resultText.gameObject.SetActive(false);

		GetButtons();
		AddListeners();
		AddGamePuzzles();

		if (scoreText != null)
		{
			UpdateScoreText();  // ���������� ��������� ����� � ��������� ��������� �����
		}

		if (timeText != null)
		{
			UpdateTimeText();  // ���������� ��������� ����� � ��������� ��������
		}

		// �������� ������� ������
		PlayBackgroundMusic();
	}

	void Update()
	{
		if (isPaused)
			return;  // ���� ���� �� �����, ������ �� ���������

		// ���� ���� ���������, �� ��������� �����
		if (isGameOver)
			return;

		// ������ �������
		timeRemaining -= Time.deltaTime;
		if (timeRemaining <= 0)
		{
			timeRemaining = 0;
			EndGame(false);  // ����� �����, ���� ��������� � ����������
		}

		// ��������� ����������� �������
		UpdateTimeText();
	}

	void GetButtons()
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag("puzzleBtn");
		for (int i = 0; i < objects.Length; i++)
		{
			btns.Add(objects[i].GetComponent<Button>());
			btns[i].image.sprite = cardBackground;  // ���������� ��� �� 10.jpg
		}
	}

	void AddGamePuzzles()
	{
		List<Sprite> allSprites = new List<Sprite>();

		allSprites.AddRange(puzzleImages1);
		allSprites.AddRange(puzzleImages2);
		allSprites.AddRange(puzzleImages3);
		allSprites.AddRange(puzzleImages4);
		allSprites.AddRange(puzzleImages5);
		allSprites.AddRange(puzzleImages6);
		allSprites.AddRange(puzzleImages7);
		allSprites.AddRange(puzzleImages8);

		int pairsNeeded = btns.Count / 2;

		for (int i = 0; i < pairsNeeded; i++)
		{
			Sprite puzzleSprite = allSprites[i % allSprites.Count];
			gamePuzzles.Add(puzzleSprite);
			gamePuzzles.Add(puzzleSprite);  // ������ ���� ����
		}

		Shuffle(gamePuzzles);
	}

	void Shuffle(List<Sprite> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Sprite temp = list[i];
			int randomIndex = Random.Range(i, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
	}

	void AddListeners()
	{
		foreach (Button btn in btns)
		{
			btn.onClick.AddListener(() => PickPuzzle());
		}
	}

	public void PickPuzzle()
	{
		if (firstGuess && secondGuess)
			return;

		Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
		int clickedIndex = int.Parse(clickedButton.name);
		clickedButton.image.sprite = gamePuzzles[clickedIndex];

		// ������������� ���� ��� ��������� �����
		audioSource.PlayOneShot(cardFlipSound);

		if (!firstGuess)
		{
			firstGuess = true;
			firstGuessIndex = clickedIndex;
		}
		else if (!secondGuess)
		{
			secondGuess = true;
			secondGuessIndex = clickedIndex;

			StartCoroutine(CheckMatch());
		}
	}

	private IEnumerator CheckMatch()
	{
		yield return new WaitForSeconds(0.4f);

		if (gamePuzzles[firstGuessIndex] == gamePuzzles[secondGuessIndex])
		{
			btns[firstGuessIndex].interactable = false;
			btns[secondGuessIndex].interactable = false;

			score++;
			UpdateScoreText();
		}
		else
		{
			btns[firstGuessIndex].image.sprite = cardBackground;  // ��������������� ��� �����, ���� �� �������
			btns[secondGuessIndex].image.sprite = cardBackground;
		}

		firstGuess = false;
		secondGuess = false;

		// ��������, ��������� �� ����
		if (IsGameCompleted())
		{
			EndGame(true);  // ���� ��� ���� �������, ������
		}
	}

	void UpdateScoreText()
	{
		if (scoreText != null)
		{
			scoreText.text = " " + score;  // ��������� ����� �� UI
		}
	}

	void UpdateTimeText()
	{
		if (timeText != null)
		{
			timeText.text = " " + Mathf.Ceil(timeRemaining).ToString();  // ��������� ����� �������
		}
	}

	// ��������, ��������� �� ���� (��� ���� �������)
	bool IsGameCompleted()
	{
		foreach (Button btn in btns)
		{
			if (btn.interactable) // ���� ���� ��� �������� ������, ���� �� ���������
				return false;
		}
		return true;
	}

	void EndGame(bool isVictory)
	{
		isGameOver = true;

		// ��������� ������� ������, ���� ��� ������
		audioSource.Stop();

		// �������� ����� ������/���������
		if (isVictory)
		{
			resultText.text = "Victory!";
			audioSource.PlayOneShot(victorySound);  // ���� ������
		}
		else
		{
			resultText.text = " Loser!";
			audioSource.PlayOneShot(defeatSound);  // ���� ���������
		}

		resultText.gameObject.SetActive(true);  // ���������� ���������

		// ���������, ��� ����� ������ ������ ���� UI ���������
		Canvas resultCanvas = resultText.GetComponentInParent<Canvas>();
		if (resultCanvas != null)
		{
			resultCanvas.sortingOrder = 10;  // ������������� ������� ������������� �������
		}
	}


	// ���������/���������� �����
	public void Pause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			Time.timeScale = 0f;  // ������������� �����
			pauseMenu.SetActive(true);  // ���������� ���� �����
			audioSource.Pause();  // ���������������� ������
		}
		else
		{
			Time.timeScale = 1f;  // ���������� ���������� �����
			pauseMenu.SetActive(false);  // �������� ���� �����
			audioSource.UnPause();  // ������������ ������
		}
	}

	public void ResumeGame()
	{
		Pause();  // ���������� ���� � �������� ���������
	}



	public void reGame()
	{
		// ���� ������� �� ������ ����
		audioSource.PlayOneShot(menuButtonClickSound);
		SceneManager.LoadScene("Game");
	}

	public void reMenu()
	{
		// ���� ������� �� ������ ����
		audioSource.PlayOneShot(menuButtonClickSound);
		SceneManager.LoadScene("Menu");
	}

	// ����� ��� ��������������� ������� ������
	private void PlayBackgroundMusic()
	{
		if (audioSource != null && backgroundMusic != null)
		{
			audioSource.clip = backgroundMusic;
			audioSource.loop = true;  // ����������� ������
			audioSource.Play();
		}
	}
}
