using UnityEngine;

public class BossDestruction : MonoBehaviour
{
    public float destructionRadius = 5f;
    public LayerMask destructionLayer;
    void Start()
    {
        DestroyNearbyStructures();
    }

    void Update()
    {
        DestroyNearbyStructures();
    }

    void DestroyNearbyStructures()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, destructionRadius, destructionLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Structure"))
            {
                Destroy(col.gameObject);
                Debug.Log("Structura destruida por aparicion del Boss!");
            }
        }
    }
    
    void DestroyStructuresNearBoss()
{
    Collider[] nearbyStructures = Physics.OverlapSphere(transform.position, destructionRadius, destructionLayer);

    foreach (Collider col in nearbyStructures)
    {
        if (col.CompareTag("Structure"))
        {
            Destroy(col.gameObject);
            Debug.Log("Estructura destruida por el jefe al caminar.");
        }
    }
}
}
