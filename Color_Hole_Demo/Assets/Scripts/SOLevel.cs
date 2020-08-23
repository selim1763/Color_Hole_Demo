using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 1)]
public class SOLevel : ScriptableObject
{
    public Transform levelPrefab;
    public Color groundColor;
    public Color edgeColor;
    public Color interactableObjectColor;
    public Color obstacleColor;
}