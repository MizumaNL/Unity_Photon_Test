using Photon.Pun;
using TMPro;
using UnityEngine;
using Cinemachine;


/// <summary>
/// ���a�򥻱��
/// </summary>
public class PlayerControl : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("���ʳt��"), Range(0, 10)]
    private float speed = 3.5f;
    [Header("�ˬd�a�O���")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private Vector3 groundSize;
    [SerializeField, Header("���D����"), Range(0, 1000)]
    private float jump = 300f;

    private Rigidbody2D rig;
    private Animator ani;
    private string parwalk ="�}������";
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

        //�p�G���O�ۤv������ ��������
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
        if (collision.name.Contains("�Ѱ�"))
        {
            //�s�u���A��.�R��(�I�쪺����)
            PhotonNetwork.Destroy(collision.gameObject);
        }
        
    }

    [PunRPC]
    private void RPCUpdateName()
    {
        transform.Find("�e�����a�W��/���a�W��").GetComponent<TextMeshProUGUI>().text = photonView.Owner.NickName;
    }

    private void Move()
    {

        //A <- -1
        //D ->  1
        //�S��  0
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
