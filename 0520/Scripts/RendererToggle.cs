using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererToggle : MonoBehaviour
{
    public enum rendererState {full, rotten, empty};
    public rendererState state = rendererState.empty;

    public MeshRenderer[] fullTree;
    public MeshRenderer[] rottenTree;
    public MeshRenderer[] emptyTree;


    public void makeFullTree()
    {
        if (state == rendererState.rotten)
        {
            foreach (MeshRenderer r in fullTree)
            {
                r.enabled = true;
            }
            foreach (MeshRenderer r in rottenTree)
            {
                r.enabled = false;
            }
        }
        if (state == rendererState.empty)
        {
            foreach (MeshRenderer r in fullTree)
            {
                r.enabled = true;
            }
            foreach (MeshRenderer r in emptyTree)
            {
                r.enabled = false;
            }
        }
        state = rendererState.full;
    }

    public void makeRottenTree()
    {
        if (state == rendererState.full)
        {
            foreach (MeshRenderer r in rottenTree)
            {
                r.enabled = true;
            }
            foreach (MeshRenderer r in fullTree)
            {
                r.enabled = false;
            }
        }
        if (state == rendererState.empty)
        {
            foreach (MeshRenderer r in rottenTree)
            {
                r.enabled = true;
            }
            foreach (MeshRenderer r in emptyTree)
            {
                r.enabled = false;
            }
        }
        state = rendererState.rotten;
    }

    public void makeEmptyTree()
    {
        if (state == rendererState.full)
        {
            foreach (MeshRenderer r in emptyTree)
            {
                r.enabled = true;
            }
            foreach (MeshRenderer r in fullTree)
            {
                r.enabled = false;
            }
        }
        if (state == rendererState.rotten)
        {
            foreach (MeshRenderer r in emptyTree)
            {
                r.enabled = true;
            }
            foreach (MeshRenderer r in rottenTree)
            {
                r.enabled = false;
            }
        }
        state = rendererState.empty;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (MeshRenderer r in fullTree)
        {
            r.enabled = false;
        }
        foreach (MeshRenderer r in rottenTree)
        {
            r.enabled = false;
        }
        foreach (MeshRenderer r in emptyTree)
        {
            r.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
