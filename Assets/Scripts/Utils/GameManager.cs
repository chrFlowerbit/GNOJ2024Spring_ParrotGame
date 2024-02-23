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

    public int maxMana = 10; // Maximum mana capacity
    public int currentMana = 0; // Current mana
    public Slider manaSlider; // Slider to display mana


    public static GameManager instance;
    public static bool canEmitForce = false;
    void Start()
    {
        manaSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = Color.grey;
        instance = this;
        counter = maxTime;
        UpdateManaUI();
    }
    void FixedUpdate()
    {
        // Update the timer value every FixedUpdate
        UpdateTimer();
        UpdateTimerUI();
        UpdateManaUI();
    }
    // Метод для обновления отображения счета
    void UpdateScoreUI(int score)
    {
        currentMana++;
        
        if (currentMana >= maxMana)
        {
            canEmitForce = true;
            currentMana = maxMana;
            manaSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = Color.red;
        }

        UpdateManaUI();
        scoreText.text = score.ToString(); // Обновляем текст счета
    }
    void UpdateManaUI()
    {
        float fillAmount = currentMana / (float)maxMana;
        manaSlider.value = fillAmount;
    }
    public void ResetSliderValue()
    {
        manaSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = Color.grey;
        instance.currentMana = 0;
        canEmitForce = false;
        StartCoroutine(DepleteSliderValue(instance.manaSlider, 1, 0));
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
        instance.UpdateScoreUI(score); // Обновляем отображение счета
    }

    public static IEnumerator DepleteSliderValue(Slider slider, float duration, float delay = 0f)
    {
        // Wait for the delay
        yield return new WaitForSeconds(delay);

        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float progress = (Time.time - startTime) / duration;
            slider.value = Mathf.Lerp(slider.maxValue, slider.minValue, progress);

            yield return null;
        }

        // Set the final value to zero to ensure completeness
        slider.value = slider.minValue;
        instance.currentMana = 0;
    }

}
