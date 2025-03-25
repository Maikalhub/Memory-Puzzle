using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public GameObject loadingScreen; // Объект, который будет показывать экран загрузки
	public Slider loadingBar;        // Индикатор загрузки (например, слайдер)
	public Dropdown sceneDropdown;   // Выпадающий список для выбора сцены

	// Метод для начала загрузки сцены
	public void OnLoadButtonClicked()
	{
		// Показываем экран загрузки
		loadingScreen.SetActive(true);

		// Получаем выбранную сцену из выпадающего списка
		int sceneIndex = sceneDropdown.value;

		// Загружаем сцену асинхронно
		StartCoroutine(LoadSceneAsync(sceneIndex));
	}

	// Асинхронная загрузка сцены
	private IEnumerator LoadSceneAsync(int sceneIndex)
	{
		// Начинаем асинхронную загрузку сцены
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		// Не будем менять сцену, пока она не будет полностью загружена
		operation.allowSceneActivation = false;

		// Пока сцена загружается, обновляем индикатор прогресса
		while (!operation.isDone)
		{
			// Обновляем прогресс загрузки
			loadingBar.value = operation.progress;

			// Если загрузка завершена (operation.progress примерно 0.9), активируем сцену
			if (operation.progress >= 0.9f)
			{
				// Ждем несколько секунд, чтобы пользователь увидел завершение загрузки
				yield return new WaitForSeconds(1f);

				// Активируем сцену
				operation.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
