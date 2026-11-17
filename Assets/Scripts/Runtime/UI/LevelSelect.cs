using System;
using SteppeBastion.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SteppeBastion.UI
{
    public class LevelSelect : MonoBehaviour
    {
        [Serializable]
        public class Card
        {
            public Button button;
            public Image[] stars;
            public GameObject lockedOverlay;
        }

        [SerializeField] private Card[] cards;
        [SerializeField] private Color earnedStar = new(1f, 0.85f, 0.4f);
        [SerializeField] private Color emptyStar = new(0.17f, 0.11f, 0.07f, 0.6f);

        private void Awake()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                string scene = LevelProgress.Levels[i].scene;
                cards[i].button.onClick.AddListener(() => SceneLoader.Load(scene));
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                bool unlocked = LevelProgress.IsUnlocked(i);
                int stars = LevelProgress.GetStars(LevelProgress.Levels[i].scene);
                cards[i].button.interactable = unlocked;
                cards[i].lockedOverlay.SetActive(!unlocked);
                for (int s = 0; s < cards[i].stars.Length; s++)
                {
                    cards[i].stars[s].gameObject.SetActive(unlocked);
                    cards[i].stars[s].color = s < stars ? earnedStar : emptyStar;
                }
            }
        }

        public Selectable FirstCard => cards[0].button;
    }
}
