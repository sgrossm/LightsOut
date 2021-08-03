using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Set color for lights on/off
    private static Color _onColor = Color.white;
    private static Color _offColor = Color.black;

    // Get const grid size from Game Manager
    private static int gridSize = GameManager.SIZE;

    // Initialize button grid
    private static Button[,] _buttonGrid = new Button[gridSize, gridSize];

    // variables to handle numMoves/timer
    private int numMoves = 0;
    private float startTime;

    // Create audio manager for sound playback
    private AudioManager _audioManager;


    // Declare text and buttons - set in Inspector
    [SerializeField] private Text _numMovesText;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Button _restartGame;
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Slider _volumeSlider;

    // Thie delegate/event triggers when a button is clicked and toggles lights from GameManager
    public delegate void UpdateButtonsDelegate(int x, int y);
    public static event UpdateButtonsDelegate updateButtonsEvent;

    // This delegate/event triggers when restart button is pressed and reinitializes GameManager data
    public delegate void RestartPressed();
    public static event RestartPressed RestartPressedEvent;

    private void Awake()
    {
        // Get all buttons in grid
        Button[] buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                // Create 2D button grid from 1D array
                _buttonGrid[i, j] = buttons[i * gridSize + j];
            }
        }
    }

    private void OnEnable()
    {
        GameManager.GameOverEvent += gameOver;
    }

    private void Start()
    {
        initText();
        _audioManager = GetComponent<AudioManager>();
    }

    private void initText()
    {
        // Initialize text objects 
        _gameOverText.gameObject.SetActive(false);
        _restartGame.gameObject.SetActive(false);
        startTime = Time.time;
        _timeText.text = startTime.ToString();
        numMoves = 0;
        _numMovesText.text = "Moves: " + numMoves;
    }

    private void Update()
    {

        if (!GameManager.GameOver)
        {
            // As long as the game is not over,
            // keep updating the timer
            UpdateTime();
        }
    }

    public void UpdateTime()
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
        for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                // Iterate through buttons and find currently selected/clicked button
                if (EventSystem.current.currentSelectedGameObject == _buttonGrid[i, j].gameObject)
                {
                    // trigger event which will call ToggleLight in Game Manager
                    updateButtonsEvent(i, j);

                    // Play sound of flippin light
                    _audioManager.PlayFlipSound();

                    // update number of moves
                    numMoves++;
                    _numMovesText.text = "Moves: " + numMoves;

                }
            }
        }
    }

    public static void flipColor(int x, int y, bool on)
    {
        Image toFlip = _buttonGrid[x, y].image;

        // if the current button's light is on, turn if off
        // otherwise turn it on
        if (on)
            toFlip.color = _offColor;

        else
            toFlip.color = _onColor;
    }


    private void gameOver()
    {
        // Display game over text and restart button
        _gameOverText.gameObject.SetActive(true);
        _restartGame.gameObject.SetActive(true);

        _soundToggle.interactable = false;
        _volumeSlider.interactable = false;

        for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                // Make grid buttons non-interactive
                _buttonGrid[i, j].interactable = false;
            }
        }
    }

    public void restartGame()
    {
        _audioManager.PlayRestartGameSound();
        // OnClick event handler for Restart button - set in Inspector
        // Reinitialize number of moves and time text
        initText();

        for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                // make buttons interactable again
                _buttonGrid[i, j].interactable = true;
            }
        }

        _soundToggle.interactable = true;
        _volumeSlider.interactable = true;

        // Event which will reinitialize GameManager
        RestartPressedEvent();
    }

    public void toggleAudio()
    {
        _audioManager.ToggleSound(_soundToggle.isOn);
    }

    public void setVolume()
    {
        _audioManager.mainMixer.SetFloat("MainVolume", Mathf.Log10(_volumeSlider.value) * 20);
    }

    private void OnDisable()
    {
        GameManager.GameOverEvent -= gameOver;
    }
}
