using UnityEngine;

/// <summary>
/// Bu component'a sahip objeler oyuncu tarafından alınabilir.
/// Rigidbody gereklidir (fizik kontrolü için).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    [Tooltip("Bu obje elde tutulurken kütlesi geçici olarak bu değere ayarlanır.")]
    [SerializeField] private float heldMass = 1f;

    [Tooltip("Fırlatıldığında uygulanacak kuvvet.")]
    [SerializeField] private float throwForce = 8f;

    private Rigidbody _rb;
    private float _originalMass;
    private bool _originalUseGravity;
    private CollisionDetectionMode _originalCollisionMode;
    private RigidbodyInterpolation _originalInterpolation;

    public Rigidbody Rigidbody => _rb;
    public float ThrowForce => throwForce;
    public bool IsHeld { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _originalMass = _rb.mass;
        _originalUseGravity = _rb.useGravity;
        _originalCollisionMode = _rb.collisionDetectionMode;
        _originalInterpolation = _rb.interpolation;
    }

    public void OnPickedUp()
    {
        IsHeld = true;
        _rb.useGravity = false;
        _rb.mass = heldMass;
        _rb.linearDamping = 10f;
        _rb.angularDamping = 10f;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void OnDropped()
    {
        IsHeld = false;
        _rb.useGravity = _originalUseGravity;
        _rb.mass = _originalMass;
        _rb.linearDamping = 0f;
        _rb.angularDamping = 0.05f;
        _rb.interpolation = _originalInterpolation;
        _rb.collisionDetectionMode = _originalCollisionMode;
    }
}
