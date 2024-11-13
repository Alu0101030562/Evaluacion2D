using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private float _horizontalMovement;
    private Vector3 _direction;

    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private TMP_Text gameOverTxt;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 7.5f;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private int _scoreItems;

    private bool _isGrounded;
    private bool isGameOver;
    private GameObject _enemy;


    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsCrawl = Animator.StringToHash("isCrawl");
    private static readonly int VerticalMovement = Animator.StringToHash("verticalMovement");

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");

        // Crawl
        if (Input.GetKey(KeyCode.V))
        {
            _animator.SetBool(IsCrawl, true);
        }
        else
        {
            _animator.SetBool(IsCrawl, false);
        }

        // Movimiento horizontal
        if (_horizontalMovement != 0)
        {
            _animator.SetBool(IsWalking, true);
            _spriteRenderer.flipX = _horizontalMovement > 0;
            _direction = new Vector3(_horizontalMovement, 0, transform.position.z).normalized;
            transform.Translate(_direction * (moveSpeed * Time.deltaTime));
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Debug.Log("Salto activado"); // Para verificar que el salto se activa
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }
        if (_enemy != null && Input.GetKeyDown(KeyCode.X))
        {
            Destroy(_enemy);
            _enemy = null; // Limpia la referencia al enemigo
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            _scoreItems++;
            scoreTxt.text = $"Puntuación: {_scoreItems}";
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _enemy = collision.gameObject;
            //Destroy(collision.gameObject);// Guarda la referencia del enemigo
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tesoro"))
        {
            isGameOver = true;
            gameOverTxt.gameObject.SetActive(true); // Muestra el mensaje de fin del juego
            gameOverTxt.text = "¡Felicidades! Has encontrado el tesoro.";
            Debug.Log("Juego Terminado: Has encontrado el tesoro.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        transform.SetParent(null);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _enemy = null; // Limpia la referencia al enemigo cuando ya no colisiona
        }

    }


}