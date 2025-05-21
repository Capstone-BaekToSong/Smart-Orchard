using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static TreeManager;

public class HealthBar : MonoBehaviour
{
    public TreeManager treeManager;
    
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    // Start is called before the first frame update
    void Start()
    {
        treeManager = GetComponent<TreeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        fill.color = gradient.Evaluate(slider.normalizedValue);
        slider.value = treeManager.HP;
    }
}
