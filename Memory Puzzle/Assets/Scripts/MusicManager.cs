using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
	// Ссылка на компонент AudioSource для воспроизведения музыки
	public AudioSource audioSource;

	// Ссылка на кнопку
	public Button toggleButton;

	// Флаг для отслеживания состояния музыки (включена или выключена)
	private bool isMusicPlaying = true;

	void Start()
	{
		// Проверяем, что у нас есть ссылка на AudioSource
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}

		// Проверяем, что у нас есть ссылка на кнопку
		if (toggleButton != null)
		{
			toggleButton.onClick.AddListener(ToggleMusic); // Добавляем обработчик события
		}

		// Начинаем воспроизведение музыки при старте
		if (audioSource != null && !audioSource.isPlaying)
		{
			audioSource.Play();
		}
	}

	// Метод для переключения состояния музыки
	void ToggleMusic()
	{
		if (audioSource != null)
		{
			if (isMusicPlaying)
			{
				audioSource.Pause(); // Приостановить музыку
				isMusicPlaying = false;
			}
			else
			{
				audioSource.Play(); // Воспроизвести музыку
				isMusicPlaying = true;
			}
		}
	}
}
