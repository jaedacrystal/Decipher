using UnityEngine;
using System.Reflection;

public class GameObjectCaller : MonoBehaviour
{
    public string objectName;
    public string methodName;
    public string componentName;

    public void DoMethod()
    {
        GameObject obj = FindDeepNestedObjectByName(objectName);

        if (obj != null)
        {
            Debug.Log("Calling " + methodName + " on " + obj.name);

            Component targetComponent = obj.GetComponent(componentName);
            if (targetComponent != null)
            {
                MethodInfo method = targetComponent.GetType().GetMethod(methodName);
                if (method != null)
                {
                    method.Invoke(targetComponent, null);
                }
            }
        }
    }

    private GameObject FindDeepNestedObjectByName(string targetName)
    {
        GameObject[] allRoots = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject root in allRoots)
        {
            if (root.hideFlags == HideFlags.None)
            {
                GameObject match = SearchRecursively(root.transform, targetName);
                if (match != null)
                    return match;
            }
        }

        return null;
    }

    private GameObject SearchRecursively(Transform parent, string targetName)
    {
        if (parent.name == targetName)
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject found = SearchRecursively(child, targetName);
            if (found != null)
                return found;
        }

        return null;
    }
}

