using System;
using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    [SerializeField] private RectTransform wheel;
    [SerializeField] private SpinDetector detector;
    [SerializeField] private float[] initialSpeeds = { 880f, 890f, 898f, 907f, 916f, 925f, 933f, 942f };
    [SerializeField, Min(1f)] private float deceleration = 180f;
    private float currentSpeed;

    public bool IsSpinning => currentSpeed > 0;
    public float InitialSpeed { get; private set; }
    public event Action<int> Completed;

    private void OnValidate()
    {
        wheel = transform.Find("ui_image_wheel_value") as RectTransform;
        detector = transform.Find("ui_spin_detector")?.GetComponent<SpinDetector>();
        deceleration = Mathf.Max(1, deceleration);
    }

    public void Spin()
    {
        if (IsSpinning) return;
        wheel.localRotation = Quaternion.identity;
        InitialSpeed = initialSpeeds[UnityEngine.Random.Range(0, initialSpeeds.Length)];
        currentSpeed = InitialSpeed;
    }

    public void ResetWheel()
    {
        currentSpeed = 0;
        wheel.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!IsSpinning) return;
        float dt = Mathf.Min(Time.deltaTime, currentSpeed / deceleration);
        float nextSpeed = Mathf.Max(0, currentSpeed - deceleration * dt);
        wheel.Rotate(0, 0, (currentSpeed + nextSpeed) * .5f * dt);
        currentSpeed = nextSpeed;
        if (currentSpeed == 0)
        {
            var slot = detector.Detect(wheel);
            Completed?.Invoke(slot != null ? slot.Index : -1);
        }
    }
}
