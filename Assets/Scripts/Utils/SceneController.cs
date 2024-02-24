using System.Collections;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SceneController : MonoBehaviour
{
    #region Singleton
    private static SceneController instance;

    public static SceneController Instance { 
        get {
            return instance;
        }
    }

    #endregion

    #region  Serialized Fields

    [Header("Canvas Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject creditPanel;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject endScreenPanel;
    [SerializeField] GameObject fadeImage;
    [Space(10)]

    [Header("Loading Settings")]
    [SerializeField] Slider loadingBar;
    [SerializeField] float loadingDelta = 0.9f;
    [Space(10)]

    [Header("Audio Settings")]
    [SerializeField] GameObject musicMuteImage;
    [SerializeField] AudioSource audioSource;
    [Space(10)]

    [Header("End Screen")]
    [SerializeField] TMP_Text scoreNumBerText;
    [SerializeField] TMP_Text objectDestroyedNumBerText;

    #endregion

    #region  Fields

    private Animator animator;
    private int levelToLoad = 0;
    private float target = 0;
    private bool isGamePaused = false;
    private bool isGameMusicMuted = false;
    private bool isMainMenuActive = true;
    private bool isGameOver = false;
    private bool isInsideGameOver = false;
    private bool setCreditActive = false;

    #endregion

    #region Properties

    public bool IsGamePaused
    {
        get { return isGamePaused; }
    }     

    public bool IsGameMusicMuted
    {
        get { return isGameMusicMuted; }
    } 

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { SetEndScreen(value); }
    } 

    #endregion


    private void Awake()
    {
        if (instance == null)
        {
            instance = this as SceneController;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (loadingPanel.activeSelf)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, target, loadingDelta * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(!pausePanel.activeSelf && !isMainMenuActive && !loadingPanel.activeSelf 
                && !fadeImage.activeSelf && !creditPanel.activeSelf && !endScreenPanel.activeSelf)
            {
                SetPauseScreen(true);
            }
            else if(pausePanel.activeSelf)
            {
                SetPauseScreen(false);
            }
        }
    }


    #region Public Methods

    public void LoadMainMenu()
    {
        isMainMenuActive = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 1f;
        FadeAndLoadScene(0);
    }

    public void LoadGame()
    {
        FadeAndLoadScene(1);
        isMainMenuActive = false;
    }

    public void CreditsGame()
    {
        if(isGameOver)
        {
            setCreditActive = true;
            LoadMainMenu();
        }
        else
        {
            mainMenuPanel.SetActive(false);
            creditPanel.SetActive(true);
        }
    }

    public void BackGame()
    {
        mainMenuPanel.SetActive(true);
        creditPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void ContinueGame()
    {
        SetPauseScreen(false);
    }

    public void MuteMusic()
    {
        if(!isGameMusicMuted)
        {
            audioSource.volume = 0f;
            musicMuteImage.SetActive(true);
            isGameMusicMuted = true;
        }
        else
        {
            audioSource.volume = 1f;
            musicMuteImage.SetActive(false);
            isGameMusicMuted = false;
        }
    }

    public void OnFadeOutComplete()
    {
        if(isInsideGameOver)
        {
            animator.SetBool("IsFadingOut", false);
            endScreenPanel.SetActive(true);
            scoreNumBerText.text = GameManager.score.ToString();
            objectDestroyedNumBerText.text = (GameManager.score / 5).ToString();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            isInsideGameOver = false;
        }
        else 
        {
            LoadSceneAsync();
        }
    }

    #endregion

    #region Private Methods

    private void SetPauseScreen(bool setActive)
    {
        pausePanel.SetActive(setActive);
        isGamePaused = setActive;

        if(setActive)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = setActive;
    }

    private void SetEndScreen(bool setActive)
    {
        isGameOver = setActive;
        isInsideGameOver = setActive;

        if(setActive)
        {
            animator.SetBool("IsFadingOut", true);
        }
        else
        {
            endScreenPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        
    }

    private async void LoadSceneAsync()
    {
        loadingPanel.SetActive(true);
        loadingBar.value = 0;
        target = 0;

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        operation.allowSceneActivation = false;

        do
        {
            await Task.Delay(1000);
            target = operation.progress;
        }
        while (loadingBar.value < 0.9f);

        target = 1;
        await Task.Delay(1000);

        operation.allowSceneActivation = true;
        pausePanel.SetActive(false);
        loadingPanel.SetActive(false);
        if(endScreenPanel.activeSelf) { SetEndScreen(false); }

        animator.SetBool("IsFadingOut", false);
        mainMenuPanel.SetActive(isMainMenuActive);

        if(setCreditActive)
        {
            creditPanel.SetActive(true);
            setCreditActive = false;
        }

        isGameOver = false;
        isInsideGameOver = false;
    }

    private void FadeAndLoadScene(int sceneBuildIndex)
    {
        levelToLoad = sceneBuildIndex;
        animator.SetBool("IsFadingOut", true);
        // StartCoroutine(StartFadeAudio(1.0f, 0));
    }
    
    private IEnumerator StartFadeAudio(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    #endregion

}
