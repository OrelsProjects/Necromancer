using UnityEngine;

[CreateAssetMenu(fileName = "AreaLevel", menuName = "Necromancer/Area/Level", order = 2)]

public class AreaLevel : ScriptableObject
{
    public int Level = 1;
    public int CurrencyPerMinute = 1;
    public int PriceToUpgrade = 1;
}
