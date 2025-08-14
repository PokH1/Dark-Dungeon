using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject firstPerson;
    public GameObject thirdPerson;
    private bool isFirstPerson = true;
    public Transform audioListener;
    public Camera currentCamera;
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
        currentCamera = firstPerson.GetComponent<Camera>();
        audioListener.SetParent(firstPerson.transform);
        audioListener.localPosition = Vector3.zero;
        audioListener.localRotation = Quaternion.identity;
    }

    void ActivethirdPerson()
    {
        thirdPerson.SetActive(true);
        firstPerson.SetActive(false);
        currentCamera = thirdPerson.GetComponent<Camera>();
        audioListener.SetParent(thirdPerson.transform);
        audioListener.localPosition = Vector3.zero;
        audioListener.localRotation = Quaternion.identity;
    }
}
