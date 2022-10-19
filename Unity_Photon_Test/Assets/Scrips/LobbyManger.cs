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
    #region 資料大廳
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
    #region 資料房間
    private TextMeshProUGUI textRoomName;
    private TextMeshProUGUI textRoomPlayers;
    private CanvasGroup groupRoom;

    private Button btnStartGame;
    private Button btnExitGame;
    #endregion

    private void Awake()
    {
        GetLobbyObkectSetting();

        textRoomName = GameObject.Find("房間名稱").GetComponent<TextMeshProUGUI>();
        textRoomPlayers = GameObject.Find("房間人數").GetComponent<TextMeshProUGUI>();
        groupRoom = GameObject.Find("RoomCanv").GetComponent<CanvasGroup>();

        btnStartGame = GameObject.Find("按鈕開始遊戲").GetComponent<Button>();
        btnExitGame = GameObject.Find("按鈕離開遊戲").GetComponent<Button>();

        btnExitGame.onClick.AddListener(ExitGame);
        //photoView 遠端同步客戶端("RPC 方法",針對哪些玩家);
        btnStartGame.onClick.AddListener(() => photonView.RPC("RPCStartGame", RpcTarget.All));

        PhotonNetwork.ConnectUsingSettings();
    }
    //遠端同步客戶端方法
    [PunRPC]
    private void RPCStartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("遊戲場景");
    }

    /// <summary>
    /// 取得大廳物件與事件
    /// </summary>
    private void GetLobbyObkectSetting()
    {
        inputFieldPlayerName = GameObject.Find("輸入PlayerName").GetComponent<TMP_InputField>();
        inputFieldCreateRoomName = GameObject.Find("輸入RoomName").GetComponent<TMP_InputField>();
        inputFieldJoinRoomName = GameObject.Find("輸入搜尋房間").GetComponent<TMP_InputField>();

        btnCreatRoom = GameObject.Find("按鈕NewRoom").GetComponent<Button>();
        btnJoinRoom = GameObject.Find("按鈕指定房間").GetComponent<Button>();
        btnJoinRandomRoom = GameObject.Find("按鈕隨機房").GetComponent<Button>();

        groupMain = GameObject.Find("MainCanv").GetComponent<CanvasGroup>();

        //結束編輯 : 按下Enter 或在空白處點一下
        //輸入欄位,結束編輯,添加監聽((輸入欄位的輸入字串) => 儲存)
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
    /// 連線至主機
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        groupMain.interactable = true;
        groupMain.blocksRaycasts = true;

        print("<color=red>已連線至主機</color>");
    }

    /// <summary>
    /// 創造房間
    /// </summary>
    private void CreateRoom()
    {
        //房間選項設置
        RoomOptions ro = new RoomOptions();
        //最大人數&房間可視性
        ro.MaxPlayers = 20;
        ro.IsVisible = true;
        //Photon連線.創建房間(房間名稱,房間選項)
        PhotonNetwork.CreateRoom(nameCreateRoom, ro);
    }

    /// <summary>
    /// 加入房間
    /// </summary>
    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(nameJoinRoom);
    }

    /// <summary>
    /// 加入隨機房間
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
        print("<color=green>房間創立成功</color>");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("<color=green>加入房間成功</color>");

        groupRoom.alpha = 1;
        groupRoom.interactable = true;
        groupRoom.blocksRaycasts = true;

        textRoomName.text = "房間名稱 :" + PhotonNetwork.CurrentRoom.Name;
        textRoomPlayers.text = $"房間人數 : + { PhotonNetwork.CurrentRoom.PlayerCount } / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        textRoomPlayers.text = $"房間人數 : + { PhotonNetwork.CurrentRoom.PlayerCount } / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        textRoomPlayers.text = $"房間人數 : + { PhotonNetwork.CurrentRoom.PlayerCount } / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
}
