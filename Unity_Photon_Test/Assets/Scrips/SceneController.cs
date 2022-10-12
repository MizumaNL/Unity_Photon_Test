using Photon.Pun;
using UnityEngine;


/// <summary>
/// �������: �޲z���a�i�J�᪫�󪺥ͦ�
/// </summary>
public class SceneController : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("���a���m��")]
    private GameObject prefabPlayer;

    private void Awake()
    {
        InitializePlayer();
    }

    /// <summary>
    /// ��l�ƪ��a
    /// </summary>
    private void InitializePlayer()
    {
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-5f, 5f);
        pos.y = 6f;
        PhotonNetwork.Instantiate(prefabPlayer.name, pos, Quaternion.identity);
    }
}
