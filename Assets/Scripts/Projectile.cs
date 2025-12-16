using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody _rbd;
    [SerializeField] private float _speed;

    void Awake()
    {
        _rbd = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _rbd.AddForce(transform.forward * _speed * Time.deltaTime, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }
}
