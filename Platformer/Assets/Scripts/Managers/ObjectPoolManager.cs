using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singelton<ObjectPoolManager>
{
    private Dictionary<string, Stack<GameObject>> poolDictionary = new Dictionary<string, Stack<GameObject>>();

    private GameObject[] prefabModels;

    private void Awake()
    {
        prefabModels = Resources.LoadAll<GameObject>("LevelPrefabs/") as GameObject[];
    }

    public GameObject GetObjectFromPool(string prefabName)
    {
        Stack<GameObject> pool;

        if(poolDictionary.TryGetValue(prefabName, out pool))
        {
            if(pool.Count > 0)
            {
                GameObject instance = pool.Pop();
                instance.SetActive(true);
                return instance;
            }
            else
            {
                // Create a new prefab instance and stack.
                return CreatePrefabInstance(prefabName);             
            }
        }
        // We did not have stack with wanted prefab instances.
        else
        {
            // Create a new stack
            poolDictionary.Add(prefabName, new Stack<GameObject>());

            // Create a new prefab instance
            return CreatePrefabInstance(prefabName);
        }
    }

    private GameObject CreatePrefabInstance(string prefabName)
    {
        GameObject newPrefabInstance = Instantiate(GetPrefabModel(prefabName));
        newPrefabInstance.name = prefabName;
        return newPrefabInstance;
    }

    private GameObject GetPrefabModel(string prefabModelName)
    {
        foreach (var prefabModel in prefabModels)
        {
            if(prefabModel.name == prefabModelName)
            {
                return prefabModel;
            }
        }

        return null;
    }

    public void SetObjectBackToPool(GameObject instance)
    {
        instance.SetActive(false);

        Stack<GameObject> poolToReturn;

        if (poolDictionary.TryGetValue(instance.name, out poolToReturn))
        {
            if (poolToReturn != null)
            {
                poolToReturn.Push(instance);
            }
        }

        //Debug.Log(poolDictionary.Values.Count);
    }
}
