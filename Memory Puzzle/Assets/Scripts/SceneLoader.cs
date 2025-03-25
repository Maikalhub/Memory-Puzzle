using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public GameObject loadingScreen; // ������, ������� ����� ���������� ����� ��������
	public Slider loadingBar;        // ��������� �������� (��������, �������)
	public Dropdown sceneDropdown;   // ���������� ������ ��� ������ �����

	// ����� ��� ������ �������� �����
	public void OnLoadButtonClicked()
	{
		// ���������� ����� ��������
		loadingScreen.SetActive(true);

		// �������� ��������� ����� �� ����������� ������
		int sceneIndex = sceneDropdown.value;

		// ��������� ����� ����������
		StartCoroutine(LoadSceneAsync(sceneIndex));
	}

	// ����������� �������� �����
	private IEnumerator LoadSceneAsync(int sceneIndex)
	{
		// �������� ����������� �������� �����
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		// �� ����� ������ �����, ���� ��� �� ����� ��������� ���������
		operation.allowSceneActivation = false;

		// ���� ����� �����������, ��������� ��������� ���������
		while (!operation.isDone)
		{
			// ��������� �������� ��������
			loadingBar.value = operation.progress;

			// ���� �������� ��������� (operation.progress �������� 0.9), ���������� �����
			if (operation.progress >= 0.9f)
			{
				// ���� ��������� ������, ����� ������������ ������ ���������� ��������
				yield return new WaitForSeconds(1f);

				// ���������� �����
				operation.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
