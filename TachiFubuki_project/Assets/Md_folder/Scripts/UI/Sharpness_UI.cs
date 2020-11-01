using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sharpness_UI : MonoBehaviour
{
    private Slider slider_component;
    // Start is called before the first frame update
    void Start()
    {
        slider_component = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMaxSharpness(int delta)
    {
        slider_component.maxValue = delta;
    }

    public void UpdateSharpness(int delta)
    {
        slider_component.value = delta;
    }
}
