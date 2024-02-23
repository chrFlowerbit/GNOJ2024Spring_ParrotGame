using System.Collections;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject credit;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Slider loadingBar;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float loadingDelta = 0.9f;
    [SerializeField] GameObject musicMuteImage;
    [SerializeField] Button startGameButton;
    [SerializeField] Button creditsGameButton;
    [SerializeField] Button quitGameButton;

    #endregion

    #region  Fields

    private Animator animator;
    private int levelToLoad = 0;
    private float target = 0;
    private bool isGamePaused = false;
    private bool isGameMusicMuted = false;
    private bool isMainMenuActive = true;

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
        if (loadingScreen.activeSelf)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, target, loadingDelta * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(!pauseScreen.activeSelf && !isMainMenuActive)
            {
                SetPauseScreen(true);
            }
            else if(pauseScreen.activeSelf)
            {
                SetPauseScreen(false);
            }
        }
    }


    #region Public Methods

    public void LoadMainMenu()
    {
        SetPauseScreen(false);
        isMainMenuActive = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        FadeAndLoadScene(0);
    }

    public void LoadGame()
    {
        FadeAndLoadScene(1);
        isMainMenuActive = false;
        mainMenu.SetActive(false);
    }

    public void CreditsGame()
    {
        mainMenu.SetActive(false);
        credit.SetActive(true);
        Debug.Log("Credits");
    }

    public void BackGame()
    {
        mainMenu.SetActive(true);
        credit.SetActive(false);
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
        LoadSceneAsync();
    }

    #endregion

    #region Private Methods

    private void SetPauseScreen(bool setActive)
    {
        pauseScreen.SetActive(setActive);
        isGamePaused = setActive;
        Cursor.visible = setActive;

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
    }

    private async void LoadSceneAsync()
    {
        loadingScreen.SetActive(true);
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
        loadingScreen.SetActive(false);
        animator.SetBool("IsFadingOut", false);

        if(isMainMenuActive)
        {
            mainMenu.SetActive(true);
        }
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
