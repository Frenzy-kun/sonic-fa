using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecorationGeneratorProfile", menuName = "LevelEditor/DecorationGeneratorProfile", order = 1)]
public class DecorationGeneratorProfile : ScriptableObject
{
    public List<GameObject> mDecorations;
}
