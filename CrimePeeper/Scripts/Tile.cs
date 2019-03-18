using UnityEngine;

public class Tile : MonoBehaviour
{
    // Hold public properties for each tile to be updated in the "Controller" script.
    public bool crime = false;
    public bool clicked = false;
    public int adjacentCrimes = 0;
}
