using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage;
    private bool _isCritical;
    private GameObject _owner;
    private bool _hasHit = false; // Предохранитель от двойного урона

    public void Launch(float damage, float speed, float range, GameObject owner, bool isCritical)
    {
        _damage = damage;
        _isCritical = isCritical;
        _owner = owner;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        
        Destroy(gameObject, range / speed);
    }

    // Срабатывает, когда пуля ВЛЕТАЕТ в коллайдер
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessHit(collision);
    }

    // Срабатывает, если пуля ЗАСПАВНИЛАСЬ ВНУТРИ коллайдера
    private void OnTriggerStay2D(Collider2D collision)
    {
        ProcessHit(collision);
    }

    // Вся ваша логика урона теперь здесь
    private void ProcessHit(Collider2D collision)
    {
        // Защита от того, чтобы пуля не нанесла урон дважды за одну миллисекунду
        if (_hasHit) return; 

        if (collision.transform.root.gameObject == _owner) return;

        VehiclePartHitbox hitbox = collision.GetComponent<VehiclePartHitbox>();

        if (hitbox != null)
        {
            hitbox.TakeHit(_damage, _isCritical, _owner);
            _hasHit = true; 
        }
        else
        {
            Health rootHealth = collision.GetComponentInParent<Health>();
            if (rootHealth != null)
            {
                rootHealth.TakeDamage(_damage, _isCritical, _owner);
                _hasHit = true; 
            }
        }

        // Если мы попали хоть по кому-то (кроме себя) - пуля уничтожается
        if (_hasHit)
        {
            Destroy(gameObject);
        }
    }
}