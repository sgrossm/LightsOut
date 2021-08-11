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

    // has the game been started yet?
    private bool gameStarted = false;

    // variables to handle numMoves/timer/difficulty
    private int _numMoves = 0;
    private float _startTime;
    private float _difficulty = 0.15f; // default is easy

    // Keep track of number of off lights
    private int _numOffLights;

    // Create audio manager for sound playback
    private AudioManager _audioManager;

    // Declare text and buttons - set in Inspector
    [SerializeField] private Button _playGameBtn;
    [SerializeField] private Button _retryBtn;
    [SerializeField] private Text _numMovesText;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _gameText;
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Dropdown _difficultyDrop;

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
                _buttonGrid[i, j].interactable = false;
            }
        }
    }

    private void Start()
    {
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
        _numMovesText.gameObject.SetActive(true);
        _timeText.gameObject.SetActive(true);
        _retryBtn.gameObject.SetActive(true);

        _gameText.gameObject.SetActive(false);
        _playGameBtn.gameObject.SetActive(false);
        _difficultyDrop.gameObject.SetActive(false);
        _startTime = Time.time;
        _timeText.text = _startTime.ToString();
        _numMoves = 0;
        _numMovesText.text = "Moves: " + _numMoves;
    }

    private void initGrid()
    {
        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // initialize all lights to off
                _buttonGrid[i, j].image.color = _offColor;
                _buttonGrid[i, j].interactable = true;
            }
        }
        _numOffLights = SIZE * SIZE;
    }

    private void createRandomSolvableGrid()
    {
        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // randomly turn on lights using toggleLight()                
                if (Random.Range(0.0f, 1.0f) < _difficulty)
                {
                    // Using this funcion ensures the game will be solvable
                    toggleLight(i, j);
                }
            }
        }
    }

    private void Update()
    {
        // As long as the number of off lights is not equal to the grid size
        // keep updating the clock
        if (_numOffLights != SIZE * SIZE && gameStarted)
        {
            UpdateTime();
        }
    }

    private void UpdateTime()
    {
        // Get amount of time elapsed
        float elapsed = Time.time - _startTime;
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
                    _numMoves++;
                    _numMovesText.text = "Moves: " + _numMoves;

                    // check if the game is over
                    if (isGameFinished())
                    {
                        gameIsOver();
                    }
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
        // otherwise turn it on and update number of off lights
        if (toFlip.color.Equals(_onColor))
        {
            toFlip.color = _offColor;
            _numOffLights += 1;
        }

        else
        {
            toFlip.color = _onColor;
            _numOffLights -= 1;
        }            

    }


    private bool isGameFinished()
    {
        //check that all lights are off
        return (_numOffLights == SIZE * SIZE);
    }

    private void gameIsOver()
    {
        _audioManager.StopMusic();
        _audioManager.PlayYouWinSound();

        // Display game over text and restart button
        _gameText.text = "You win!";
        _playGameBtn.GetComponentInChildren<Text>().text = "Restart";
        ShowStartScreen();
    }

    public void GameReset()
    {
        // clear grid
        initGrid(); 

        // Set audio            
        _audioManager.PlayMainMenuSound();
        _audioManager.SetMenuMusic();
        
        // Set initial game screen text 
        _gameText.text = "Lights Out!";
        _playGameBtn.GetComponentInChildren<Text>().text = "Start";
        ShowStartScreen();
    }

    private void ShowStartScreen()
    {
        // Makes start screen game objects interactable
        gameStarted = false;
        _gameText.gameObject.SetActive(true);
        _playGameBtn.gameObject.SetActive(true);
        _difficultyDrop.gameObject.SetActive(true);

        _retryBtn.gameObject.SetActive(false);
        _numMovesText.gameObject.SetActive(false);
        _timeText.gameObject.SetActive(false);

        for (int i = 0; i < SIZE; ++i)
        {
            for (int j = 0; j < SIZE; ++j)
            {
                // Make grid buttons non-interactive
                _buttonGrid[i, j].interactable = false;
            }
        }
    }

    public void SetDifficulty()
    {
        // Set in inspector
        _difficultyDrop.RefreshShownValue();
        // 0 - easy, 2 - medium, 2 - hard
        if (_difficultyDrop.value == 0)
            _difficulty = 0.15f;
        else if (_difficultyDrop.value == 1)
            _difficulty = 0.50f;
        else if (_difficultyDrop.value == 2)
            _difficulty = 0.90f;
        else
            Debug.LogWarning("Difficulty is not set");
        _audioManager.PlaySetDifficultyGameSound();
    }

    public void StartGame()
    {
        // OnClick event handler for (Re)start button - set in Inspector
        _audioManager.PlayStartGameSound();
        _audioManager.SetGameplayMusic();
        gameStarted = true;

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
        _numOffLights = 0;
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
