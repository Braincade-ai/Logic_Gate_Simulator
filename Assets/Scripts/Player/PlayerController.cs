using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float increaseSpeed = 1.0f; public float limitValue = 10.0f; public float _normalVelocity = 1.0f;

    private float originalValue = 0.0f;
    private float currentValue = 0.0f;
    private bool isPointerDown = false;
    private Rigidbody2D _playerRb;

    void Start()
    {
        originalValue = _playerRb.velocity.y;
        currentValue = originalValue;
    }

    void Update()
    {
        if (isPointerDown)
        {
            currentValue = Mathf.MoveTowards(currentValue, limitValue, increaseSpeed * Time.deltaTime);
            _playerRb.velocity = new Vector2(_normalVelocity, currentValue);
        }
        else
        {
            _playerRb.velocity = new Vector2(_normalVelocity, 0.0f);
        }
    }

    void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        currentValue = originalValue;
    }

    void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        currentValue = 0.0f;
    }

    private void OnEnable()
    {
        _playerRb = GetComponent<Rigidbody2D>();
    }

}