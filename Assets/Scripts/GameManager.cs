using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public const int SIZE = 5;

    // 2D BitArray: 0 -> light off, 1 -> light on
    private BitArray[] _grid = new BitArray[SIZE];

    // Game Over property 
    private static bool gameOver = false;
    public static bool GameOver
    {
        private set { }
        get { return gameOver; }
    }



    // Delegate/event that updates UI when game is finished
    public delegate void GameOverDelegate();
    public static event GameOverDelegate GameOverEvent;

    private void OnEnable()
    {
        UIManager.updateButtonsEvent += toggleLight;
        UIManager.RestartPressedEvent += restartGame;
    }

    void Awake()
    {
        for (int i = 0; i < _grid.Length; ++i)
            _grid[i] = new BitArray(SIZE);

        initializeGrid();
    }

    private void Start()
    {
        createRandomSolvableGrid();
    }

    private void initializeGrid()
    {
        // Create empty board with all lights off
        for (int i = 0; i < _grid.Length; ++i)
        {
            for (int j = 0; j < _grid[i].Length; ++j)
            {
                _grid[i][j] = false;
            }
        }
    }

    private void createRandomSolvableGrid()
    {
        for (int i = 0; i < _grid.Length; ++i)
        {
            for (int j = 0; j < _grid[i].Length; ++j)
            {
                // randomly turn on lights using toggleLight()                
                if (Random.Range(0.0f, 1.0f) < 0.50f)
                {
                    // Using this funcion ensures the game will be solvable
                    toggleLight(i, j);
                }
            }
        }

    }

    void Update()
    {
        // keep checking for game to be finished
        if (isGameFinished()) { }
    }

    public void toggleLight(int x, int y)
    {
        // Are we a special case?
        if (x == 0 || y == 0 || x == SIZE - 1 || y == SIZE - 1)
        {
            //upper left corner
            if (x == 0 && y == 0)
            {
                // flip right and below
                _grid[y][x + 1] = !_grid[y][x + 1];
                _grid[y + 1][x] = !_grid[y + 1][x];
                UIManager.flipColor(y, x + 1, !_grid[y][x + 1]);
                UIManager.flipColor(y + 1, x, !_grid[y + 1][x]);
                return;
            }

            // upper right corner
            else if (x == SIZE - 1 && y == 0)
            {
                // flip left and below
                _grid[y][x - 1] = !_grid[y][x - 1];
                _grid[y + 1][x] = !_grid[y + 1][x];
                UIManager.flipColor(x - 1, y, !_grid[y][x - 1]);
                UIManager.flipColor(x, y + 1, !_grid[y + 1][x]);
                return;
            }

            // bottom left corner
            else if (x == 0 && y == SIZE - 1)
            {
                // flip above and right
                _grid[y][x + 1] = !_grid[y][x + 1];
                _grid[y - 1][x] = !_grid[y - 1][x];
                UIManager.flipColor(x + 1, y, !_grid[y][x + 1]);
                UIManager.flipColor(x, y - 1, !_grid[y - 1][x]);
                return;
            }

            // bottom right corner
            else if (x == SIZE - 1 && y == SIZE - 1)
            {
                // flip above and left
                _grid[y][x - 1] = !_grid[y][x - 1];
                _grid[y - 1][x] = !_grid[y - 1][x];
                UIManager.flipColor(x - 1, y, !_grid[y][x - 1]);
                UIManager.flipColor(x, y - 1, !_grid[y - 1][x]);
                return;
            }

            // if we are in middle of top row
            else if (y == 0)
            {
                // flip left, right, below
                _grid[y][x - 1] = !_grid[y][x - 1];
                _grid[y][x + 1] = !_grid[y][x + 1];
                _grid[y + 1][x] = !_grid[y + 1][x];
                UIManager.flipColor(x - 1, y, !_grid[y][x - 1]);
                UIManager.flipColor(x + 1, y, !_grid[y][x + 1]);
                UIManager.flipColor(x, y + 1, !_grid[y + 1][x]);
                return;
            }

            // if we are in middle of leftmost column
            else if (x == 0)
            {
                // flip above, right, below
                _grid[y][x + 1] = !_grid[y][x + 1];
                _grid[y + 1][x] = !_grid[y + 1][x];
                _grid[y - 1][x] = !_grid[y - 1][x];
                UIManager.flipColor(x + 1, y, !_grid[y][x + 1]);
                UIManager.flipColor(x, y + 1, !_grid[y + 1][x]);
                UIManager.flipColor(x, y - 1, !_grid[y - 1][x]);
                return;
            }

            // if we are in middle of rightmost column
            else if (x == SIZE - 1)
            {
                // flip above, below, left
                _grid[y][x - 1] = !_grid[y][x - 1];
                _grid[y + 1][x] = !_grid[y + 1][x];
                _grid[y - 1][x] = !_grid[y - 1][x];

                UIManager.flipColor(x - 1, y, !_grid[y][x - 1]);
                UIManager.flipColor(x, y + 1, !_grid[y + 1][x]);
                UIManager.flipColor(x, y - 1, !_grid[y - 1][x]);
                return;
            }

            // if we are in middle of bottom row
            else if (y == SIZE - 1)
            {
                // flip left, above, right
                _grid[y][x - 1] = !_grid[y][x - 1];
                _grid[y][x + 1] = !_grid[y][x + 1];
                _grid[y - 1][x] = !_grid[y - 1][x];

                UIManager.flipColor(x - 1, y, !_grid[y][x - 1]);
                UIManager.flipColor(x + 1, y, !_grid[y][x + 1]);
                UIManager.flipColor(x, y - 1, !_grid[y - 1][x]);
                return;
            }
        }

        // General case
        else
        {
            // flip left, right, above, below
            _grid[y][x - 1] = !_grid[y][x - 1];
            _grid[y][x + 1] = !_grid[y][x + 1];
            _grid[y + 1][x] = !_grid[y + 1][x];
            _grid[y - 1][x] = !_grid[y - 1][x];
            UIManager.flipColor(x - 1, y, !_grid[y][x - 1]);
            UIManager.flipColor(x + 1, y, !_grid[y][x + 1]);
            UIManager.flipColor(x, y + 1, !_grid[y + 1][x]);
            UIManager.flipColor(x, y - 1, !_grid[y - 1][x]);
            return;
        }
    }

    bool isGameFinished()
    {
        //check that all bits are false
        for (int i = 0; i < _grid.Length; ++i)
        {
            for (int j = 0; j < _grid[i].Length; ++j)
            {
                // if any lights are on, we aren't done
                if (_grid[i][j])
                    return false;
            }
        }

        // no lights are on, game is over!
        GameOverEvent();

        return true;
    }

    private void restartGame()
    {
        // restart game with new randomly solvable grid
        gameOver = false;
        initializeGrid();
        createRandomSolvableGrid();
    }

    private void OnDisable()
    {
        UIManager.updateButtonsEvent -= toggleLight;
        UIManager.RestartPressedEvent -= restartGame;
    }
}
