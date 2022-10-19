using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lobby Manger
/// </summary>
public class LobbyManger : MonoBehaviourPunCallbacks
{
    #region ��Ƥj�U
    private TMP_InputField inputFieldPlayerName;
    private TMP_InputField inputFieldCreateRoomName;
    private TMP_InputField inputFieldJoinRoomName;

    private Button btnCreatRoom;
    private Button btnJoinRoom;
    private Button btnJoinRandomRoom;

    private string namePlayer;
    private string nameCreateRoom;
    private string nameJoinRoom;

    private CanvasGroup groupMain;


    #endregion
    #region ��Ʃж�
    private TextMeshProUGUI textRoomName;
    private TextMeshProUGUI textRoomPlayers;
    private CanvasGroup groupRoom;

    private Button btnStartGame;
    private Button btnExitGame;
    #endregion

    private void Awake()
    {
        GetLobbyObkectSetting();

        textRoomName = GameObject.Find("�ж��W��").GetComponent<TextMeshProUGUI>();
        textRoomPlayers = GameObject.Find("�ж��H��").GetComponent<TextMeshProUGUI>();
        groupRoom = GameObject.Find("RoomCanv").GetComponent<CanvasGroup>();

        btnStartGame = GameObject.Find("���s�}�l�C��").GetComponent<Button>();
        btnExitGame = GameObject.Find("���s���}�C��").GetComponent<Button>();

        btnExitGame.onClick.AddListener(ExitGame);
        //photoView ���ݦP�B�Ȥ��("RPC ��k",�w����Ǫ��a);
        btnStartGame.onClick.AddListener(() => photonView.RPC("RPCStartGame", RpcTarget.All));

        PhotonNetwork.ConnectUsingSettings();
    }
    //���ݦP�B�Ȥ�ݤ�k
    [PunRPC]
    private void RPCStartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("�C������");
    }

    /// <summary>
    /// ���o�j�U����P�ƥ�
    /// </summary>
    private void GetLobbyObkectSetting()
    {
        inputFieldPlayerName = GameObject.Find("��JPlayerName").GetComponent<TMP_InputField>();
        inputFieldCreateRoomName = GameObject.Find("��JRoomName").GetComponent<TMP_InputField>();
        inputFieldJoinRoomName = GameObject.Find("��J�j�M�ж�").GetComponent<TMP_InputField>();

        btnCreatRoom = GameObject.Find("���sNewRoom").GetComponent<Button>();
        btnJoinRoom = GameObject.Find("���s���w�ж�").GetComponent<Button>();
        btnJoinRandomRoom = GameObject.Find("���s�H����").GetComponent<Button>();

        groupMain = GameObject.Find("MainCanv").GetComponent<CanvasGroup>();

        //�����s�� : ���UEnter �Φb�ťճB�I�@�U
        //��J���,�����s��,�K�[��ť((��J��쪺��J�r��) => �x�s)
        inputFieldPlayerName.onEndEdit.AddListener((input) =>
        {
            namePlayer = input;
            PhotonNetwork.NickName = namePlayer;
         });
        inputFieldCreateRoomName.onEndEdit.AddListener((input) => nameCreateRoom = input);
        inputFieldJoinRoomName.onEndEdit.AddListener((input) => nameJoinRoom = input);

        btnCreatRoom.onClick.AddListener(CreateRoom);
        btnJoinRoom.onClick.AddListener(JoinRoom);
        btnJoinRandomRoom.onClick.AddListener(JoinRandomRoom);
    }

    /// <summary>
    /// �s�u�ܥD��
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        groupMain.interactable = true;
        groupMain.blocksRaycasts = true;

        print("<color=red>�w�s�u�ܥD��</color>");
    }

    /// <summary>
    /// �гy�ж�
    /// </summary>
    private void CreateRoom()
    {
        //�ж��ﶵ�]�m
        RoomOptions ro = new RoomOptions();
        //�̤j�H��&�ж��i����
        ro.MaxPlayers = 20;
        ro.IsVisible = true;
        //Photon�s�u.�Ыةж�(�ж��W��,�ж��ﶵ)
        PhotonNetwork.CreateRoom(nameCreateRoom, ro);
    }

    /// <summary>
    /// �[�J�ж�
    /// </summary>
    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(nameJoinRoom);
    }

    /// <summary>
    /// �[�J�H���ж�
    /// </summary>
    private void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private void ExitGame()
    {
        PhotonNetwork.LeaveRoom();

        groupRoom.alpha = 0;
        groupRoom.interactable = false;
        groupRoom.blocksRaycasts = false;

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("<color=green>�ж��Хߦ��\</color>");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("<color=green>�[�J�ж����\</color>");

        groupRoom.alpha = 1;
        groupRoom.interactable = true;
        groupRoom.blocksRaycasts = true;

        textRoomName.text = "�ж��W�� :" + PhotonNetwork.CurrentRoom.Name;
        textRoomPlayers.text = $"�ж��H�� : + { PhotonNetwork.CurrentRoom.PlayerCount } / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        textRoomPlayers.text = $"�ж��H�� : + { PhotonNetwork.CurrentRoom.PlayerCount } / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        textRoomPlayers.text = $"�ж��H�� : + { PhotonNetwork.CurrentRoom.PlayerCount } / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
}
