using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spearman : UnitInteractable
{
    protected override void Awake()
    {
        base.Awake();
       
        perks.Add(Concentration);
    }

    public void Concentration()
    {
        //Реализация перка
    }
}
