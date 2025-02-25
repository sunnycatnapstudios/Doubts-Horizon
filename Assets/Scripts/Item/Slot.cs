using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot
{
    Item item;
    int count =0;
    public Slot(Item item){
        this.item = item;
    }
    public Item GetItem()
    {
        return item; 

    }

    public void incCount()
    {
        count++;
    }
    public override string ToString() => item.GetName()+" "+ count.ToString();
    public void decCount() { count--; }
    public void setCount(int count) { this.count = count; }
    public string getName() { return item.GetName(); }
    public int getCount() { return count; }
    
    
    
}
