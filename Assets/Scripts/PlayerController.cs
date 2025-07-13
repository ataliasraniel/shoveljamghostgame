using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [Header("Movimentação")]
  [SerializeField] private float moveSpeed = 5f;

  [Header("Referências")]
  private Rigidbody2D rb;
  private Vector2 movement;
  private SpriteRenderer spriteRenderer;

  public Vector2 Movement => movement; // outros scripts podem acessar

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();

  }

  private void Update()
  {
    HandleInput();
    HandleFlip();
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
  }

  private void HandleFlip()
  {
    if (movement.x != 0)
    {
      Vector3 newScale = transform.localScale;
      newScale.x = Mathf.Sign(movement.x) * Mathf.Abs(newScale.x);
      transform.localScale = newScale;
    }
  }
}
