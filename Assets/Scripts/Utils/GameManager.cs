using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //score tracker
    //timer  

    //count down

    public TMP_Text scoreText; // ������ �� ��������� ���� ��� ����������� �����
    public TMP_Text timerText; // ������ �� ��������� ���� ��� ����������� �������
    public Slider timerSlider; // ������ �� ������� ��� ����������� ��������� �������

    // ������ ���������� � ������ �������� ���������

    public  int score = 0; // ���������� ��� ������������ �����
    private float counter = 0; // ��������� �������� �������
    public float maxTime = 60f; // ��������� �������� �������


    void Start()
    {
        counter = maxTime;
        UpdateScoreUI(); // ��������� ����������� ����� ��� ������� ����
        UpdateTimerUI(); // ��������� ����������� ������� ��� ������� ����
    }
    void FixedUpdate()
    {
        // Update the timer value every FixedUpdate
        UpdateTimer();
        UpdateTimerUI();
    }
    // ����� ��� ���������� ����������� �����
    void UpdateScoreUI()
    {
        scoreText.text = score.ToString(); // ��������� ����� �����
    }

    void UpdateTimer()
    {
        // Calculate the time elapsed between frames and subtract it from the timer
        float timeElapsed = Time.fixedDeltaTime;
        counter -= timeElapsed;

        // If the timer goes below 0, clamp it to 0
        if (counter < 0f)
        {
            counter = 0f;
            // Optionally, you can handle game over logic here
            Debug.Log("Time's up!");
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(counter / 60);
        int seconds = Mathf.FloorToInt(counter % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = timerString;

        float fillAmount = counter / maxTime;
        timerSlider.value = fillAmount;
    }
   

    // ����� ��� ���������� �����
    public void IncreaseScore(int points)
    {
        score += points; // ����������� ���� �� ��������� ���������� �����
        UpdateScoreUI(); // ��������� ����������� �����
    }
}
