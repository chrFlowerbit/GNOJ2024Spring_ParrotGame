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
    [SerializeField] string gamesNowUrl;
    [Space(10)]

    [Header("Loading Settings")]
    [SerializeField] Slider loadingBar;
    [SerializeField] float loadingDelta = 0.9f;
    [Space(10)]

    [Header("Audio Settings")]
    [SerializeField] GameObject musicMuteImage;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float musicVolume = 0.025f;
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip gamePlayClip;
    [SerializeField] AudioClip endClip;
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
    private bool isCutscenePlaying = false;
    private bool isPlaySceneLoaded = false;
    private float countDown = 64f;

    private static int numberOfPlays = 0;

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
                && !fadeImage.activeSelf && !creditPanel.activeSelf && !endScreenPanel.activeSelf && !isCutscenePlaying)
            {
                SetPauseScreen(true);
            }
            else if(pausePanel.activeSelf)
            {
                SetPauseScreen(false);
            }

            if(isCutscenePlaying)
            {
                countDown = 0;
                foreach (var audio in FindObjectsOfType<AudioSource>())
                {
                    StartCoroutine(StartFadeAudio(1.0f, 0, audio));
                }
            }
        }

        if (numberOfPlays == 0 && isCutscenePlaying)
        {
            if(countDown >= 0) 
            {
                countDown -= Time.deltaTime;
            }

            if(countDown < 0)
            {
                countDown = 0;
                isCutscenePlaying = false;
                FadeAndLoadScene(1);
                numberOfPlays++;
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
        if(numberOfPlays == 0)
        {
            FadeAndLoadScene(2);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            FadeAndLoadScene(1);
            numberOfPlays++;
        }

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

    public void GamesNow()
    {
        Application.OpenURL(gamesNowUrl);
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
            audioSource.volume = musicVolume;
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
            audioSource.Stop();
            audioSource.volume = musicVolume * 2.5f;
            audioSource.PlayOneShot(endClip);
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

        isPlaySceneLoaded = levelToLoad != 0 && levelToLoad != 2; 
        if(mainMenuPanel.activeSelf)
        {
            PlayClip(mainMenuClip);
        }
        if(isPlaySceneLoaded)
        {
            PlayClip(gamePlayClip);
        }

        if(setCreditActive)
        {
            creditPanel.SetActive(true);
            setCreditActive = false;
        }

        isGameOver = false;
        isInsideGameOver = false;

        if(numberOfPlays == 0)
        {
            isCutscenePlaying = true;
        }

    }

    private void FadeAndLoadScene(int sceneBuildIndex)
    {
        levelToLoad = sceneBuildIndex;
        animator.SetBool("IsFadingOut", true);
        StartCoroutine(StartFadeAudio(1.0f, 0, audioSource));
    }
    
    public IEnumerator StartFadeAudio(float duration, float targetVolume, AudioSource audio)
    {
        float currentTime = 0;
        float start = audio.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    private void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = musicVolume;
        audioSource.Play(0);
    }

    #endregion

}
