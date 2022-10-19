
using Photon.Pun;
using UnityEngine;

public class SpawnGhost : MonoBehaviour
{
    [SerializeField, Header("�Ѱ�")]
    private GameObject prefabGhost;
    [SerializeField, Header("�ͦ��W�v"), Range(0, 5)]
    private float intercalSpawn = 1.5f;
    [SerializeField, Header("�ͦ��I")]
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
