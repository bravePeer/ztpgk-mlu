using UnityEngine;

public class experimental : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider fl;
    public WheelCollider fr;
    public WheelCollider rr;
    public WheelCollider rl;

    [Header("Physics Materials")]
    public PhysicsMaterial dryMaterial;
    public PhysicsMaterial wetMaterial;
    public PhysicsMaterial iceMaterial;

    [Header("Friction Stiffness Settings")]
    [Tooltip("Mno�nik przyczepno�ci na suchej nawierzchni (standardowo 1.0)")]
    public float dryStiffness = 1.0f;
    [Tooltip("Mno�nik przyczepno�ci na mokrej nawierzchni")]
    public float wetStiffness = 0.7f;
    [Tooltip("Mno�nik przyczepno�ci na lodzie")]
    public float iceStiffness = 0.25f;

    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        getInput();
    }

    void getInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // Poruszanie i skr�canie
        fl.motorTorque = verticalInput * 1000;
        fr.motorTorque = verticalInput * 1000;
        rr.motorTorque = verticalInput * 1000;
        rl.motorTorque = verticalInput * 1000;

        fl.steerAngle = horizontalInput * 30;
        fr.steerAngle = horizontalInput * 30;

        // Aktualizacja tarcia dla ka�dego ko�a osobno
        UpdateWheelFriction(fl);
        UpdateWheelFriction(fr);
        UpdateWheelFriction(rr);
        UpdateWheelFriction(rl);
    }

    void UpdateWheelFriction(WheelCollider wheel)
    {
        WheelHit hit;
        // Sprawdzamy, czy ko�o w og�le dotyka pod�o�a
        if (wheel.GetGroundHit(out hit))
        {
            PhysicsMaterial surfaceMat = hit.collider.sharedMaterial;

            if (surfaceMat != null)
            {
                // Por�wnujemy materia� pod�o�a z przypisanymi w Inspectorze
                if (surfaceMat == dryMaterial)
                {
                    SetWheelStiffness(wheel, dryStiffness);
                }
                else if (surfaceMat == wetMaterial)
                {
                    SetWheelStiffness(wheel, wetStiffness);
                }
                else if (surfaceMat == iceMaterial)
                {
                    SetWheelStiffness(wheel, iceStiffness);
                }
                else
                {
                    // Domy�lna warto��, je�li pod�o�e ma inny materia�
                    SetWheelStiffness(wheel, 1.0f);
                }
            }
        }
    }

    void SetWheelStiffness(WheelCollider wheel, float stiffness)
    {
        // Zmiana tarcia wzd�u�nego (jazda/hamowanie)
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = stiffness;
        wheel.forwardFriction = forwardFriction;

        // Zmiana tarcia bocznego (skr�canie/drift)
        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = stiffness;
        wheel.sidewaysFriction = sidewaysFriction;
    }
}