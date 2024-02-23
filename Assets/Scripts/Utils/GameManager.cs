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

    public TMP_Text scoreText; // Ссылка на текстовое поле для отображения счета
    public TMP_Text timerText; // Ссылка на текстовое поле для отображения таймера
    public Slider timerSlider; // Ссылка на слайдер для отображения градиента таймера

    // Другие переменные и методы игрового менеджера

    public static int score = 0; // Переменная для отслеживания счета
    private float counter = 0; // Начальное значение таймера
    public float maxTime = 60f; // Начальное значение таймера

    public static GameManager instance;
    void Start()
    {
        instance = this;
        counter = maxTime;
    }
    void FixedUpdate()
    {
        // Update the timer value every FixedUpdate
        UpdateTimer();
        UpdateTimerUI();
    }
    // Метод для обновления отображения счета
    void UpdateScoreUI(int score)
    {
        scoreText.text = score.ToString(); // Обновляем текст счета
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
   

    // Метод для увеличения счета
    public static void IncreaseScore(int points)
    {
        score += points; // Увеличиваем счет на указанное количество очков
        GameManager.instance.UpdateScoreUI(score); // Обновляем отображение счета
    }
}
