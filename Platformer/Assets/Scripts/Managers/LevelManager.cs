using System.Collections;
using UnityEngine;

[System.Serializable]
public class ColorToPrefab
{
	public GameObject Prefab;
	public Color32 TypeColor;
}

public class LevelManager : Singelton<LevelManager>
{
	private int levelReached;
	private int currentLevelIndex;
	private bool isLevelCreated = false;

	public LevelData[] LevelDatas { get; private set; }
	public Transform[] LevelContainers { get; private set; }

	public bool IsLevelCreated { get { return isLevelCreated; } }
	public int LevelReached
	{
		get
		{
			return levelReached;
		}
		set
		{
			levelReached = value;
		}
	}
	public int CurrentLevelIndex
	{
		get
		{
			return currentLevelIndex;
		}
		set
		{
			currentLevelIndex = value;
		}
	}

	private int width;
	private int height;

	public ColorToPrefab[] ColorsToPrefabs;

	private void Awake()
	{
		levelReached = SaveManager.Instance.LoadInt("LevelReached");
	}

	public void InitializeLevelData(Texture2D[] levelMaps)
	{
		LevelDatas = new LevelData[levelMaps.Length];
		LevelContainers = new Transform[LevelDatas.Length];
	}

	public void LoadMap(LevelData levelData)
	{
		StartCoroutine(ILoadLevel(levelData));
	}

	public Transform GetLevelContainer()
	{
		return null;
	}

	private void SpawnTileAt(Color32 pixelColor,int x,int y, Transform parent)
	{
		if(pixelColor.a <= 0)
		{
			return;
		}

		foreach (ColorToPrefab colorToPrefab in ColorsToPrefabs)
		{
			if (pixelColor.Equals(colorToPrefab.TypeColor))
			{
				if(colorToPrefab.Prefab != null)
				{
					GameObject gameObject = ObjectPoolManager.Instance.GetObjectFromPool(colorToPrefab.Prefab.name);
					gameObject.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.identity);
					gameObject.transform.SetParent(parent);
					gameObject.name = colorToPrefab.Prefab.name;
					return;
				}
				else
				{
					Debug.Log(colorToPrefab.Prefab.name);
					colorToPrefab.Prefab = new GameObject("EmptyGameObject");
					return;
				}
			}
		}
	}

	public void EmptyMap()
	{
		StartCoroutine(IEmptyLevel());
	}

	public void OnLevelCompleted()
	{
		if (currentLevelIndex.Equals(levelReached - 1))
		{
			SaveManager.Instance.SaveValue("LevelReached", levelReached++);
			UIManager.Instance.CheckUnlockedLevels();
		}
	}

	#region COROUTINES

	private IEnumerator ILoadLevel(LevelData levelData)
	{
		Color32[] allLevelMapPixels = levelData.LevelMap.GetPixels32();
		width = levelData.Width;
		height = levelData.Height;

		UIManager.Instance.FadeScreenImage(1f);

		while (UIManager.Instance.IsFading.Equals(true))
			yield return null;

		while (!isLevelCreated)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					SpawnTileAt(allLevelMapPixels[x + (y * width)], x, y, transform);
				}
			}

			isLevelCreated = true;

			yield return null;
		}

		UIManager.Instance.SetLevelUI();
		UIManager.Instance.FadeScreenImage(0f);
	}

	private IEnumerator IEmptyLevel()
	{
		UIManager.Instance.FadeScreenImage(1f);

		while (UIManager.Instance.IsFading.Equals(true))
			yield return null;

		while (transform.childCount > 0)
		{
			Transform child = transform.GetChild(0);
			child.SetParent(null);
			ObjectPoolManager.Instance.SetObjectBackToPool(child.gameObject);
		}

		isLevelCreated = false;
		UIManager.Instance.SetMenuUI();
		UIManager.Instance.FadeScreenImage(0f);

		yield return null;

	}

	#endregion COROUTINES
}
