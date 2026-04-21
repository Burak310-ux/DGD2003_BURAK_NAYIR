using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oyuncunun önündeki Pickupable objeleri alıp tutmasını ve bırakmasını/fırlatmasını sağlar.
/// E: al/bırak, Sol Tık: fırlat (elinde varsa).
/// </summary>
public class ObjectHolder : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Raycast'in atılacağı kamera. Boşsa Camera.main kullanılır.")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("Obje elde tutulurken konumlanacağı nokta. Genelde kameranın child'ı olur.")]
    [SerializeField] private Transform holdPoint;

    [Header("Pickup Ayarları")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask pickupMask = ~0;

    [Header("Hold Davranışı")]
    [Tooltip("Objenin hold point'e ne kadar hızlı çekileceği (daha yüksek = daha sert).")]
    [SerializeField] private float followSpeed = 15f;

    [Tooltip("Obje ile oyuncu arasındaki mesafe bu eşiği aşarsa otomatik düşer.")]
    [SerializeField] private float breakDistance = 2.5f;

    [Tooltip("Tutulurken obje kameranın rotasyonuna uysun mu?")]
    [SerializeField] private bool matchCameraRotation = false;

    private Pickupable _heldObject;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (holdPoint == null && cameraTransform != null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(cameraTransform, false);
            hp.transform.localPosition = new Vector3(0f, -0.2f, 1.2f);
            holdPoint = hp.transform;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (_heldObject == null) TryPickup();
            else Drop();
        }

        if (_heldObject != null &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Throw();
        }
    }

    private void FixedUpdate()
    {
        if (_heldObject == null || holdPoint == null) return;

        Rigidbody rb = _heldObject.Rigidbody;
        Vector3 toTarget = holdPoint.position - rb.position;

        if (toTarget.magnitude > breakDistance)
        {
            Drop();
            return;
        }

        rb.linearVelocity = toTarget * followSpeed;

        if (matchCameraRotation && cameraTransform != null)
        {
            Quaternion targetRot = cameraTransform.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * followSpeed));
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void TryPickup()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask, QueryTriggerInteraction.Ignore))
        {
            Pickupable p = hit.collider.GetComponentInParent<Pickupable>();
            if (p != null)
            {
                _heldObject = p;
                _heldObject.OnPickedUp();
            }
        }
    }

    private void Drop()
    {
        if (_heldObject == null) return;
        _heldObject.OnDropped();
        _heldObject = null;
    }

    private void Throw()
    {
        if (_heldObject == null || cameraTransform == null) return;
        Pickupable thrown = _heldObject;
        float force = thrown.ThrowForce;
        Drop();
        thrown.Rigidbody.AddForce(cameraTransform.forward * force, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(cameraTransform.position,
                        cameraTransform.position + cameraTransform.forward * pickupRange);

        if (holdPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(holdPoint.position, 0.1f);
        }
    }
}
