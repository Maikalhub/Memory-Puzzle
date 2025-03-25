using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
	// ������ �� ��������� AudioSource ��� ��������������� ������
	public AudioSource audioSource;

	// ������ �� ������
	public Button toggleButton;

	// ���� ��� ������������ ��������� ������ (�������� ��� ���������)
	private bool isMusicPlaying = true;

	void Start()
	{
		// ���������, ��� � ��� ���� ������ �� AudioSource
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}

		// ���������, ��� � ��� ���� ������ �� ������
		if (toggleButton != null)
		{
			toggleButton.onClick.AddListener(ToggleMusic); // ��������� ���������� �������
		}

		// �������� ��������������� ������ ��� ������
		if (audioSource != null && !audioSource.isPlaying)
		{
			audioSource.Play();
		}
	}

	// ����� ��� ������������ ��������� ������
	void ToggleMusic()
	{
		if (audioSource != null)
		{
			if (isMusicPlaying)
			{
				audioSource.Pause(); // ������������� ������
				isMusicPlaying = false;
			}
			else
			{
				audioSource.Play(); // ������������� ������
				isMusicPlaying = true;
			}
		}
	}
}
