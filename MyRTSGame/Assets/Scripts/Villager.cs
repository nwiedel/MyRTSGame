using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : IHuman
{
    /// <summary>
    /// Verweis auf das originale Material.
    /// </summary>
    public Material originalMat;

    /// <summary>
    /// Zwischenspeicher für das Material, um nicht
    /// das Original überschreiben zu müssen.
    /// </summary>
    private Material _material;

    /// <summary>
    /// Schalter zum wechseln der Farbe
    /// </summary>
    private bool toogle;

    private void Start()
    {
        _material = Instantiate(originalMat);
        GetComponent<Renderer>().material = _material;
    }

    private void Update()
    {
        if(isSelected != toogle)
        {
            toogle = isSelected;

            if (isSelected)
            {
                _material.SetColor("_BaseColor", Color.green);
            }
            else
            {
                _material.SetColor("_BaseColor", Color.white);
            }
        }
    }
}
