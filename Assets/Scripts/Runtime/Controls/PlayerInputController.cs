using System;
using SteppeBastion.Core;
using SteppeBastion.Heroes;
using SteppeBastion.Towers;
using SteppeBastion.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SteppeBastion.Controls
{
    public enum InputMode
    {
        Normal,      // clicking a build spot opens the build menu
        AimBerkut,   // next click sends the berkut
        MoveTower    // next click on a free spot moves the selected tower (Kosh)
    }

    /// <summary>Reads mouse and keyboard through the Input System and turns clicks into game actions.</summary>
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private BuildMenu buildMenu;
        [SerializeField] private BerkutAbility berkut;
        [SerializeField] private WaveSpawner spawner;

        private InputAction point;
        private InputAction click;
        private InputAction cancel;
        private InputAction callWave;
        private InputAction toggleSpeed;
        private InputAction useBerkut;
        private InputAction restart;

        private BuildSpot[] spots;
        private Tower movingTower;

        public InputMode Mode { get; private set; }
        public event Action<InputMode> ModeChanged;

        private void Awake()
        {
            point = new InputAction("Point", InputActionType.Value, "<Pointer>/position");
            click = new InputAction("Click", InputActionType.Button, "<Pointer>/press");
            cancel = new InputAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
            callWave = new InputAction("CallWave", InputActionType.Button, "<Keyboard>/space");
            toggleSpeed = new InputAction("ToggleSpeed", InputActionType.Button, "<Keyboard>/f");
            useBerkut = new InputAction("Berkut", InputActionType.Button, "<Keyboard>/b");
            useBerkut.AddBinding("<Mouse>/rightButton");
            restart = new InputAction("Restart", InputActionType.Button, "<Keyboard>/r");

            spots = FindObjectsByType<BuildSpot>(FindObjectsSortMode.None);
        }

        private void Start() => LevelManager.Instance.StateChanged += OnStateChanged;

        private void OnStateChanged(GameState state)
        {
            buildMenu.Close();
            SetMode(InputMode.Normal);
            berkut.ShowAim(false, Vector3.zero);
        }

        private void OnEnable()
        {
            foreach (InputAction action in Actions) action.Enable();
        }

        private void OnDisable()
        {
            foreach (InputAction action in Actions) action.Disable();
        }

        private InputAction[] Actions => new[] { point, click, cancel, callWave, toggleSpeed, useBerkut, restart };

        private void Update()
        {
            if (LevelManager.Instance.State != GameState.Playing)
            {
                if (restart.WasPressedThisFrame()) SceneLoader.RestartLevel();
                return;
            }

            if (cancel.WasPressedThisFrame())
            {
                if (Mode != InputMode.Normal) SetMode(InputMode.Normal);
                else if (buildMenu.IsOpen) buildMenu.Close();
                else GameSpeed.SetPaused(!GameSpeed.Paused);
                return;
            }

            if (GameSpeed.Paused) return;

            // Keep clicked HUD buttons from staying selected, otherwise Space would also "press" them.
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

            if (callWave.WasPressedThisFrame()) spawner.CallEarly();
            if (toggleSpeed.WasPressedThisFrame()) GameSpeed.ToggleFast();
            if (useBerkut.WasPressedThisFrame())
            {
                if (Mode == InputMode.AimBerkut) HandleClick(PointerWorld());
                else BeginBerkut();
            }

            if (click.WasPressedThisFrame() && !IsPointerOverUI())
                HandleClick(PointerWorld());

            berkut.ShowAim(Mode == InputMode.AimBerkut, PointerWorld());
        }

        public void BeginBerkut()
        {
            if (!berkut.Ready) return;
            buildMenu.Close();
            SetMode(InputMode.AimBerkut);
        }

        public void BeginMove(Tower tower)
        {
            movingTower = tower;
            buildMenu.Close();
            SetMode(InputMode.MoveTower);
        }

        private void HandleClick(Vector3 world)
        {
            switch (Mode)
            {
                case InputMode.AimBerkut:
                    berkut.Strike(world);
                    SetMode(InputMode.Normal);
                    break;

                case InputMode.MoveTower:
                    BuildSpot target = SpotAt(world);
                    if (target != null && target.IsFree && movingTower != null)
                        movingTower.MoveTo(target);
                    SetMode(InputMode.Normal);
                    break;

                default:
                    BuildSpot spot = SpotAt(world);
                    if (spot != null) buildMenu.Open(spot);
                    else buildMenu.Close();
                    break;
            }
        }

        private void SetMode(InputMode mode)
        {
            Mode = mode;
            if (mode != InputMode.MoveTower) movingTower = null;
            foreach (BuildSpot spot in spots)
                spot.SetHighlight(mode == InputMode.MoveTower && spot.IsFree);
            ModeChanged?.Invoke(mode);
        }

        private Vector3 PointerWorld()
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(point.ReadValue<Vector2>());
            world.z = 0f;
            return world;
        }

        private static BuildSpot SpotAt(Vector3 world)
        {
            Collider2D hit = Physics2D.OverlapPoint(world);
            return hit != null ? hit.GetComponent<BuildSpot>() : null;
        }

        private static bool IsPointerOverUI() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
