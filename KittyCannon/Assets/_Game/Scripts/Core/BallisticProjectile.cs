using UnityEngine;

public class BallisticProjectile : MonoBehaviour
{
    [Header("Physics (Your Code)")]
    [SerializeField] Vector3 gravity = new Vector3(0f, -9.81f, 0f); // acceleration (m/s^2)
    [SerializeField] Vector3 constantAccelerationXZ = Vector3.zero; // e.g., wind: (ax,0,az)

    [Header("Collision")]
    [SerializeField] bool useGroundPlane = true;
    [SerializeField] float groundY = 0f;              // "floor" height
    [SerializeField] bool useRaycastHits = false;     // discrete hit checks along the step
    [SerializeField] LayerMask hitMask = ~0;          // what counts as a hit

    [Header("Lifetime")]
    [SerializeField] float maxLifetime = 30f;

    // State
    Vector3 p0;   // launch position
    Vector3 v0;   // launch velocity
    float   t;    // time since launch
    bool    launched;

    public System.Action<Vector3> OnImpact; // subscribe for effects/score

    public void Launch(Vector3 position, Vector3 initialVelocity)
    {
        p0 = position;
        v0 = initialVelocity;
        t  = 0f;
        launched = true;
        transform.position = p0;
    }

    void Update()
    {
        if (!launched) return;

        float dt = Time.deltaTime;

        // Previous and candidate times/positions
        float tPrev = t;
        t += dt;

        // total acceleration (you own this)
        Vector3 a = gravity + constantAccelerationXZ;

        // closed-form position at time t
        Vector3 posPrev = PositionAt(tPrev, p0, v0, a);
        Vector3 posNow  = PositionAt(t,     p0, v0, a);

        // Optional raycast to stop exactly at first collider we cross this frame
        if (useRaycastHits)
        {
            Vector3 delta = posNow - posPrev;
            float dist = delta.magnitude;
            if (dist > 0f && Physics.Raycast(posPrev, delta.normalized, out var hit, dist, hitMask, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point;
                Impact(hit.point);
                return;
            }
        }

        // Ground-plane resolution (no physics engine; exact landing between frames)
        if (useGroundPlane && posNow.y <= groundY && posPrev.y > groundY)
        {
            // Solve for tau in [0,1] where y(tPrev + tau*dt) = groundY
            // y(t) = y0 + v0y*t + 0.5*ay*t^2
            float y0   = p0.y;
            float vy0  = v0.y;
            float ay   = a.y;

            // We want y(tPrev + s) = groundY, where 0<=s<=dt
            // Let u = tPrev + s. Solve 0.5*ay*u^2 + vy0*u + (y0 - groundY) = 0 for u, pick root in [tPrev, t]
            float A = 0.5f * ay;
            float B = vy0;
            float C = y0 - groundY;

            // Quadratic formula
            float disc = B*B - 4f*A*C;
            if (disc >= 0f)
            {
                float sqrt = Mathf.Sqrt(disc);
                // two candidates
                float u1 = (-B - sqrt) / (2f*A);
                float u2 = (-B + sqrt) / (2f*A);

                // pick the root between tPrev and t
                float u = PickRootInInterval(u1, u2, tPrev, t);
                if (!float.IsNaN(u))
                {
                    Vector3 landing = PositionAt(u, p0, v0, a);
                    landing.y = groundY;
                    transform.position = landing;
                    Impact(landing);
                    return;
                }
            }
        }

        // No impact this frame → update normally
        transform.position = posNow;

        // Kill after a while
        if (t >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    Vector3 PositionAt(float time, Vector3 p0, Vector3 v0, Vector3 a)
        => p0 + v0 * time + a * (0.5f * (time * time));

    float PickRootInInterval(float u1, float u2, float lo, float hi)
    {
        bool r1 = u1 >= lo && u1 <= hi;
        bool r2 = u2 >= lo && u2 <= hi;
        if (r1 && r2) return Mathf.Min(u1, u2); // first contact
        if (r1) return u1;
        if (r2) return u2;
        return float.NaN;
    }

    void Impact(Vector3 point)
    {
        launched = false;
        OnImpact?.Invoke(point);
        // Example: spawn VFX, SFX, score popup, then destroy
        Destroy(gameObject, 0.05f);
    }
}
