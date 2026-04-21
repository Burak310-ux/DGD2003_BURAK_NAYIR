using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    [Tooltip("Geçiş yapılacak Cinemachine kameraları sırayla ekle.")]
    public List<CinemachineCamera> cameras;

    [Tooltip("Kaç saniyede bir otomatik geçiş yapılacak?")]
    [SerializeField] private float switchInterval = 2f;

    [Tooltip("Başlangıçta otomatik geçiş çalışsın mı?")]
    [SerializeField] private bool autoSwitch = true;

    [Tooltip("C tuşu ile manuel geçiş de yapılabilsin mi?")]
    [SerializeField] private bool allowManualSwitch = true;

    private int currentIndex = 0;
    private float _timer;

    private void Start()
    {
        if (cameras == null || cameras.Count == 0) return;

        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] != null)
                cameras[i].Priority = (i == currentIndex) ? 20 : 10;
        }

        _timer = switchInterval;
    }

    void Update()
    {
        if (cameras == null || cameras.Count < 2) return;

        if (autoSwitch)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                SwitchCamera();
                _timer = switchInterval;
            }
        }

        if (allowManualSwitch &&
            Keyboard.current != null &&
            Keyboard.current.cKey.wasPressedThisFrame)
        {
            SwitchCamera();
            _timer = switchInterval;
        }
    }

    void SwitchCamera()
    {
        cameras[currentIndex].Priority = 10;
        currentIndex = (currentIndex + 1) % cameras.Count;
        cameras[currentIndex].Priority = 20;
    }
}
