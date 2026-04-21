using UnityEngine;

/// <summary>
/// Raycast ile bakıldığında rengi rastgele değişecek objelere eklenir.
/// Renderer gereklidir.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class ColorChangeTarget : MonoBehaviour
{
    [Tooltip("Bakılmayı bıraktığında orijinal renge dönsün mü?")]
    [SerializeField] private bool restoreOriginalOnExit = false;

    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;
    private Color _originalColor;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP/HDRP
    private static readonly int ColorId = Shader.PropertyToID("_Color");          // Built-in
    private int _activeColorId;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        // Hangi renk property'sinin shader'da bulunduğunu belirle.
        Material mat = _renderer.sharedMaterial;
        if (mat != null && mat.HasProperty(BaseColorId))
        {
            _activeColorId = BaseColorId;
            _originalColor = mat.GetColor(BaseColorId);
        }
        else
        {
            _activeColorId = ColorId;
            _originalColor = (mat != null && mat.HasProperty(ColorId)) ? mat.GetColor(ColorId) : Color.white;
        }
    }

    public void ApplyRandomColor()
    {
        Color random = new Color(Random.value, Random.value, Random.value, 1f);
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(_activeColorId, random);
        _renderer.SetPropertyBlock(_mpb);
    }

    public void RestoreIfRequested()
    {
        if (!restoreOriginalOnExit) return;
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(_activeColorId, _originalColor);
        _renderer.SetPropertyBlock(_mpb);
    }
}
