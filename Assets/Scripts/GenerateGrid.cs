
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using DG.Tweening;

public class GenerateGrid : MonoBehaviour, ISettingsUI
{
    private int _gridHeight;
    private int _gridWidth;

    [SerializeField] private TextMeshProUGUI _prefabCell;
    [SerializeField] private RectTransform _containerCells;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _shuffleButton;
    [SerializeField] private TMP_InputField _widthInput;
    [SerializeField] private TMP_InputField _heightInput;

    private List<char> _charCells = new List<char>();
    private List<TextMeshProUGUI> _cellsText = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> _poolObjects = new List<TextMeshProUGUI>();
    
    char[] characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private ObjectsPool _pool = new ObjectsPool();

    private async void Awake()
    {
        await Task.CompletedTask;
        _generateButton.onClick.AddListener(delegate
        {
            if (_cellsText != null)
            {
                foreach (var item in _poolObjects)
                {
                    item.gameObject.SetActive(false);
                }
            }
            GenerateCells();
            UpdateListWithActiveCells();
        });

        _shuffleButton.onClick.AddListener(async delegate
        {
            await Task.CompletedTask;
            HideShowChars();
            UpdateListWithCharacters();
            
        });

        _widthInput.onEndEdit.AddListener(delegate
        {
            _gridWidth = int.Parse(_widthInput.text);
        });

        _heightInput.onEndEdit.AddListener(delegate
        {
            _gridHeight = int.Parse(_heightInput.text);
        });
    }

    private void UpdateListWithCharacters()
    {
        _charCells.Clear();
        for (int i = 0; i < _cellsText.Count; i++)
        {
            if (_cellsText[i].gameObject.activeInHierarchy)
            {
                var character = Convert.ToChar(_cellsText[i].text);
                _charCells.Add(character);
            }
        }
    }

    private void UpdateListWithActiveCells()
    {
        _cellsText.Clear();
        foreach (Transform child in _containerCells.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                _cellsText.Add(child.GetComponent<TextMeshProUGUI>());
            }
        }
    }

    private void GenerateCells()
    {
        for (int i = 0; i < _gridHeight; i++)
        {
            for (int j = 0; j < _gridWidth; j++)
            {
                var position = new Vector3(j * _prefabCell.rectTransform.rect.x, i * _prefabCell.rectTransform.rect.y, 0);

                _pool.PoolCells<TextMeshProUGUI>(position, _poolObjects, _prefabCell, _containerCells, _cellsText);
            }
        }
        foreach (var item in _cellsText)
        {
            item.text = RandomStringGenerator(characters);

             if (item.gameObject.activeInHierarchy)
             {
                var char1 = Convert.ToChar(item.text);
                _charCells.Add(char1);
             }
        }
        SettingsGrid();
    }

    public string RandomStringGenerator(char[] randomChar)
    {
        string generated_string = "";

        generated_string += randomChar[UnityEngine.Random.Range(0, randomChar.Length)];

        return generated_string;
    }

    public string RandomStringGenerator(List<char> randomChar)
    {

        string generated_string = "";

        var index = UnityEngine.Random.Range(0, randomChar.Count);
        
        generated_string += randomChar[index];
        _charCells.RemoveAt(index);

        return generated_string;
    }

    private async void HideShowChars()
    {
        await HideShowChar(_cellsText, 0f);
        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        foreach (var cell in _cellsText)
        {
            cell.text = RandomStringGenerator(_charCells);
        }
        await HideShowChar(_cellsText, 1f);
    }
    private async Task HideShowChar(List<TextMeshProUGUI> text, float alpha)
    {
        await Task.Yield();
        for (int i = 0; i < text.Count; i++)
        {
            text[i].DOFade(alpha, 0.5f);
        }
    }

    public void SettingsGrid()
    {
        _gridLayoutGroup.constraintCount = _gridWidth;

        float sizeCell = 0f;

        if (_gridHeight > _gridWidth)
            sizeCell = _containerCells.rect.height / _gridHeight;
        else
            sizeCell = _containerCells.rect.width / _gridWidth;

        _gridLayoutGroup.cellSize = new Vector2(sizeCell, sizeCell);
    }
}
