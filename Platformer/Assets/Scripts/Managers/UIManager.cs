using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singelton<UIManager>
{
    #region VARIABLES

    public Texture2D[] LevelMaps { get; private set; }

    private GameObject timeControlPanel;
    private GameObject hud;
    private GameObject pausePanel;
    private GameObject gameOverPanel;
    private GameObject optionsPanel;
    private GameObject mainMenuUI;
    private GameObject levelUI;
    private GameObject levelContainer;

    private int collectableCount = 0;
    private Text collectableCountText;

    private Scrollbar masterVolumeScrollbar, sfxVolumeScrollbar, musicVolumeScrollbar;
    private Image screenFadeImage;
    private bool isFading = false;

    public bool IsFading { get { return isFading; } }
    public bool IsOverUIElement
    {
        get
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    #endregion VARIABLES

    private Sprite SearchSpriteFromArray(Sprite[] sprites, string spriteName)
    {
        foreach (var sprite in sprites)
        {
            if (spriteName.Equals(sprite.name))
            {
                return sprite;
            }
        }

        return null;
    }

    private void Start()
    {
        GetReferences();
        CreateLevelButtons(LevelMaps);
        CheckUnlockedLevels();
        SetScrollbarValues();
        SetMenuUI();
        FadeScreenImage(0f);
    } 

    private void GetReferences()
    {
        LevelMaps = GameMaster.Instance.LevelMaps;

        timeControlPanel = GameMaster.Instance.TimeControlPanel.gameObject;
        hud = GameMaster.Instance.HUD.gameObject;
        pausePanel = GameMaster.Instance.PausePanel.gameObject;
        optionsPanel = GameMaster.Instance.OptionsPanel.gameObject;
        gameOverPanel = GameMaster.Instance.GameOverPanel.gameObject;
        mainMenuUI = GameMaster.Instance.MainMenuUI.gameObject;
        levelUI = GameMaster.Instance.LevelUI.gameObject;
        levelContainer = GameMaster.Instance.LevelContainer.gameObject;

        collectableCountText = levelUI.transform.Find("HUD").transform.Find("CollectableIcon").transform.Find("CountText").GetComponent<Text>();
        collectableCountText.text = collectableCount.ToString();

        var optionsPanelButtonContainer = optionsPanel.transform.Find("ButtonContainer").transform;
        masterVolumeScrollbar = optionsPanelButtonContainer.transform.Find("MasterVolumeScrollbar").GetComponent<Scrollbar>();
        sfxVolumeScrollbar = optionsPanelButtonContainer.transform.Find("SfxVolumeScrollbar").GetComponent<Scrollbar>();
        musicVolumeScrollbar = optionsPanelButtonContainer.transform.Find("MusicVolumeScrollbar").GetComponent<Scrollbar>();

        screenFadeImage = GameMaster.Instance.ScreenFadeImageObject.GetComponent<Image>();
        screenFadeImage.fillAmount = 1f;   
    }

    private void SetScrollbarValues()
    {
        masterVolumeScrollbar.value = SaveManager.Instance.LoadFloat("MasterVolume");
        sfxVolumeScrollbar.value = SaveManager.Instance.LoadFloat("SfxVolume");
        musicVolumeScrollbar.value = SaveManager.Instance.LoadFloat("MusicVolume");
    }

    private void CreateLevelButtons(Texture2D[] levelMaps)
    {
        LevelManager.Instance.InitializeLevelData(levelMaps);

        int levelIndex = 0;

        foreach (var levelMap in levelMaps)
        {
            GameObject newLevelButton = Instantiate(GameMaster.Instance.LevelButtonPrefab, levelContainer.transform, false);

            LevelData levelData = newLevelButton.AddComponent<LevelData>();
            levelData.Initialize(levelMap, levelMap.name, levelIndex);
            LevelManager.Instance.LevelDatas[levelIndex] = levelData;
            newLevelButton.name = levelData.LevelName;

            Button newButton = newLevelButton.GetComponent<Button>();
            newButton.onClick.AddListener(() => OnLevelButtonClicked(levelData));

            newButton.interactable = levelData.IsUnlocked ? true : false;
         
            Text levelNameLabel = newLevelButton.transform.Find("LevelNameLabel").GetComponent<Text>();
            levelNameLabel.text = levelData.LevelName;

            levelIndex++;
        }
    }

    public void AddCollectable(int amount)
    {
        collectableCount += amount;
        UpdateHUD();

        SfxLibrary.Instance.PlayGameSfx("Collectable");
    }

    public void CheckUnlockedLevels()
    {
        foreach (var levelData in LevelManager.Instance.LevelDatas)
        {
            Button levelButton = levelData.GetComponent<Button>();
            if (levelButton != null)
            {
                levelData.IsUnlocked = (levelData.LevelIndex < LevelManager.Instance.LevelReached) ? true : false;
                levelButton.interactable = levelData.IsUnlocked ? true : false;
            }
        }
    }

    #region UI_BUTTON_FUNCTIONS

    private void OnLevelButtonClicked(LevelData levelData)
    {
        LevelManager.Instance.LoadMap(levelData);
        LevelManager.Instance.CurrentLevelIndex = levelData.LevelIndex;
        MusicPlayer.Instance.ChangeRandomMusicTrack();

        SfxLibrary.Instance.PlayUISfx("Button1");
    }

    public void OnPauseButtonClicked()
    {
        SetPauseUI();

        SfxLibrary.Instance.PlayUISfx("Button2");
    }

    public void OnResumeButtonClicked()
    {
        SetLevelUI();

        SfxLibrary.Instance.PlayUISfx("Button2");
    }

    public void OnBackToMenuButtonClicked()
    {
        LevelManager.Instance.EmptyMap();
        MusicPlayer.Instance.ChangeRandomMusicTrack();

        SfxLibrary.Instance.PlayUISfx("Button2");
    }

    public void OnRewindButtonClicked()
    {
        TimeController.Instance.StartRewind();

        SfxLibrary.Instance.PlayUISfx("Button2");
    }

    public void OnExitGameButtonClicked()
    {
        SaveManager.Instance.SaveValue("MasterVolume", masterVolumeScrollbar.value);
        SaveManager.Instance.SaveValue("SfxVolume", sfxVolumeScrollbar.value);
        SaveManager.Instance.SaveValue("MusicVolume", musicVolumeScrollbar.value);

        SfxLibrary.Instance.PlayUISfx("Button2");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnDeleteSavesButtonClicked()
    {
        SaveManager.Instance.DeleteAll();
        SaveManager.Instance.SaveValue("LevelReached", LevelManager.Instance.LevelReached = 1);
        CheckUnlockedLevels();

        SfxLibrary.Instance.PlayUISfx("Button2");

        Debug.LogWarning("Game progress has been deleted!");
    }

    public void OnSaveProgressButtonClicked()
    {
        SfxLibrary.Instance.PlayUISfx("Button2");

        Debug.Log("Game progress has been saved!");
    }

    #endregion UI_BUTTON_FUNCTIONS

    #region UI_FUNCTIONS

    private void ResetHUD()
    {
        collectableCount = 0;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        collectableCountText.text = collectableCount.ToString();
    }

    private void SetPauseUI()
    {
        timeControlPanel.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0;

        SfxLibrary.Instance.PlayUISfx("Paused");
    }

    public void SetMenuUI()
    {
        mainMenuUI.SetActive(true);
        levelUI.SetActive(false);
        hud.SetActive(false);

        Time.timeScale = 1;
    }

    public void SetLevelUI()
    {
        mainMenuUI.SetActive(false);
        levelUI.SetActive(true);
        hud.SetActive(true);
        timeControlPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);

        Time.timeScale = 1;

        ResetHUD();
    }

    public void SetGameOverUI()
    {
        gameOverPanel.SetActive(true);
        timeControlPanel.SetActive(false);

        Time.timeScale = 0;
    }

    #endregion UI_FUNCTIONS

    #region COROUTINES

    public void FadeScreenImage(float targetFillAmount)
    {
        StartCoroutine(IFadeScreenImage(targetFillAmount, 1f));
    }

    private IEnumerator IFadeScreenImage(float targetFillAmount, float fadeSpeed)
    {
        isFading = true;
        screenFadeImage.raycastTarget = true;

        while (screenFadeImage.fillAmount != targetFillAmount)
        {
            screenFadeImage.fillAmount += screenFadeImage.fillAmount<targetFillAmount ? (1f / fadeSpeed) * Time.unscaledDeltaTime : -(1f / fadeSpeed) * Time.unscaledDeltaTime;
            yield return null;
        }

        screenFadeImage.raycastTarget = false;
        isFading = false;

        //yield return new WaitForSecondsRealtime(1);
    }

    #endregion COROUTINES
}
