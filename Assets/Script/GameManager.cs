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
    Label randomFood;
    Label chooseFood;
    Label ChosenLabel;
    Button closeButton;
    Button clearButton;
    Button SubmitButton;
    Button loseButton;
    Button winButton;
    Button stopButton;
    Dictionary<string, Button> materialButtons;
    List<string> selectedMaterials = new List<string>();
    Dictionary<string, List<string>> recipeBook = new Dictionary<string, List<string>>
    {
        { "黑料理", new List<string> { "A", "C" } },
        { "紅料理", new List<string> { "B", "C" } },
        { "白料理", new List<string> { "A", "D", "E" } }
    };
    string nowRecipe;
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
                button.AddToClassList("selected");

                SortSelectedMaterials();
                UpdateChosenLabel();
            };
        }
        SubmitButton.clicked += () =>
        {
            if (recipeBook[currentRecipeName].Count == selectedMaterials.Count && !recipeBook[currentRecipeName].Except(selectedMaterials).Any())
            {
                LittleGame();
            }
            else
            {
                Lose();
            }
        };
        RandomFood();
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
        stopButton = rootWin.Q<Button>("stopButton");
        stopButton.clicked += () =>
        {
            Win();
            littleGame.SetActive(false);
        };
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
        randomFood.text =  $"隨機抽取：{currentRecipeName}";
    }
}
