using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererToggle : MonoBehaviour
{
    public enum rendererState {full, rotten, empty};
    public rendererState state;

    public string treeType;

    public MeshRenderer[] fullTree;
    public MeshRenderer[] rottenTree;
    public MeshRenderer[] emptyTree;

    public void initializeRenderers()
    {
        foreach (MeshRenderer r in fullTree)
        {
            r.enabled = false;
        }
        foreach (MeshRenderer r in emptyTree)
        {
            r.enabled = true;
        }
        foreach (MeshRenderer r in rottenTree)
        {
            r.enabled = false;
        }
        state = rendererState.empty;
    }

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

    void Awake()
    {
        treeType = transform.parent.name;

        if (treeType == "apple_trees")
        {
            fullTree = transform.Find("apple_full_tree")?.GetComponentsInChildren<MeshRenderer>();
            rottenTree = transform.Find("apple_rotten_tree")?.GetComponentsInChildren<MeshRenderer>();
            emptyTree = transform.Find("apple_empty_tree")?.GetComponentsInChildren<MeshRenderer>();
        }
        if (treeType == "mogua_trees")
        {
            fullTree = transform.Find("mogua_full_tree")?.GetComponentsInChildren<MeshRenderer>();
            rottenTree = transform.Find("mogua_rotten_tree")?.GetComponentsInChildren<MeshRenderer>();
            emptyTree = transform.Find("mogua_empty_tree")?.GetComponentsInChildren<MeshRenderer>();
        }
        if (treeType == "paprika_trees")
        {
            fullTree = transform.Find("paprika_full_tree")?.GetComponentsInChildren<MeshRenderer>();
            rottenTree = transform.Find("paprika_rotten_tree")?.GetComponentsInChildren<MeshRenderer>();
            emptyTree = transform.Find("paprika_empty_tree")?.GetComponentsInChildren<MeshRenderer>();
        }
        if (treeType == "pepper_trees")
        {
            fullTree = transform.Find("pepper_full_tree")?.GetComponentsInChildren<MeshRenderer>();
            rottenTree = transform.Find("pepper_rotten_tree")?.GetComponentsInChildren<MeshRenderer>();
            emptyTree = transform.Find("pepper_empty_tree")?.GetComponentsInChildren<MeshRenderer>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initializeRenderers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
