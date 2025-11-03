using UnityEngine;
using TMPro;

public class CannonController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform muzzle;                 // where the projectile spawns
    [SerializeField] GameObject projectilePrefab;      // must have BallisticProjectile
    [SerializeField] TMP_InputField powerInput;        // initial speed in m/s
    [SerializeField] TMP_InputField angleInput;        // elevation angle in degrees
    [SerializeField] TMP_Text     statusText;          // optional feedback

    [Header("Defaults")]
    [SerializeField, Min(0f)] float defaultPower = 25f;
    [SerializeField, Range(0f, 85f)] float defaultAngle = 35f;

    // Optional extras
    [SerializeField] float yawDegrees = 0f;            // turn cannon left/right (kitty cannon is mostly 2D; set yaw=0)

    public void Fire()
    {
        // Parse inputs (fallback to defaults if blank/invalid)
        float speed = TryParseOrDefault(powerInput?.text, defaultPower);
        float elevDeg = TryParseOrDefault(angleInput?.text, defaultAngle);

        // Compute initial velocity from cannon orientation + user angles
        // You can aim either by rotating the muzzle in the scene OR by math here.
        // We'll compute direction using yaw (around Y) and elevation (around X).
        Quaternion aim =
            Quaternion.Euler(0f, yawDegrees, 0f) *   // left/right
            Quaternion.Euler(-elevDeg, 0f, 0f);      // -X tilts "up" in Unity forward convention

        Vector3 dir = aim * Vector3.forward;         // forward of the muzzle
        Vector3 v0  = dir.normalized * speed;

        // Spawn and initialize
        var go = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        var proj = go.GetComponent<BallisticProjectile>();
        proj.Launch(muzzle.position, v0);

        if (statusText) statusText.text = $"Fired: {speed:0.0} m/s @ {elevDeg:0}°";
    }

    static float TryParseOrDefault(string s, float fallback)
        => float.TryParse(s, out var v) ? v : fallback;
}
