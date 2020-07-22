using NeatImpl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UISettings : MonoBehaviour
{
    [SerializeField] private Slider sliderObstacles;
    [SerializeField] private Text textObstacles;

    [SerializeField] private Dropdown dropdownModel;

    [SerializeField] private Button btnStart;


    [Inject]
    private IGameController _gameController;


    private Dictionary<string, Genome> _mapModels = new Dictionary<string, Genome>();


    private void Start() {


        Setup();
    }

    void Setup() {

        dropdownModel.ClearOptions();

        LoadModelsByExtension("trainedmodels\\",".g");

        sliderObstacles.onValueChanged.AddListener(x => textObstacles.text = x.ToString());

    }
    void LoadModelsByExtension(string directory,string extension) {

        var files = Directory.GetFiles(directory, "*" + extension, SearchOption.AllDirectories);

        var models = GetModelsFromFiles(files).ToArray();

        var list = new List<Dropdown.OptionData>();

        foreach (var item in files.Zip(models,(name,data) => new { name, data })) {
            string name = Path.GetFileNameWithoutExtension(item.name);
            list.Add(new Dropdown.OptionData(name));
            _mapModels.Add(name, item.data);
            
        }
        dropdownModel.AddOptions(list);




    }
    IEnumerable<Genome> GetModelsFromFiles(IEnumerable<string> files) {
        foreach (var item in files) {
            yield return new Genome(Utils.DataOps.XmlLoad<GenomeData>(item));
        }
    }


}
