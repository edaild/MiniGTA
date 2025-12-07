using UnityEngine;

namespace bullet.fx.pack {
    public sealed class Gun : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 500f;
        [SerializeField] private float gravityForce;


        public void Shoot(Vector3 forward) {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(forward - firePoint.position) * Quaternion.Euler(90, 0, 0));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            Vector3 direction = (forward - firePoint.position).normalized;
            rb.velocity = direction * bulletSpeed;
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
        }
    }
}