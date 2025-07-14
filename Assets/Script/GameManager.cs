using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public UIDocument document;
    UIDocument LoseDocument;
    UIDocument winDocument;
    UIDocument littleGameDocument;
    public GameObject win;
    public GameObject lose;
    public GameObject littleGame;
    public int ChooseFoodIndex;
    string currentRecipeName;
    VisualElement rootFood;
    VisualElement rootLose;
    VisualElement rootWin;
    VisualElement rootLittleGame;
    VisualElement movingBlock;
    Label randomFood;
    Label chooseFood;
    Label ChosenLabel;
    Label pointer;
    Button closeButton;
    Button clearButton;
    Button SubmitButton;
    Button loseButton;
    Button winButton;
    Button stopButton;
    Dictionary<string, Button> materialButtons;
    List<string> selectedMaterials = new List<string>();
    bool isRunning = true;
    float currentX = 0f;
    float direction = 1f;
    float pointerX = 100f;
    float barWidth = 300f;
    float speed = 150f;
    float blockWidth = 60f;
    float pointerWidth = 20f;
    public NPC npc;
    string[] recipe = new string[5];
    Dictionary<string, List<string>> recipeBook = new Dictionary<string, List<string>>
    {
        { "黑料理", new List<string> { "A", "C" } },
        { "紅料理", new List<string> { "B", "C" } },
        { "白料理", new List<string> { "A", "D", "E" } }
    };
    string[] materialOrder = { "A", "B", "C", "D", "E" };
    public void ChooseFood()
    {
        document.enabled = true;
        rootFood = document.rootVisualElement;

        randomFood = rootFood.Q<Label>("TitleLabel");
        chooseFood = rootFood.Q<Label>("SelectedText");
        ChosenLabel = rootFood.Q<Label>("ChosenLabel");

        closeButton = rootFood.Q<Button>("CloseButton");
        clearButton = rootFood.Q<Button>("ClearButton");
        SubmitButton = rootFood.Q<Button>("SubmitButton");
        materialButtons = new Dictionary<string, Button>
        {
            { "A", rootFood.Q<Button>("ButtonA") },
            { "B", rootFood.Q<Button>("ButtonB") },
            { "C", rootFood.Q<Button>("ButtonC") },
            { "D", rootFood.Q<Button>("ButtonD") },
            { "E", rootFood.Q<Button>("ButtonE") }
        };

        closeButton.RegisterCallback<ClickEvent>(closeFood);
        clearButton.RegisterCallback<ClickEvent>(clear);
        foreach (var pair in materialButtons)
        {
            string material = pair.Key;
            Button button = pair.Value;

            button.clicked += () =>
            {
                if (selectedMaterials.Contains(material))
                {
                    return;
                }

                selectedMaterials.Add(material);
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                recipe[(int)asciiEncoding.GetBytes(material)[0] - 65] = material;
                button.AddToClassList("selected");

                SortSelectedMaterials();
                UpdateChosenLabel();
            };
        }
        SubmitButton.clicked += () =>
        {
            npc.MoveToUI(recipe);

        };
        RandomFood();
    }
    public void Comfirm()
    {
        if (recipeBook[currentRecipeName].Count == selectedMaterials.Count && !recipeBook[currentRecipeName].Except(selectedMaterials).Any())
        {
            LittleGame();
        }
        else
        {
            Lose();
        }
    }
    void closeFood(ClickEvent c)
    {
        RealClear();
        document.enabled = false;
    }
    void clear(ClickEvent c)
    {
        RealClear();
    }
    void RealClear()
    {
        selectedMaterials.Clear();
        // 移除每個按鈕的 "selected" 樣式
        foreach (var btn in materialButtons.Values)
        {
            btn.RemoveFromClassList("selected");
        }
        UpdateChosenLabel();
    }
    void Win()
    {
        win.SetActive(true);
        winDocument = win.GetComponent<UIDocument>();
        rootWin = winDocument.rootVisualElement;
        winButton = rootWin.Q<Button>("ConfirmButton");
        winButton.clicked += () =>
        {
            RealClear();
            document.enabled = false;
            win.SetActive(false);
        };
    }
    void LittleGame()
    {
        littleGame.SetActive(true);
        littleGameDocument = littleGame.GetComponent<UIDocument>();
        rootLittleGame = littleGameDocument.rootVisualElement;
        movingBlock = rootLittleGame.Q<VisualElement>("movingBlock");
        pointer = rootLittleGame.Q<Label>("pointer");
        stopButton = rootLittleGame.Q<Button>("stopButton");
        // 決定指標位置

        currentX = 0f;
        isRunning = true;

        float pointerHalf = pointerWidth / 2f;
        pointerX = Random.Range(0, barWidth - pointerHalf);
        pointer.style.left = pointerX - pointerHalf;
        movingBlock.style.left = currentX;

        stopButton.RegisterCallback<ClickEvent>(OnStopClicked);
    }
    void Update()
    {
        MoveBlock();
    }
    void MoveBlock()
    {
        if (!isRunning || movingBlock == null) return;

        currentX += direction * speed * Time.deltaTime;

        // 來回邊界判斷
        if (currentX <= 0)
        {
            currentX = 0;
            direction *= -1;
        }
        else if (currentX >= (barWidth - blockWidth))
        {
            currentX = barWidth - blockWidth;
            direction *= -1;
        }

        // 更新移動方塊位置
        movingBlock.style.left = currentX;
    }
    void OnStopClicked(ClickEvent c)
    {
        isRunning = false;
        float blockLeft = currentX;
        float blockRight = currentX + blockWidth;

        float pointerCenter = pointerX + 10f; // 三角形中心點

        if (pointerCenter >= blockLeft && pointerCenter <= blockRight)
            Win();
        else
            Lose();
        littleGame.SetActive(false);
    }
    void Lose()
    {
        lose.SetActive(true);
        LoseDocument = lose.GetComponent<UIDocument>();
        rootLose = LoseDocument.rootVisualElement;
        loseButton = rootLose.Q<Button>("ConfirmButton");
        loseButton.clicked += () =>
        {
            SceneManager.LoadScene(0);
        };
    }
    void SortSelectedMaterials()
    {
        selectedMaterials.Sort((a, b) =>
            System.Array.IndexOf(materialOrder, a).CompareTo(
            System.Array.IndexOf(materialOrder, b)));
    }
    void UpdateChosenLabel()
    {
        if (selectedMaterials.Count == 0)
        {
            ChosenLabel.text = "已選擇：";
            chooseFood.text = "選擇材料:";
        }
        else
        {
            ChosenLabel.text = "已選擇：" + string.Join("・", selectedMaterials);
            chooseFood.text = "已選擇：" + string.Join("・", selectedMaterials);
        }
    }
    void RandomFood()
    {
        var keys = recipeBook.Keys.ToArray();
        int randomIndex = Random.Range(0, recipeBook.Count);
        currentRecipeName = keys[randomIndex];
        randomFood.text = $"隨機抽取：{currentRecipeName}";
    }
}
