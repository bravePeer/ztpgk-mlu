using UnityEngine;

public class experimental : MonoBehaviour
{
    public GameObject fl;
    public GameObject fr;
    public GameObject rr;
    public GameObject rl;


    private float turn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        getInput();

        // Skrêcanie kó³ w lewo i prawo
        if (turn != 0)
        {
            
            Vector3 currentLocalEulerFL = fl.transform.localRotation.eulerAngles;
            fl.transform.localRotation = Quaternion.Euler(currentLocalEulerFL.x, turn * 30, currentLocalEulerFL.z);

            Vector3 currentLocalEulerFR = fr.transform.localRotation.eulerAngles;
            fr.transform.localRotation = Quaternion.Euler(currentLocalEulerFR.x, turn * 30, currentLocalEulerFR.z);
        }
    }
    private void getInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            turn = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turn = 1;
        }
        else
        {
            turn = 0;
        }
    }
}
