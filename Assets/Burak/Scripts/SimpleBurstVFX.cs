using UnityEngine;

/// <summary>
/// Basit, kod tabanlı partikül patlaması.
/// Boş bir GameObject'e ekle -> prefab yap -> istediğin zaman Instantiate et.
/// Oynatma bitince kendini yok eder.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class SimpleBurstVFX : MonoBehaviour
{
    [Header("Görsel")]
    [SerializeField] private int particleCount = 30;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float startSpeed = 6f;
    [SerializeField] private float startSizeMin = 0.1f;
    [SerializeField] private float startSizeMax = 0.3f;

    [Header("Renk")]
    [SerializeField] private Color colorStart = new Color(1f, 0.9f, 0.3f, 1f);
    [SerializeField] private Color colorEnd = new Color(1f, 0.3f, 0.1f, 0f);

    [Header("Davranış")]
    [Tooltip("Yer çekimi etkisi (0 = havada kalır, 1 = aşağı düşer).")]
    [SerializeField] private float gravity = 0.5f;
    [SerializeField] private bool autoDestroy = true;

    private void Reset()
    {
        // Component ilk eklendiğinde ParticleSystem'i temiz bir şekilde ayarla.
        ConfigureParticleSystem();
    }

    private void Awake()
    {
        ConfigureParticleSystem();
    }

    private void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Play();

        if (autoDestroy)
            Destroy(gameObject, lifetime + 0.5f);
    }

    private void ConfigureParticleSystem()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = 0.1f;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = startSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(startSizeMin, startSizeMax);
        main.startColor = colorStart;
        main.gravityModifier = gravity;
        main.playOnAwake = false;
        main.stopAction = ParticleSystemStopAction.None;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, particleCount)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(colorStart, 0f), new GradientColorKey(colorEnd, 1f) },
            new[] { new GradientAlphaKey(colorStart.a, 0f), new GradientAlphaKey(colorEnd.a, 1f) }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        );
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = false;

        // Varsayılan materyali atama denemesi (Built-in / URP uyumlu particle mat).
        ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
        if (psr != null && psr.sharedMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                            ?? Shader.Find("Particles/Standard Unlit")
                            ?? Shader.Find("Sprites/Default");
            if (shader != null)
            {
                Material mat = new Material(shader);
                psr.sharedMaterial = mat;
            }
        }
    }
}
