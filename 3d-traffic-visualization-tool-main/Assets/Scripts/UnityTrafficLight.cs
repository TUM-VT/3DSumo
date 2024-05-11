using System.Collections.Generic;
using UnityEngine;

public class UnityTrafficLight : MonoBehaviour
{
    float time = 0;
    public List<string> dirs;

    public List<GameObject> lights;

    public Material greenMat;
    public Material yellowMat;
    public Material redMat;

    void Start()
    {
        foreach (var light in lights)
        {
            light.SetActive(false);
        }
        GenerateLights();
    }

    void GenerateLights()
    {
        foreach (var dir in dirs)
        {
            switch (dir)
            {
                case "s":
                    lights[0].SetActive(true);
                    lights[1].SetActive(true);
                    lights[2].SetActive(true);
                    break;
                case "r": lights[3].SetActive(true); break;
                case "l": lights[4].SetActive(true); break;
            }
        }
    }

    public void SwitchLight(string dir, char state)
    {
        if (dir == "s")
        {
            lights[0].GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
            lights[1].GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
            lights[2].GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
            if (state == 'r')
            {
                lights[0].GetComponent<Renderer>().materials[1].EnableKeyword("_EMISSION");
            }
            else if (state == 'y')
            {
                lights[1].GetComponent<Renderer>().materials[1].EnableKeyword("_EMISSION");
            }
            else if (state == 'g')
            {
                lights[2].GetComponent<Renderer>().materials[1].EnableKeyword("_EMISSION");
            }
        }
        else if (dir == "r")
        {
            var materials = lights[3].GetComponent<Renderer>().materials;
            if (state == 'r')
            {
                materials[1] = redMat;
            }
            else if (state == 'y')
            {
                materials[1] = yellowMat;
            }
            else if (state == 'g')
            {
                materials[1] = greenMat;
            }
        }
        else if (dir == "l")
        {
            var materials = lights[4].GetComponent<Renderer>().materials;
            if (state == 'r')
            {
                materials[1] = redMat;
            }
            else if (state == 'y')
            {
                materials[1] = yellowMat;
            }
            else if (state == 'g')
            {
                materials[1] = greenMat;
            }
        }
    }
}
