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

    public static int score = 0; // ���������� ��� ������������ �����
    private float counter = 0; // ��������� �������� �������
    public float maxTime = 60f; // ��������� �������� �������

    public int maxMana = 10; // Maximum mana capacity
    public int currentMana = 0; // Current mana
    public Slider manaSlider; // Slider to display mana


    private SceneController sceneController;
    public static GameManager instance;
    public static bool canEmitForce = false;

    private void Awake() {
        sceneController = FindObjectOfType<SceneController>();
    }

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
    // ����� ��� ���������� ����������� �����
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
        scoreText.text = score.ToString(); // ��������� ����� �����
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
        if(!sceneController.IsGameOver)
        {
            float timeElapsed = Time.fixedDeltaTime;
            counter -= timeElapsed;
        }

        // If the timer goes below 0, clamp it to 0
        if (counter < 0f)
        {
            counter = 0f;
            // Optionally, you can handle game over logic here
            Debug.Log("Time's up!");
            sceneController.IsGameOver = true;
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
    public static void IncreaseScore(int points)
    {
        score += points; // ����������� ���� �� ��������� ���������� �����
        instance.UpdateScoreUI(score); // ��������� ����������� �����
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
