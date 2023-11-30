using UnityEngine;

[CreateAssetMenu(fileName = "Application Settings", menuName = "")]
public class ApplicationConfig : ScriptableObject
{
    public string ApplicationPath;
    public string RegistryKey;
    public string RegistryValue;
}
