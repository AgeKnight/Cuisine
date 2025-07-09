using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public GameObject foodUI;
    public void ChooseFood()
    {
        Debug.Log("1");
        foodUI.SetActive(true);
    }
}
