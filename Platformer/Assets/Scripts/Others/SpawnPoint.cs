using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private float spawnDelay = 2f;

    private void OnEnable()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        StartCoroutine(ISpawnPlayer()); 
    }

    private IEnumerator ISpawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);

        GameObject newPlayerGameObject = ObjectPoolManager.Instance.GetObjectFromPool("Player");
        newPlayerGameObject.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        newPlayerGameObject.transform.SetParent(LevelManager.Instance.transform);

        CameraEngine.Instance.SearchTarget();
    }  
}
