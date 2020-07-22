using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclesSelector : MonoBehaviour, IObstacleSelector
{

    private List<GameObject> _list = new List<GameObject>();
    private System.Random _rand = new System.Random();

    private void Awake() {


        int n = transform.childCount;
        while (n-- > 0) {
            _list.Add(transform.GetChild(n).gameObject);
        }
    }

    public bool SelectRandom(int n) {

        if(_list.Count >= n) {

            _list.ForEach(x => x.SetActive(false));

            _list.OrderBy(x => _rand.Next()).Take(n).ToList().ForEach(x => x.SetActive(true));

        }

        return false;

    }

}

internal interface IObstacleSelector {
    bool SelectRandom(int n);
}