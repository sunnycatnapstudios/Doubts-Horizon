using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item: ScriptableObject{
    public abstract string GetName();
    

    public abstract void Use();

    public abstract string GetDesc();
   
    public abstract string GetFlavour();
    public abstract Sprite GetSprite();



}