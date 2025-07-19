using UnityEngine;
using System.Collections; // Necessário para Coroutines

public class PlayerController : MonoBehaviour
{
  [Header("Movimentação")]
  [SerializeField] private float moveSpeed = 5f;
  [SerializeField] private float jumpForce = 10f;

  [Header("Verificação de Chão")]
  [SerializeField] private Transform groundCheck;
  [SerializeField] private LayerMask groundLayer;
  [SerializeField] private float groundCheckRadius = 0.2f;

  [Header("Impulso Aéreo Rotativo")] // Nova seção de configurações
  [SerializeField] private float airDashForce = 15f; // Força do impulso aéreo
  [SerializeField] private float airDashDuration = 0.2f; // Duração do impulso
  [SerializeField] private float airDashRotationSpeed = 720f; // Velocidade de rotação em graus por segundo
  [SerializeField] private float airDashCooldown = 1.0f; // Tempo de recarga do impulso

  [Header("Referências")]
  private Rigidbody2D rb;
  private Vector2 movement;
  private SpriteRenderer spriteRenderer;

  [Header("Animações")]
  [SerializeField] private Animator animator;

  private Camera mainCamera;
  private bool isGrounded;
  private float nextAirDashTime = 0f; // Tempo que poderá usar o dash novamente
  private bool isAirDashing = false; // Flag para indicar se o player está em um dash
  private Vector2 airDashDirection; // Direção do dash

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
    CheckGroundStatus();
    HandleJumpInput();
    HandleAirDashInput(); // Novo método para o input do dash
    UpdateAnimationState();
  }

  private void FixedUpdate()
  {
    Move();
    ApplyAirDashForce(); // Novo método para aplicar a força do dash
    ApplyAirDashRotation(); // Novo método para aplicar a rotação
  }

  private void HandleInput()
  {
    float inputX = Input.GetAxisRaw("Horizontal");
    movement = new Vector2(inputX, 0f).normalized;
  }

  private void HandleJumpInput()
  {
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
      Jump();
    }
  }

  private void Jump()
  {
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
  }

  private void CheckGroundStatus()
  {
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    // Resetar o cooldown do dash aéreo quando toca o chão
    if (isGrounded)
    {
      nextAirDashTime = Time.time; // Reseta o cooldown imediatamente ao tocar o chão
      if (isAirDashing) // Garante que o dash seja interrompido se tocar o chão
      {
        StopCoroutine(AirDashCoroutine()); // Para a corrotina se estiver ativa
        isAirDashing = false;
        rb.angularVelocity = 0f; // Para o giro
        transform.rotation = Quaternion.identity; // Reseta a rotação visual
      }
    }
  }

  private void HandleAirDashInput()
  {
    // Se a tecla de dash (ex: Shift Esquerdo) for pressionada,
    // NÃO estiver no chão, NÃO estiver já em dash, e o cooldown permitindo
    if (Input.GetKeyDown(KeyCode.LeftShift) && !isGrounded && !isAirDashing && Time.time >= nextAirDashTime)
    {
      // Determina a direção do dash. Pode ser a direção do mouse, ou a direção do movimento atual.
      // Para simplicidade, vamos usar a direção do mouse para um dash de "esquiva/ataque".
      Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
      mouseWorldPos.z = 0f;
      airDashDirection = (mouseWorldPos - transform.position).normalized;

      StartCoroutine(AirDashCoroutine());
    }
  }

  private IEnumerator AirDashCoroutine()
  {
    isAirDashing = true;
    nextAirDashTime = Time.time + airDashCooldown; // Inicia o cooldown

    float originalGravityScale = rb.gravityScale;
    rb.gravityScale = 0f; // Remove a gravidade durante o dash para um movimento mais "flutuante"

    rb.linearVelocity = Vector2.zero; // Zera a velocidade para aplicar o impulso de forma limpa

    // Aplica o impulso inicial
    rb.AddForce(airDashDirection * airDashForce, ForceMode2D.Impulse);

    // Começa a rotação
    rb.angularVelocity = (airDashDirection.x > 0 ? -1 : 1) * airDashRotationSpeed; // Gira na direção oposta ao dash ou no sentido do dash. Ajuste conforme preferir.

    yield return new WaitForSeconds(airDashDuration);

    // Finaliza o dash
    isAirDashing = false;
    rb.gravityScale = originalGravityScale; // Restaura a gravidade
    rb.angularVelocity = 0f; // Para o giro

    // Opcional: Se quiser um reset visual imediato da rotação após o dash
    transform.rotation = Quaternion.identity;
  }

  private void ApplyAirDashForce()
  {
    // Nenhuma lógica contínua aqui, pois o impulso é aplicado via AddForce(ForceMode2D.Impulse)
    // no início da corrotina. A corrotina já gerencia a duração.
    // Se você quisesse um dash com aceleração gradual, usaria Forca (ForceMode2D.Force) aqui
    // e ajustaria a velocidade no FixedUpdate. Mas para um impulso, o atual é bom.
  }

  private void ApplyAirDashRotation()
  {
    // A rotação já está sendo gerenciada pelo rb.angularVelocity na corrotina.
    // Se você não usasse angularVelocity, aplicaria transform.Rotate aqui.
    // rb.angularVelocity é geralmente melhor para física.
  }

  private void Move()
  {
    // Apenas aplica movimento se não estiver em um dash aéreo
    if (!isAirDashing)
    {
      rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }
  }

  private void UpdateAnimationState()
  {
    // Se estiver em dash, pode ter uma animação/feedback diferente
    if (isAirDashing)
    {
      // Opcional: Ativar animação de dash/giro
      animator.speed = 1f; // Ou uma velocidade específica para o dash
                           // animator.SetBool("IsAirDashing", true);
    }
    else if (Mathf.Abs(rb.linearVelocity.x) > 0.01f && isGrounded)
    {
      animator.speed = 1f;
      // animator.SetBool("IsWalking", true);
      // animator.SetBool("IsJumping", false);
    }
    else if (isGrounded)
    {
      animator.speed = 0.2f;
      // animator.SetBool("IsWalking", false);
      // animator.SetBool("IsJumping", false);
    }
    else // Não está no chão e não está em dash
    {
      animator.speed = 1f;
      // animator.SetBool("IsJumping", true);
      // animator.SetBool("IsWalking", false);
    }
  }

  private void HandleFlipByMouse()
  {
    if (isAirDashing) return; // Não flipa durante o dash para não atrapalhar o giro

    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    if (mouseWorldPos.x < transform.position.x)
    {
      transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    else
    {
      transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
  }

  private void OnDrawGizmos()
  {
    if (groundCheck == null) return;
    Gizmos.color = isGrounded ? Color.green : Color.red;
    Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
  }
}