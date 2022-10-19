
using Photon.Pun;
using UnityEngine;

public class SpawnGhost : MonoBehaviour
{
    [SerializeField, Header("┭강")]
    private GameObject prefabGhost;
    [SerializeField, Header("Ν┬픚쾤"), Range(0, 5)]
    private float intercalSpawn = 1.5f;
    [SerializeField, Header("Ν┬헕")]
    private Transform[] spawnPoints;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("Spawn", 0, intercalSpawn);
        }       
    }

    private void Spawn()
    {
        int random = Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(prefabGhost.name, spawnPoints[random].position, Quaternion.identity);
    }
}
