using UnityEngine;

public class LevelData : MonoBehaviour
{
    public Texture2D LevelMap { get; private set; }

    public string LevelName { get; private set; }
    public int LevelIndex { get; private set; }
    public bool IsUnlocked
    {
        get { return isUnlocked; }
        set { isUnlocked = value; }
    }

    public int Width { get; private set; }
    public int Height { get; private set; }

    private bool isUnlocked;

    public void Initialize(Texture2D levelMap, string levelName, int levelIndex)
    {
        LevelMap = levelMap;
        LevelName = levelName;
        LevelIndex = levelIndex;

        Width = LevelMap.width;
        Height = LevelMap.height;

        //IsUnlocked = (LevelIndex < LevelManager.Instance.LevelReached) ? true : false;
    }   
}
