using UnityEngine;

/// <summary>
/// Kameradan ileri doğru raycast atar.
/// Işın bir ColorChangeTarget'a değdiği sürece objenin rengi her frame rastgele değişir.
/// </summary>
public class ColorRaycaster : MonoBehaviour
{
    [Header("Referans")]
    [Tooltip("Raycast'in başlayacağı transform. Boşsa Camera.main kullanılır.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Ray Ayarları")]
    [SerializeField] private float rayDistance = 20f;
    [SerializeField] private LayerMask rayMask = ~0;

    [Header("Görsel (Debug)")]
    [SerializeField] private bool drawDebugRay = true;

    private ColorChangeTarget _currentTarget;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (drawDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        ColorChangeTarget newTarget = null;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, rayMask, QueryTriggerInteraction.Ignore))
        {
            newTarget = hit.collider.GetComponentInParent<ColorChangeTarget>();
        }

        // Hedef değiştiyse eskisini restore et.
        if (newTarget != _currentTarget)
        {
            if (_currentTarget != null)
                _currentTarget.RestoreIfRequested();

            _currentTarget = newTarget;
        }

        // Bakılan hedefin rengini her frame güncelle.
        if (_currentTarget != null)
            _currentTarget.ApplyRandomColor();
    }
}
