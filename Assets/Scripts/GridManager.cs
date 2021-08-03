using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    private const int SIZE = 5;
    // Set color for lights on/off
    private static Color _onColor = Color.white;
    private static Color _offColor = Color.black;

    // Initialize button grid
    private static Button[,] _buttonGrid = new Button[SIZE, SIZE];

    // variables to handle numMoves/timer
    private int numMoves = 0;
    private float startTime;

    // Create audio manager for sound playback
    private AudioManager _audioManager;

    // check if game is over
    private bool gameDoneTextDisplayed = false;

    // Declare text and buttons - set in Inspector
    [SerializeField] private Text _numMovesText;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Button _restartGame;
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Slider _volumeSlider;

    private void Awake()
    {
        // Get all buttons in grid
        Button[] buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // Create 2D button grid from 1D array
                _buttonGrid[i, j] = buttons[i * SIZE + j];
            }
        }
    }

    private void Start()
    {
        initializeGame();
        _audioManager = GetComponent<AudioManager>();
    }

    private void initializeGame()
    {
        initText();
        initGrid();
        createRandomSolvableGrid();
    }

    private void initText()
    {
        // Initialize ui objects 
        _gameOverText.gameObject.SetActive(false);
        _restartGame.gameObject.SetActive(false);
        gameDoneTextDisplayed = false;
        startTime = Time.time;
        _timeText.text = startTime.ToString();
        numMoves = 0;
        _numMovesText.text = "Moves: " + numMoves;
    }

    private void initGrid()
    {
        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // initialize all lights to off
                _buttonGrid[i, j].image.color = _offColor;
            }
        }
    }

    private void createRandomSolvableGrid()
    {
        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // randomly turn on lights using toggleLight()                
                if (Random.Range(0.0f, 1.0f) < 0.25f)
                {
                    // Using this funcion ensures the game will be solvable
                    toggleLight(i, j);
                }
            }
        }

    }

    private void Update()
    {

        if (!isGameFinished())
        {
            // As long as the game is not over,
            // keep updating the timer
            UpdateTime();
        }

        else
        {
            // Game is over
            if (!gameDoneTextDisplayed)
                gameIsOver();
        }
    }

    private void UpdateTime()
    {
        // Get amount of time elapsed
        float elapsed = Time.time - startTime;
        // Convert to min/sec
        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed - minutes * 60);
        // Format time string to show min/sec with leading zeroes
        string timeRemaining = string.Format("{0:00}:{1:00}", minutes, seconds);
        _timeText.text = timeRemaining;
    }

    public void updateButtons()
    {
        // OnClick event handle for buttons - set in Unity Inspector
        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // Iterate through buttons and find currently selected/clicked button
                if (EventSystem.current.currentSelectedGameObject == _buttonGrid[i, j].gameObject)
                {
                    // toggle lights
                    toggleLight(i, j);

                    // Play sound of flippin light
                    _audioManager.PlayFlipSound();

                    // update number of moves
                    numMoves++;
                    _numMovesText.text = "Moves: " + numMoves;

                }
            }
        }
    }

    private void toggleLight(int x, int y)
    {
        // Always flip current selected cell
        flipColor(x, y);

        // Are we a special case?
        if (x == 0 || y == 0 || x == SIZE - 1 || y == SIZE - 1)
        {
            //upper left corner
            if (x == 0 && y == 0)
            {
                // flip right and below
                flipColor(y, x + 1);
                flipColor(y + 1, x);
                return;
            }

            // upper right corner
            else if (x == SIZE - 1 && y == 0)
            {
                // flip left and below
                flipColor(x - 1, y);
                flipColor(x, y + 1);
                return;
            }

            // bottom left corner
            else if (x == 0 && y == SIZE - 1)
            {
                // flip above and right
                flipColor(x + 1, y);
                flipColor(x, y - 1);
                return;
            }

            // bottom right corner
            else if (x == SIZE - 1 && y == SIZE - 1)
            {
                // flip above and left
                flipColor(x - 1, y);
                flipColor(x, y - 1);
                return;
            }

            // if we are in middle of top row
            else if (y == 0)
            {
                // flip left, right, below
                flipColor(x - 1, y);
                flipColor(x + 1, y);
                flipColor(x, y + 1);
                return;
            }

            // if we are in middle of leftmost column
            else if (x == 0)
            {
                // flip above, right, below
                flipColor(x + 1, y);
                flipColor(x, y + 1);
                flipColor(x, y - 1);
                return;
            }

            // if we are in middle of rightmost column
            else if (x == SIZE - 1)
            {
                // flip above, below, left
                flipColor(x - 1, y);
                flipColor(x, y + 1);
                flipColor(x, y - 1);
                return;
            }

            // if we are in middle of bottom row
            else if (y == SIZE - 1)
            {
                // flip left, above, right
                flipColor(x - 1, y);
                flipColor(x + 1, y);
                flipColor(x, y - 1);
                return;
            }
        }

        // General case
        else
        {
            // flip left, right, above, below
            flipColor(x - 1, y);
            flipColor(x + 1, y);
            flipColor(x, y + 1);
            flipColor(x, y - 1);
            return;
        }
    }

    private void flipColor(int x, int y)
    {
        Image toFlip = _buttonGrid[x, y].image;

        // if the current button's light is on, turn if off
        // otherwise turn it on
        if (toFlip.color.Equals(_onColor))
            toFlip.color = _offColor;

        else
            toFlip.color = _onColor;

    }


    private bool isGameFinished()
    {
        //check that all lights are off
        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // if any lights are on, we aren't done
                if (_buttonGrid[i, j].image.color.Equals(_onColor))
                    return false;
            }
        }

        // no lights are on, game is over!
        return true;
    }

    private void gameIsOver()
    {
        // Display game over text and restart button
        _gameOverText.gameObject.SetActive(true);
        _restartGame.gameObject.SetActive(true);
        _soundToggle.interactable = false;
        _volumeSlider.interactable = false;

        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // Make grid buttons non-interactive
                _buttonGrid[i, j].interactable = false;
            }
        }

        gameDoneTextDisplayed = true;
    }

    public void restartGame()
    {
        // OnClick event handler for Restart button - set in Inspector
        _audioManager.PlayRestartGameSound();

        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // make buttons interactable again
                _buttonGrid[i, j].interactable = true;
            }
        }

        _soundToggle.interactable = true;
        _volumeSlider.interactable = true;

        // reinitialize game state
        initializeGame();
    }

    public void toggleAudio()
    {
        // used with checkkbox to mute/unmute audio - set in inspector
        _audioManager.ToggleSound(_soundToggle.isOn);
    }

    public void setVolume()
    {
        // used with volume slider to smoothly set output volume - set in inspector
        _audioManager.mainMixer.SetFloat("MainVolume", Mathf.Log10(_volumeSlider.value) * 20);
    }
}
