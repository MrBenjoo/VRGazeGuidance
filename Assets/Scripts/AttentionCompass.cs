using UnityEngine;
using Tobii.Research.Unity;
using Valve.VR;

public class AttentionCompass : MonoBehaviour
{
    private Transform stimuli;
    public Vector3 stimuliVector;
    private SpriteRenderer stimuliSprite;
    public Color stimuliColor = new Color(1.0f, 1.0f, 1.0f);
    public Vector3 stimuliScale = new Vector3(0.05f, 0.05f, 0.0f);
    public float stimuliCenterOffset = 1.0f;

    public GameObject target;
    private Vector3 targetVector;
    public float targetDistance;
    public bool targetInFront;
    public bool targetInView;

    public float alpha;
    public float alphaMin = 0.25f;
    public float alphaMax = 0.75f;
    public float alphaNearFade = 0.2f;
    public float alphaFarFade = 0.8f;

    public bool pulseOscillator = true;
    public float pulseFrequency = 10;
    public float pulseDutyCycle = 0.5f;
    public float oscillatorFrequency = 45;

    public VRGazeTrail gaze;
    public Vector3 gazeVector;
    public float gazeDistance;
    public float gazeToStimuliDistance;



    // Start is called before the first frame update
    void Start()
    {
        // vSync needs to be disabled (Edit -> Projekt Settings... -> Quality -> V Sync Count: Don't Sync)
        Application.targetFrameRate = 90;
        stimuli = transform;
        stimuli.localScale = stimuliScale;
        stimuliSprite = GetComponent<SpriteRenderer>();
        stimuliSprite.color = stimuliColor;
        gaze = VRGazeTrail.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Get and translate world points to viewport center offset vectors
        stimuliVector = worldToViewportCenterPoint(stimuli.position);
        targetVector = worldToViewportCenterPoint(target.transform.position);
        gazeVector = worldToViewportCenterPoint(gaze.LatestHit.point);

        // Calculate target distance parameters
        targetDistance = getDistance(targetVector.x, targetVector.y);
        targetInFront = targetVector.z >= 0.0f;
        targetInView = targetInFront && targetDistance <= stimuliCenterOffset;

        // Calculate gaze distance parameters
        gazeDistance = getDistance(gazeVector.x, gazeVector.y);
        gazeToStimuliDistance = getDistance(gazeVector.x - stimuliVector.x, gazeVector.y - stimuliVector.y);

        // If target is behind camera, flip x- and y-axis
        if (!targetInFront)
        {
            targetVector.x *= -1;
            targetVector.y *= -1;
        }

        // Set stimuli alpha and/or flicker
        alpha = stimuliColor.a;
        stimuliColor.a = targetInView && targetDistance < gazeToStimuliDistance ? getAlpha(targetDistance) : getAlpha(gazeToStimuliDistance);
        stimuliSprite.color = pulseOscillator ? stimuliColor * pulseOscillate() : stimuliColor;

        // Calculate new stumli position
        float angle = Mathf.Atan2(targetVector.x, targetVector.y);
        targetVector.x = stimuliCenterOffset / 2 * Mathf.Sin(angle) + 0.5f;
        targetVector.y = stimuliCenterOffset / 2 * Mathf.Cos(angle) + 0.5f;
        targetVector.z = 0.5f;

        // Update stumli position and scale
        stimuli.LookAt(Camera.main.transform.position, Camera.main.transform.forward);
        stimuli.position = Camera.main.ViewportToWorldPoint(targetVector);
        stimuli.localScale = stimuliScale;
    }

    // Pulse oscillator
    float pulseOscillate()
    {
        float pulse = Time.time * pulseFrequency;
        float oscillator = Time.time * oscillatorFrequency;
        pulse -= Mathf.Floor(pulse);
        oscillator -= Mathf.Floor(oscillator);
        return (pulse < pulseDutyCycle && oscillator < 0.5f) ? 1.0f : 0.0f;
    }

    // Calculate distance from center
    float getDistance(float x, float y)
    {
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) * 2;
    }

    // Calculate alpha based on distance
    float getAlpha(float distance)
    {
        float distanceScale = Mathf.Clamp01((distance * (1 / stimuliCenterOffset) - alphaNearFade) / (alphaFarFade - alphaNearFade));
        return Mathf.Clamp(((alphaMax - alphaMin) * distanceScale) + alphaMin, alphaMin, alphaMax);
    }

    Vector3 worldToViewportCenterPoint(Vector3 vector)
    {
        Vector3 v3 = Camera.main.WorldToViewportPoint(vector);
        v3.x -= 0.5f;
        v3.y -= 0.5f;
        return v3;
    }
}
