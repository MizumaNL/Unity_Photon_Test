using Photon.Pun;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

/// <summary>
/// 玩家基本控制器
/// </summary>
public class PlayerControl : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("移動速度"), Range(0, 10)]
    private float speed = 3.5f;
    [Header("檢查地板資料")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private Vector3 groundSize;
    [SerializeField, Header("跳躍高度"), Range(0, 1000)]
    private float jump = 300f;

    private Rigidbody2D rig;
    private Animator ani;
    private string parwalk = "開關走路";
    private bool isGround;
    private Transform childCanvas;
    private TextMeshProUGUI textGhost;
    private int countGhost;
    private int countGhostMax = 10;
    private CanvasGroup groupGmae;
    private TextMeshProUGUI textWinner;
    private Button btnBackToLobby;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0.2f, 0.35f);
        Gizmos.DrawCube(transform.position + groundOffset, groundSize);
    }

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        childCanvas = transform.GetChild(0);

        //如果不是自己的物件 關閉元件
        if (!photonView.IsMine) enabled = false;

        photonView.RPC("RPCUpdateName", RpcTarget.All);

        textGhost = transform.Find("畫布玩家名稱/殺鬼數量").GetComponent<TextMeshProUGUI>();
        groupGmae = GameObject.Find("畫布遊戲介面").GetComponent<CanvasGroup>();
        textWinner = GameObject.Find("勝利者").GetComponent<TextMeshProUGUI>();
        btnBackToLobby = GameObject.Find("返回大廳按鈕").GetComponent<Button>();

        btnBackToLobby.onClick.AddListener(() =>
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel("遊戲大廳");
            }
        });

    }

    private void Start()
    {
        GameObject.Find("CM").GetComponent<CinemachineVirtualCamera>().Follow = transform;
    }

    private void Update()
    {
        Move();
        Jump();
        CheckGround();
        BackToTop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("老鬼"))
        {
            //連線伺服器.刪除(碰到的物件)
            Destroy(collision.gameObject);

            textGhost.text = (++countGhost).ToString();
            if (countGhost >= countGhostMax) Win();
        }

    }
    
    private void BackToTop()
    {
        if (transform.position.y < -10)
        {
            rig.velocity = Vector3.zero;
            transform.position = new Vector3(0.09f, 3.5f, 0);            
            textGhost.text = (--countGhost).ToString();
        }
    }
    /// <summary>
    /// 獲勝
    /// </summary>
    private void Win()
    {
        groupGmae.alpha = 1;
        groupGmae.interactable = true;
        groupGmae.blocksRaycasts = true;

        textWinner.text = "Winner :  " + photonView.Owner.NickName;
        DistroyObject();
    }
    /// <summary>
    /// 刪除物件
    /// </summary>
    private void DistroyObject()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("老鬼");
        for (int i = 0; i < ghosts.Length; i++) Destroy(ghosts[i]);

        Destroy(FindObjectOfType<SpawnGhost>().gameObject);
    }

    [PunRPC]
    private void RPCUpdateName()
    {
        transform.Find("畫布玩家名稱/玩家名稱").GetComponent<TextMeshProUGUI>().text = photonView.Owner.NickName;
    }

    private void Move()
    {

        //A <- -1
        //D ->  1
        //沒按  0
        float h = Input.GetAxis("Horizontal");
        rig.velocity = new Vector2(speed * h, rig.velocity.y);
        ani.SetBool(parwalk, h != 0);

        if (Mathf.Abs(h) < 0) return;
        transform.eulerAngles = new Vector3(0, h > 0 ? 180 : 0, 0);
        childCanvas.localEulerAngles = new Vector3(0, h > 0 ? 180 : 0, 0);


    }
    private void CheckGround()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position + groundOffset, groundSize, 0);
        //print(hit.name);
        isGround = hit;
    }
    private void Jump()
    {
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(new Vector2(0, jump));
        }
    }
}
