using UnityEngine;

namespace bullet.fx.pack
{
    public sealed class Gun : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 500f;
        [SerializeField] private float gravityForce;

        [SerializeField] private float gunAlertRadius = 20f;
        public enum ShooterTeam { Player, Police }

        public void Shoot(Vector3 targetPos)
        {
            Shoot(targetPos, ShooterTeam.Player);
        }

        public void Shoot(Vector3 targetPos, ShooterTeam team)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(targetPos - firePoint.position) * Quaternion.Euler(90, 0, 0)
            );

            
            int layer = LayerMask.NameToLayer(team == ShooterTeam.Player ? "PlayerBullet" : "PoliceBullet");
            bullet.layer = layer;

            
            foreach (Transform t in bullet.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Vector3 dir = (targetPos - firePoint.position).normalized;
            rb.velocity = dir * bulletSpeed;
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);

            AlertNearbyNPCs();
        }


        void AlertNearbyNPCs()
        {
            Collider[] cols = Physics.OverlapSphere(firePoint.position, gunAlertRadius);

            foreach (var c in cols)
            {
                NPCAI ai = c.GetComponentInParent<NPCAI>();
                if (ai != null)
                {
                    ai.OnGunShot();  
                }
            }
        }


    }
}
