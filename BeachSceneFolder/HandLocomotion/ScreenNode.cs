using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenNode
{
   
    public GameObject Screen { get; private set; }
    public List<ScreenNode> Children { get; private set; }
    public ScreenNode Parent { get; private set; }

    public ScreenNode(GameObject screen)
    {
        Screen = screen;
        Children = new List<ScreenNode>();
   
    }

    public void AddChild(ScreenNode child, ScreenNode father)
    {
     
        child.Parent = father;
        Debug.Log("Filho: "+child.Screen.name + " // Pai: "+ child.Parent.Screen.name);
        Children.Add(child);
        
    }
}
