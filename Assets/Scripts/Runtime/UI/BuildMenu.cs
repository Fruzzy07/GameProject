using System;
using System.Collections.Generic;
using SteppeBastion.Audio;
using SteppeBastion.Controls;
using SteppeBastion.Core;
using SteppeBastion.Data;
using SteppeBastion.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    /// <summary>Radial menu over a build spot: build a tower on a free spot, or upgrade / move / sell an existing one.</summary>
    public class BuildMenu : MonoBehaviour
    {
        private const float Radius = 115f;
        private const float ButtonSize = 104f;

        [SerializeField] private Camera worldCamera;
        [SerializeField] private RectTransform root;
        [SerializeField] private RectTransform buttonsRoot;
        [SerializeField] private Text infoText;
        [SerializeField] private PlayerInputController controls;
        [SerializeField] private Sprite circleSprite;
        [SerializeField] private Sprite ringSprite;

        private readonly List<(Button button, Func<bool> canUse)> buttons = new();
        private BuildSpot spot;

        public bool IsOpen => spot != null;

        private void Awake() => root.gameObject.SetActive(false);

        public void Open(BuildSpot target)
        {
            Close();
            spot = target;
            root.gameObject.SetActive(true);
            if (spot.IsFree) BuildTowerButtons();
            else BuildTowerActions(spot.Tower);
            LateUpdate();
        }

        public void Close()
        {
            if (spot != null && spot.Tower != null) spot.Tower.ShowRange(false);
            spot = null;
            foreach (var (button, _) in buttons) Destroy(button.gameObject);
            buttons.Clear();
            root.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (spot == null) return;
            // Screen Space Overlay canvas: world position of a UI element is in screen pixels.
            root.position = worldCamera.WorldToScreenPoint(spot.transform.position + Vector3.up * 0.5f);
            foreach (var (button, canUse) in buttons)
                button.interactable = canUse();
        }

        private void BuildTowerButtons()
        {
            TowerData[] towers = LevelManager.Instance.Level.towers;
            infoText.text = "Build a tower";
            for (int i = 0; i < towers.Length; i++)
            {
                TowerData data = towers[i];
                float angle = (135f - i * 90f) * Mathf.Deg2Rad; // top-left, top-right, bottom-right, bottom-left
                Vector2 position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Radius;
                Button button = RoundButton(position, data.BuildCost.ToString(), data.sprite,
                    () => LevelManager.Instance.CanAfford(data.BuildCost),
                    () =>
                    {
                        if (LevelManager.Instance.TrySpend(data.BuildCost))
                        {
                            spot.Build(data);
                            GameAudio.Play(s => s.build);
                        }
                        Close();
                    });
                AddHover(button, $"{data.displayName}\n<size=18>{data.description}</size>");
            }
        }

        private void BuildTowerActions(Tower tower)
        {
            tower.ShowRange(true);
            string level = tower.CanUpgrade ? $"Level {tower.Level + 1}" : "Max level";
            infoText.text = $"{tower.Data.displayName} · {level}";

            if (tower.CanUpgrade)
            {
                RoundButton(new Vector2(0f, Radius), $"Upgrade\n{tower.UpgradeCost}", null,
                    () => LevelManager.Instance.CanAfford(tower.UpgradeCost),
                    () =>
                    {
                        if (LevelManager.Instance.TrySpend(tower.UpgradeCost))
                        {
                            tower.Upgrade();
                            GameAudio.Play(s => s.upgrade);
                        }
                        Open(spot);
                    });
            }

            Button move = RoundButton(new Vector2(-Radius, -20f), "Kosh\nmove", null,
                () => !tower.IsPacked,
                () => controls.BeginMove(tower));
            AddHover(move, $"Kosh\n<size=18>Pack the tower like a yurt and move it to a free spot. It rests for {Tower.PackTime:0} s.</size>");

            RoundButton(new Vector2(Radius, -20f), $"Sell\n+{tower.SellValue}", null,
                () => true,
                () =>
                {
                    tower.Sell();
                    GameAudio.Play(s => s.sell);
                    Close();
                });
        }

        private Button RoundButton(Vector2 position, string label, Sprite icon, Func<bool> canUse, Action onClick)
        {
            Button button = UIFactory.RoundButton(buttonsRoot, "Button", new Vector2(0.5f, 0.5f), position, ButtonSize, circleSprite, ringSprite);
            Transform background = button.transform;

            if (icon != null)
            {
                UIFactory.Image(background, "Icon", icon, Color.white,
                    new Vector2(0.5f, 0.5f), new Vector2(0f, 8f), Vector2.one * (ButtonSize * 0.8f)).raycastTarget = false;
                UIFactory.Text(background, "Cost", label, 22, UIFactory.Gold, TextAnchor.MiddleCenter,
                    new Vector2(0.5f, 0f), new Vector2(0f, 4f), new Vector2(ButtonSize, 30f));
            }
            else
            {
                UIFactory.Text(background, "Label", label, 20, UIFactory.Cream, TextAnchor.MiddleCenter,
                    new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.one * ButtonSize);
            }

            button.onClick.AddListener(() => onClick());
            buttons.Add((button, canUse));
            return button;
        }

        private void AddHover(Button button, string text)
        {
            string normal = infoText.text;
            var hover = button.gameObject.AddComponent<HoverTrigger>();
            hover.Entered += () => infoText.text = text;
            hover.Exited += () => infoText.text = normal;
        }
    }
}
