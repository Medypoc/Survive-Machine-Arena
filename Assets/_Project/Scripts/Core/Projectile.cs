using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector3 _startPosition;
    private float _damage;
    private float _range;
    private bool _isCritical;
    private bool _isLaunched;
    private GameObject _owner;

    public void Launch(float damage, float speed, float range, GameObject owner, bool isCritical)
    {
        _damage = damage;
        _range = range;
        _owner = owner;
        _isCritical = isCritical;
        _startPosition = transform.position;
        _isLaunched = true;

        if (GetComponent<Rigidbody2D>() != null) 
            GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
    }

    void Update()
    {
        if (_isLaunched && Vector3.Distance(_startPosition, transform.position) >= _range) 
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _owner) return;

        // Ищем Health на самом объекте или на любом из его родителей
        Health health = collision.GetComponentInParent<Health>(); 
        
        if (health != null)
        {
            Debug.Log($"Попадание в {collision.name}! Урон нанесен.");
            health.TakeDamage(_damage, _isCritical, _owner);
            Destroy(gameObject);
        }
        else if (!collision.isTrigger) 
        {
            // Если это не триггер и у него нет здоровья (например, стена) — пуля исчезает
            Destroy(gameObject);
        }
    }
}