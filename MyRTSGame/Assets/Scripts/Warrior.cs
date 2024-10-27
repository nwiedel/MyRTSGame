using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : IHuman
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

    /// <summary>
    /// Die Originalfarbe des Warriors
    /// </summary>
    private Color normalColor;

    private void Start()
    {
        normalColor = originalMat.GetColor("_BaseColor");
        _material = Instantiate(originalMat);
        GetComponent<Renderer>().material = _material;
    }

    private void Update()
    {
        if (isSelected != toogle)
        {
            toogle = isSelected;

            if (isSelected)
            {
                _material.SetColor("_BaseColor", Color.red);
            }
            else
            {
                _material.SetColor("_BaseColor", normalColor);
            }
        }
    }
}
