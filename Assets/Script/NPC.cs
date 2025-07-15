using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Internal;
using System.Collections;
using System;

public class NPC : MonoBehaviour
{
    public Camera camera1;
    public GameManager gameManager;
    UIDocument foodDocument;
    VisualElement root;
    VisualElement[] material = new VisualElement[5];
    Button table;
    public GameObject[] foodVector;
    public float moveSpeed = 2f;
    public bool canMove = false;
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
    public IEnumerator Move(string[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != null)
            {
                yield return StartCoroutine(MoveToUI(foodVector[i].transform.position));
                material[i].style.opacity = 0;
            }
        }
        yield return StartCoroutine(MoveToUI(foodVector[5].transform.position));
        for (int i = 0; i < 5; i++)
        {
            material[i].style.opacity = 1;
        }
        gameManager.Comfirm();
    }
    IEnumerator MoveToUI(Vector3 targetPos)
    {
        while (transform.position != targetPos)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
