using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{    
    public Sprite[] sprites;
    public GameObject[] tiles;
    public GameObject KonamiOnInitial;
    public GameObject KonamiOn;
    public GameObject KonamiOffInitial;
    public GameObject KonamiOff;

    int numberOfCrimes;
    int tileLeftClicked = -1;
    int tileRightClicked = -1;
    int peeped = 9;
    int caught = 10;
    int missed = 11;
    bool konami = false;
    bool konamiOnFirst = true;
    bool konamiOffFirst = true;
    int sequenceIndex = 0;

    // When level is loaded initialise game board to set crimes for level.
    void OnEnable()
    {
        SceneManager.sceneLoaded += Initialise;
    }

    // "Initialise" function will stop listening for the scene to be loaded once the script is disabled.
    void OnDisable()
    {
        SceneManager.sceneLoaded -= Initialise;
    }

    // Handle inputs.
    void Update()
    {
        KonamiCode();
                
        for (int i = 0; i < tiles.Length; i++)
        {
            // Check for whether a player has clicked on a tile that has not already been clicked.
            if ((!tiles[i].GetComponent<Tile>().clicked) && (Click(i)))
            {
                // If the player has left clicked on a tile.
                if (Input.GetMouseButtonDown(0))
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[sprites.Length - 2];
                    tileLeftClicked = i;
                }

                // If the player has right clicked on a tile.
                if (Input.GetMouseButtonDown(1))
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[sprites.Length - 2];
                    tileRightClicked = i;
                }

                // If the player has released left click on a tile.
                if ((Input.GetMouseButtonUp(0)) && (tileLeftClicked == i))
                {
                    // Change tile to "caught" sprite and trigger game over screen.
                    if (tiles[i].GetComponent<Tile>().crime)
                    {
                        tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[caught];
                        SceneManager.LoadScene(5);
                    }
                    // Find amount of adjacent crimes for tile and change to relevant sprite. If there are no adjacent crimes, 
                    // keep checking the adjacent tiles for their adjacent crimes until the blank ones are exhausted.
                    else
                    {
                        FindAdjacentCrimes(i);
                        tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[tiles[i].GetComponent<Tile>().adjacentCrimes];

                        if (tiles[i].GetComponent<Tile>().adjacentCrimes == 0)
                            FindBorderTiles(i);
                    }
                    
                    tiles[i].GetComponent<Tile>().clicked = true;
                    tileLeftClicked = -1;
                }

                // If the player has released right click on a tile.
                if ((Input.GetMouseButtonUp(1)) && (tileRightClicked == i))
                {
                    // Change tile to "peeped" sprite and triggered game complete screen if all crimes are peeped.
                    if (tiles[i].GetComponent<Tile>().crime)
                    {                        
                        tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[peeped];
                        numberOfCrimes--;

                        if (numberOfCrimes == 0)
                            SceneManager.LoadScene(4);
                    }
                    // Change tile to "missed" sprite and trigger game over screen.
                    else
                    {
                        tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[missed];
                        SceneManager.LoadScene(5);
                    }

                    tiles[i].GetComponent<Tile>().clicked = true;
                    tileRightClicked = -1;
                }
            }
        }
    }

    // Set the tiles on the board by randomly generating a set of crimes and distributing them randomly 
    // across the game board. If the level is replayed, reset the sprites for each tile to "unclicked".
    void Initialise(Scene scene, LoadSceneMode mode)
    {
        // Find out what level is being initialised and set it in player preferences 
        // in case the player wants to restart the level after finishing it.
        int level = scene.buildIndex;
        PlayerPrefs.SetInt("previousScene", SceneManager.GetActiveScene().buildIndex);
        int lowerBound, upperBound;
        
        // Sets the range of potential crimes generated depending on the level being played.
        if (level == 3)
        {
            lowerBound = 70;
            upperBound = 90;
        }
        else if (level == 2)
        {
            lowerBound = 25;
            upperBound = 50;
        }
        else
        {
            lowerBound = 5;
            upperBound = 20;
        }

        // Reset sprites.
        foreach (GameObject i in tiles)
            i.GetComponent<SpriteRenderer>().sprite = sprites[sprites.Length - 1];

        // Randomly generate amount of crimes within a range dependant on the level difficulty and amount of tiles.
        numberOfCrimes = (int)(Mathf.Floor(Random.value * (upperBound - lowerBound)) + lowerBound);

        // Distribute crimes across game board.
        for (int i = 0; i < numberOfCrimes; i++)
        {
            int crimeTile;

            do
            {
                crimeTile = (int)(Mathf.Floor(Random.value * (tiles.Length - 1)));
            }
            while (tiles[crimeTile].GetComponent<Tile>().crime);

            tiles[crimeTile].GetComponent<Tile>().crime = true;
        }
    }

    // If the Konami Code is entered, the game will toggle between "Criminal" and "Vigilante" mode. 
    // In Criminal mode you are a criminal scoping out areas to rob. When you right click a tile or when you left 
    // click on a crime tile, the sprites that are revealed will be different to reflect the different game mode.
    void KonamiCode()
    {
        KeyCode[] konamiSequence = { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow,
                                     KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A, KeyCode.Return };

        // Check that the Konami sequence is entered in order.
        if (Input.GetKeyDown(konamiSequence[sequenceIndex]))
        {
            sequenceIndex++;

            // Implement Konami Code functionality.
            if (sequenceIndex >= konamiSequence.Length)
            {
                sequenceIndex = 0;
                konami = !konami;

                // If first time Konami Code has been enabled, trigger explanation of what  
                // it means. Otherwise, give a brief notification that it has been enabled.
                if (konami)
                {
                    if (konamiOnFirst)
                        KonamiOnInitial.SetActive(true);
                    else
                        KonamiOn.SetActive(true);
                }
                // If first time Konami Code has been dinabled, trigger explanation that this  
                // has happened. Otherwise, give a brief notification that it has been disabled.
                else
                {
                    if (konamiOffFirst)
                        KonamiOffInitial.SetActive(true);
                    else
                        KonamiOff.SetActive(true);
                }

                // Change the sprites that are called when the relevant tiles are clicked.
                if (konami)
                {
                    peeped = 12;
                    caught = 13;
                    missed = 14;
                }
                else
                {
                    peeped = 9;
                    caught = 10;
                    missed = 11;
                }

                // Update previously peeped tiles to reflect toggled game mode.
                foreach (GameObject i in tiles)
                    if ((i.GetComponent<Tile>().clicked) && (i.GetComponent<Tile>().crime))
                        i.GetComponent<SpriteRenderer>().sprite = sprites[peeped];

                // Set flags to false to indicate that the next time the
                // Konami Code is triggered, a less verbose pop-up will appear.
                if (!konamiOnFirst)
                    konamiOffFirst = false;

                konamiOnFirst = false;
            }
        }
        else if (Input.anyKeyDown)
            sequenceIndex = 0;

        // If the space bar is pressed, the pop-up information about the Konami Code being enabled/disabled will be removed.
        if ((KonamiOnInitial.activeSelf == true) && (Input.GetKeyDown(KeyCode.Space)))
            KonamiOnInitial.SetActive(false);

        if ((KonamiOn.activeSelf == true) && (Input.GetKeyDown(KeyCode.Space)))
            KonamiOn.SetActive(false);

        if ((KonamiOffInitial.activeSelf == true) && (Input.GetKeyDown(KeyCode.Space)))
            KonamiOffInitial.SetActive(false);

        if ((KonamiOff.activeSelf == true) && (Input.GetKeyDown(KeyCode.Space)))
            KonamiOff.SetActive(false);
    }

    // Checks whether a tile has been clicked on.
    bool Click(int tile)
    {
        Vector3 position = Input.mousePosition;
        Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(position));

        if ((hitCollider != null) && (hitCollider.gameObject == tiles[tile]))
            return true;

        return false;
    }

    // Finds out and sets the amount of adjacent crimes relating to a tile.
    void FindAdjacentCrimes(int tile)
    {
        int[] adjacentTiles = AdjacentTiles(tile);

        foreach (int i in adjacentTiles)
            if (tiles[i].GetComponent<Tile>().crime)
                tiles[tile].GetComponent<Tile>().adjacentCrimes++;
    }

    // Returns a list of iterators to all the existing tiles adjacent to a tile.
    int[] AdjacentTiles(int tile)
    {
        List<int> adjacentTiles = new List<int>();
        int[] tile2D = Convert1DTo2D(tile);

        if (Convert2DTo1D(tile2D[0] - 1, tile2D[1] - 1) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0] - 1, tile2D[1] - 1));

        if (Convert2DTo1D(tile2D[0] - 1, tile2D[1]) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0] - 1, tile2D[1]));

        if (Convert2DTo1D(tile2D[0] - 1, tile2D[1] + 1) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0] - 1, tile2D[1] + 1));

        if (Convert2DTo1D(tile2D[0], tile2D[1] - 1) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0], tile2D[1] - 1));

        if (Convert2DTo1D(tile2D[0], tile2D[1] + 1) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0], tile2D[1] + 1));

        if (Convert2DTo1D(tile2D[0] + 1, tile2D[1] - 1) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0] + 1, tile2D[1] - 1));

        if (Convert2DTo1D(tile2D[0] + 1, tile2D[1]) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0] + 1, tile2D[1]));

        if (Convert2DTo1D(tile2D[0] + 1, tile2D[1] + 1) != -1)
            adjacentTiles.Add(Convert2DTo1D(tile2D[0] + 1, tile2D[1] + 1));

        return adjacentTiles.ToArray();
    }

    // Finds the 2D representation on the game board of a tile stored in the 1D array.
    int[] Convert1DTo2D(int element)
    {
        int[] tile = new int[2];
        int gridLength = (int)(Mathf.Sqrt(tiles.Length));

        if ((element < 0) || (element >= tiles.Length))
        {
            tile[0] = -1;
            tile[1] = -1;
            return tile;
        }

        tile[0] = (int)(element / gridLength);
        tile[1] = (int)(element % gridLength);
        return tile;
    }

    // Resets the 2D representation of a tile on the game board to its position in the 1D array.
    int Convert2DTo1D(int row, int column)
    {
        int tile = 0;
        int gridLength = (int)(Mathf.Sqrt(tiles.Length));

        if ((row < 0) || (row >= gridLength) || (column < 0) || (column >= gridLength))
            return -1;

        tile = (row * gridLength) + column;
        return tile;
    }

    // Recursive algorithm to check all the tiles around one that has no adjacent crimes. It will change 
    // the sprites for the adjacent tiles, and while any of them have no adjacent crimes, keep checking.
    void FindBorderTiles(int tile)
    {
        int[] adjacentTiles = AdjacentTiles(tile);

        // Check for each of the adjacent tiles.
        foreach (int i in adjacentTiles)
        {
            // If a tile is already clicked, move on.
            if (!tiles[i].GetComponent<Tile>().clicked)
            {
                // Set sprite for tile to relevant sprite.
                FindAdjacentCrimes(i);
                tiles[i].GetComponent<SpriteRenderer>().sprite = sprites[tiles[i].GetComponent<Tile>().adjacentCrimes];
                tiles[i].GetComponent<Tile>().clicked = true;

                // If this tile has no adjacent crimes, check its adjacent tiles recursively.
                if (tiles[i].GetComponent<Tile>().adjacentCrimes == 0)
                    FindBorderTiles(i);
            }
        }
    }
}