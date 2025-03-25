using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;  // Подключаем TextMeshPro

public class GameManager : MonoBehaviour
{
	public List<Button> btns = new List<Button>();

	[SerializeField] private Sprite bgImage;  // Базовое изображение для кнопок (картинка фона)
	[SerializeField] private Sprite cardBackground;  // Фон для карт
	[SerializeField] private TextMeshProUGUI scoreText;  // Используем TextMeshProUGUI для отображения счета
	[SerializeField] private TextMeshProUGUI timeText;   // Для отображения оставшегося времени
	[SerializeField] private TextMeshProUGUI resultText; // Для отображения результата (Победа/Поражение)

	// Ссылки на звуки
	[SerializeField] private AudioClip buttonClickSound;    // Звук нажатия на кнопку с картой
	[SerializeField] private AudioClip menuButtonClickSound; // Звук нажатия на кнопки меню (пауза, домой)
	[SerializeField] private AudioClip victorySound;         // Звук победы
	[SerializeField] private AudioClip defeatSound;          // Звук поражения
	[SerializeField] private AudioClip cardFlipSound;        // Звук поворота карты
	[SerializeField] private AudioClip cardShowSound;        // Звук появления карты (например, когда карта становится видимой)
	[SerializeField] private AudioClip backgroundMusic;      // Фоновая музыка

	// Для паузы
	[SerializeField] private GameObject pauseMenu;  // Панель с меню паузы
	[SerializeField] private Button pauseButton;    // Кнопка для паузы

	private bool isPaused = false;  // Флаг паузы


	private AudioSource audioSource;  // Источник звука

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

	private int score = 0;  // Счет игры
	private float timeRemaining = 45f;  // Лимит времени (например, 60 секунд)
	private bool isGameOver = false;   // Флаг завершения игры

	private void Awake()
	{
		// Получаем компонент AudioSource
		audioSource = GetComponent<AudioSource>();

		// Проверяем, если AudioSource не найден
		if (audioSource == null)
		{
			Debug.LogError("AudioSource component not found on this GameObject.");
		}

		// Загружаем изображения для пазлов
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
		// Если ссылка на scoreText не установлена в инспекторе, пытаемся найти его программно
		if (scoreText == null)
		{
			scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
			if (scoreText == null)
			{
				Debug.LogError("Не удалось найти объект 'ScoreText'. Проверьте имя и компонент.");
			}
		}

		if (timeText == null)
		{
			timeText = GameObject.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
			if (timeText == null)
			{
				Debug.LogError("Не удалось найти объект 'TimeText'. Проверьте имя и компонент.");
			}
		}

		if (resultText == null)
		{
			resultText = GameObject.Find("ResultText")?.GetComponent<TextMeshProUGUI>();
			if (resultText == null)
			{
				Debug.LogError("Не удалось найти объект 'ResultText'. Проверьте имя и компонент.");
			}
		}

		// Изначально скрыть текст результата (он будет отображаться в конце игры)
		resultText.gameObject.SetActive(false);

		GetButtons();
		AddListeners();
		AddGamePuzzles();

		if (scoreText != null)
		{
			UpdateScoreText();  // Изначально обновляем текст с начальным значением счета
		}

		if (timeText != null)
		{
			UpdateTimeText();  // Изначально обновляем текст с начальным временем
		}

		// Включаем фоновую музыку
		PlayBackgroundMusic();
	}

	void Update()
	{
		if (isPaused)
			return;  // Если игра на паузе, ничего не обновляем

		// Если игра закончена, не обновляем время
		if (isGameOver)
			return;

		// Отсчет времени
		timeRemaining -= Time.deltaTime;
		if (timeRemaining <= 0)
		{
			timeRemaining = 0;
			EndGame(false);  // Время вышло, игра завершена с поражением
		}

		// Обновляем отображение времени
		UpdateTimeText();
	}

	void GetButtons()
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag("puzzleBtn");
		for (int i = 0; i < objects.Length; i++)
		{
			btns.Add(objects[i].GetComponent<Button>());
			btns[i].image.sprite = cardBackground;  // Используем фон из 10.jpg
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
			gamePuzzles.Add(puzzleSprite);  // Делаем пару карт
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

		// Воспроизводим звук при раскрытии карты
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
			btns[firstGuessIndex].image.sprite = cardBackground;  // Восстанавливаем фон карты, если не совпало
			btns[secondGuessIndex].image.sprite = cardBackground;
		}

		firstGuess = false;
		secondGuess = false;

		// Проверим, завершена ли игра
		if (IsGameCompleted())
		{
			EndGame(true);  // Если все пары найдены, победа
		}
	}

	void UpdateScoreText()
	{
		if (scoreText != null)
		{
			scoreText.text = " " + score;  // Обновляем текст на UI
		}
	}

	void UpdateTimeText()
	{
		if (timeText != null)
		{
			timeText.text = " " + Mathf.Ceil(timeRemaining).ToString();  // Обновляем текст времени
		}
	}

	// Проверка, завершена ли игра (все пары найдены)
	bool IsGameCompleted()
	{
		foreach (Button btn in btns)
		{
			if (btn.interactable) // Если есть еще активные кнопки, игра не завершена
				return false;
		}
		return true;
	}

	void EndGame(bool isVictory)
	{
		isGameOver = true;

		// Остановим фоновую музыку, если она играет
		audioSource.Stop();

		// Настроим текст победы/поражения
		if (isVictory)
		{
			resultText.text = "Victory!";
			audioSource.PlayOneShot(victorySound);  // Звук победы
		}
		else
		{
			resultText.text = " Loser!";
			audioSource.PlayOneShot(defeatSound);  // Звук поражения
		}

		resultText.gameObject.SetActive(true);  // Показываем результат

		// Убедитесь, что текст всегда поверх всех UI элементов
		Canvas resultCanvas = resultText.GetComponentInParent<Canvas>();
		if (resultCanvas != null)
		{
			resultCanvas.sortingOrder = 10;  // Устанавливаем высокий сортировочный порядок
		}
	}


	// Включение/выключение паузы
	public void Pause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			Time.timeScale = 0f;  // Останавливаем время
			pauseMenu.SetActive(true);  // Показываем меню паузы
			audioSource.Pause();  // Приостанавливаем музыку
		}
		else
		{
			Time.timeScale = 1f;  // Возвращаем нормальное время
			pauseMenu.SetActive(false);  // Скрываем меню паузы
			audioSource.UnPause();  // Возобновляем музыку
		}
	}

	public void ResumeGame()
	{
		Pause();  // Возвращаем игру в активное состояние
	}



	public void reGame()
	{
		// Звук нажатия на кнопку меню
		audioSource.PlayOneShot(menuButtonClickSound);
		SceneManager.LoadScene("Game");
	}

	public void reMenu()
	{
		// Звук нажатия на кнопку меню
		audioSource.PlayOneShot(menuButtonClickSound);
		SceneManager.LoadScene("Menu");
	}

	// Метод для воспроизведения фоновой музыки
	private void PlayBackgroundMusic()
	{
		if (audioSource != null && backgroundMusic != null)
		{
			audioSource.clip = backgroundMusic;
			audioSource.loop = true;  // Зацикливаем музыку
			audioSource.Play();
		}
	}
}
