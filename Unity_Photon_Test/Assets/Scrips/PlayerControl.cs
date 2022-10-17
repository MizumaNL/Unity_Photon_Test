using Photon.Pun;
using TMPro;
using UnityEngine;
using Cinemachine;


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
    private string parwalk ="開關走路";
    private bool isGround;
    private Transform childCanvas;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("老鬼"))
        {
            //連線伺服器.刪除(碰到的物件)
            PhotonNetwork.Destroy(collision.gameObject);
        }
        
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
        if(isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(new Vector2(0, jump));
        }
    }
}
