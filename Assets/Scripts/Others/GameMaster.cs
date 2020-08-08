using UnityEngine;

public class GameMaster : SingeltonPersistant<GameMaster>
{
    public Texture2D[] LevelMaps { get; private set; }
    public Sprite[] ButtonIconSprites { get; private set; }
    public GameObject LevelButtonPrefab { get; private set; }

    public Transform HUDCanavas { get; private set; }
    public Transform Managers { get; private set; }
    public Transform Others { get; private set; }

    public Transform MainMenuUI { get; private set; }
    public Transform LevelUI { get; private set; }

    public Transform Panels { get; private set; }

    public Transform LevelPanel { get; private set; }
    public Transform LevelContainer { get; private set; }
    public Transform HomePanel { get; private set; }
    public Transform OptionsPanel { get; private set; }

    public Transform MainCameraObject { get; private set; }

    public Transform TimeControlPanel { get; private set; }
    public Transform GameOverPanel { get; private set; }
    public Transform PausePanel { get; private set; }
    public Transform HUD { get; private set; }

    public Transform ScreenFadeImageObject { get; private set; }

    private Transform SearchTransformByName(string transformName, Transform searchParentTransform)
    {
        Transform t = searchParentTransform.Find(transformName);

        return (t ?? (t = new GameObject().transform));
    }

    protected override void Awake()
    {
        base.Awake();

        LevelMaps = Resources.LoadAll<Texture2D>("LevelMaps/") as Texture2D[];
        ButtonIconSprites = Resources.LoadAll<Sprite>("UIElements/IconSprites/") as Sprite[];
        LevelButtonPrefab = Resources.Load<GameObject>("UIElements/LevelButton") as GameObject;

        GetTransformReferences();
    }

    private void GetTransformReferences()
    {
        HUDCanavas = SearchTransformByName("HUDCanvas", transform);
        Managers = SearchTransformByName("Managers", transform);
        Others = SearchTransformByName("Others", transform);

        MainMenuUI = SearchTransformByName("MainMenuUI", HUDCanavas);
        LevelUI = SearchTransformByName("LevelUI", HUDCanavas);

        Panels = SearchTransformByName("Panels", MainMenuUI);

        LevelPanel = SearchTransformByName("LevelPanel", Panels);
        LevelContainer = SearchTransformByName("LevelContainer", LevelPanel);
        HomePanel = SearchTransformByName("HomePanel", Panels);
        OptionsPanel = SearchTransformByName("OptionsPanel", Panels);

        MainCameraObject = SearchTransformByName("MainCamera", Others);

        TimeControlPanel = SearchTransformByName("TimeControlPanel", LevelUI);
        GameOverPanel = SearchTransformByName("GameOverPanel", LevelUI);
        PausePanel = SearchTransformByName("PausePanel", LevelUI);
        HUD = SearchTransformByName("HUD", LevelUI);

        ScreenFadeImageObject = SearchTransformByName("ScreenFadeImage", HUDCanavas);
    }
}
