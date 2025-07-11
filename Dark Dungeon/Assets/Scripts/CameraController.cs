using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject firstPerson;
    public GameObject thirdPerson;
    private bool isFirstPerson = true;
    void Start()
    {
        ActiveFirstPerson();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
                ActiveFirstPerson();
            else
                ActivethirdPerson();
        }
    }
    
    void ActiveFirstPerson()
    {
        firstPerson.SetActive(true);
        thirdPerson.SetActive(false);
    }

    void ActivethirdPerson()
    {
        thirdPerson.SetActive(true);
        firstPerson.SetActive(false);
    }
}
