using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] float previewSpeed = 25f;
    [SerializeField, Range(0f, 85f)] float previewAngleDeg = 35f;
    [SerializeField] float yawDegrees = 0f;
    [SerializeField] Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField] int   points = 40;
    [SerializeField] float dt = 0.08f;

    LineRenderer lr;

    void Awake() => lr = GetComponent<LineRenderer>();

    void LateUpdate()
    {
        if (!lr || !muzzle) return;

        Quaternion aim =
            Quaternion.Euler(0f, yawDegrees, 0f) *
            Quaternion.Euler(-previewAngleDeg, 0f, 0f);

        Vector3 v0 = (aim * Vector3.forward) * previewSpeed;

        lr.positionCount = points;
        for (int i = 0; i < points; i++)
        {
            float t = i * dt;
            Vector3 p = muzzle.position + v0 * t + gravity * (0.5f * (t * t));
            lr.SetPosition(i, p);
        }
    }

    // You can expose setters for UI sliders to update previewSpeed/previewAngleDeg live.
    public void SetSpeed(float v) => previewSpeed = v;
    public void SetAngle(float deg) => previewAngleDeg = deg;
    public void SetYaw(float deg) => yawDegrees = deg;
}