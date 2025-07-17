using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [Header("Movimentação")]
  [SerializeField] private float moveSpeed = 5f;

  [Header("Referências")]
  private Rigidbody2D rb;
  private Vector2 movement;
  private SpriteRenderer spriteRenderer;

  [Header("Animações")]
  [SerializeField] private Animator animator;

  private Camera mainCamera;

  public Vector2 Movement => movement;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
    mainCamera = Camera.main;
  }

  void Start()
  {
    animator.speed = 0.2f;
  }

  private void Update()
  {
    HandleInput();
    HandleFlipByMouse();
  }

  private void FixedUpdate()
  {
    Move();
  }

  private void HandleInput()
  {
    float inputX = Input.GetAxisRaw("Horizontal");
    movement = new Vector2(inputX, 0f).normalized;
  }

  private void Move()
  {
    rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

    if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
    {
      animator.speed = 1f;
    }
    else
    {
      animator.speed = 0.2f;
    }
  }

  private void HandleFlipByMouse()
  {
    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    if (mouseWorldPos.x < transform.position.x)
    {
      // Mouse está à esquerda
      transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    else
    {
      // Mouse está à direita
      transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
  }
}
