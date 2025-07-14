using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Internal;
using System.Collections;

public class NPC : MonoBehaviour
{
    public Camera camera1;
    UIDocument foodDocument;
    public GameManager gameManager;
    VisualElement root;
    VisualElement[] material = new VisualElement[5];
    Button table;
    Vector3[] foodVector = new Vector3[6];
    public float moveSpeed = 2f;
    bool isArrive = false;
    void Start()
    {
        foodDocument = gameObject.GetComponent<UIDocument>();
        root = foodDocument.rootVisualElement;
        material[0] = root.Q<VisualElement>("A");
        material[1] = root.Q<VisualElement>("B");
        material[2] = root.Q<VisualElement>("C");
        material[3] = root.Q<VisualElement>("D");
        material[4] = root.Q<VisualElement>("E");
        table = root.Q<Button>("table");
        table.clicked += () =>
        {
            gameManager.ChooseFood();
        };
    }

    public void MoveToUI(string[] input)
    {
        StopAllCoroutines();
    }

}
