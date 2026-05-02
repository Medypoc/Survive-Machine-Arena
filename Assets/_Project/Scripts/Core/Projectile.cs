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
        // 1. Проверяем корень объекта, с которым столкнулись
        GameObject hitRoot = collision.transform.root.gameObject;

        // 2. Если корень столкновения — это тот же самый корень владельца, игнорируем
        if (hitRoot == _owner) return;

        // 3. Ищем здоровье в иерархии того, во что попали
        Health health = collision.GetComponentInParent<Health>();
        
        if (health != null)
        {
            // Передаем урон, статус крита и ссылку на владельца (кто стрелял)
            health.TakeDamage(_damage, _isCritical, _owner); 
            Destroy(gameObject);
        }
        else if (!collision.isTrigger) 
        {
            // Если попали в объект без здоровья (например, стену)
            Destroy(gameObject);
        }
    }
}