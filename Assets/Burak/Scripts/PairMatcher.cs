using UnityEngine;

/// <summary>
/// Aynı PairId'ye sahip iki obje çarpıştığında ikisi de yok edilir.
/// Farklı PairId'ye sahip objeler çarpıştığında bir şey olmaz.
/// Çalışması için objede Collider (Rigidbody çarpan taraflardan en az birinde) olmalı.
/// </summary>
[DisallowMultipleComponent]
public class PairMatcher : MonoBehaviour
{
    [Tooltip("Eşleşme kimliği. Aynı değere sahip iki obje çarpışınca ikisi de yok olur. (örn: A, B, C)")]
    [SerializeField] private string pairId = "A";

    [Tooltip("Yok olmadan önce oynatılacak efekt (opsiyonel).")]
    [SerializeField] private GameObject destroyVfx;

    [Tooltip("Yok olmadan önce çalacak ses (opsiyonel).")]
    [SerializeField] private AudioClip destroySfx;

    public string PairId => pairId;

    private bool _destroyed;

    private void OnCollisionEnter(Collision collision)
    {
        TryMatch(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryMatch(other);
    }

    private void TryMatch(Collider other)
    {
        if (_destroyed) return;

        PairMatcher otherMatcher = other.GetComponentInParent<PairMatcher>();
        if (otherMatcher == null || otherMatcher == this) return;

        if (otherMatcher.pairId != pairId) return;

        // İki taraflı tetiklenmeyi önlemek için flag.
        _destroyed = true;
        otherMatcher._destroyed = true;

        SpawnEffects(transform.position);
        SpawnEffects(otherMatcher.transform.position);

        Destroy(otherMatcher.gameObject);
        Destroy(gameObject);
    }

    private void SpawnEffects(Vector3 position)
    {
        if (destroyVfx != null)
            Instantiate(destroyVfx, position, Quaternion.identity);

        if (destroySfx != null)
            AudioSource.PlayClipAtPoint(destroySfx, position);
    }
}
