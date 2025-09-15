using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Chemicals;
using Sigmoid.Enemies;
using Sigmoid.Players;
using Sigmoid.Cameras;
using Sigmoid.Weapons;
using Sigmoid.UI;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Sigmoid.Game
{
	public class DeathScreen : Singleton<DeathScreen>
	{
        private static WaitForSeconds _waitForSeconds0_4 = new WaitForSeconds(0.4f);
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject menuObject;
        [SerializeField] private RectTransform reportRect;
        [SerializeField] private TextMeshProUGUI caseTitle;
        [SerializeField] private TextMeshProUGUI overviewContents;
        [SerializeField] private TextMeshProUGUI performanceContents;
        [SerializeField] private TextMeshProUGUI resourcesContents;
        [SerializeField] private TextMeshProUGUI conclusionContents;
        [SerializeField] private RawImage deathScreenshot;
        [SerializeField] private Image chemicalIcon;
        [SerializeField] private Image miniPerkTree;
        [SerializeField] private Image[] chemicalBlockers;

		private void Awake() => Player.Instance.OnDeath += ShowDeathScreen; //unsubscribed automatically when unloading the Player
        private void ShowDeathScreen()
        {
            if(SceneLoader.Instance.CurrentScene == GameScene.Tutorial) return;

            menuObject.SetActive(true);
            PlayerUI.Instance.Hide();
            canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

            GenerateCaseID();
            PopulateOverview();
            PopulatePerformance();
            PopulateResources();
            ChooseMessage();
            InsertScreenshot();

            Sequence sequence = DOTween.Sequence();
            sequence.SetId("DeathScreen");
            sequence.SetUpdate(true);

            sequence.Insert(0f, MainCamera.Instance.Camera.DOOrthoSize(3f, 0.8f).SetEase(Ease.OutCubic));
            sequence.Insert(0.6f, reportRect.DOAnchorPos(new Vector2(0f, 0f), 0.6f).SetEase(Ease.OutQuint));
            sequence.Insert(0.4f, canvasGroup.DOFade(1f, 0.6f).SetEase(Ease.OutSine));
            sequence.OnComplete(() => Time.timeScale = 0f);
        }

        private static readonly string[] caseSuffixes = new string[]{"A", "B", "C", "D", "E", "F", "G", "K", "S", "T", "U", "X", "Y", "Z"};
        private void GenerateCaseID()
        {
            int randomNumber = Random.Range(1000, 10000);
            int randomIndex = Random.Range(0, 10);
            string caseID = randomNumber.ToString() + caseSuffixes[randomIndex];
            caseTitle.SetText($"CASE #{caseID}");
        }

        private void PopulateOverview()
        {
            string causeOfDeath = Player.Instance.LastDamager.DisplayName;
            string floorReached = (FloorManager.Instance.FloorNumber - 1).ToString();
            string timeSurvived = TimeTracker.Instance.TimeSurvived;
            string selectedDifficulty = DifficultyManager.Difficulty.ToString();
            overviewContents.SetText($"Cause: {causeOfDeath}\nFloor Reached: {floorReached}\nTime Survived: {timeSurvived}\nDifficulty: {selectedDifficulty}");
        }

        private void InsertScreenshot()
        {
            StartCoroutine(CInsertScreenshot());
            IEnumerator CInsertScreenshot()
            {
                yield return _waitForSeconds0_4;
                deathScreenshot.texture = DeathCamera.Instance.TakeScreenshot();
            }
        }



        private void PopulatePerformance()
        {
            Chemical favouriteElement = WeaponManager.Instance.GetMostUsedChemical();
            ScriptableChemical elementInfo = ChemicalManager.Get(favouriteElement);
            chemicalIcon.sprite = elementInfo.miniSprite;
            string elementColour = elementInfo.colours[5].GetHexCode();

            string numKills = KillCounter.Instance.Kills.ToString();
            string damageDealt = WeaponManager.Instance.TotalDamage.ToString();
            string highestDamage = WeaponManager.Instance.HighestDamage.ToString();
            performanceContents.SetText($"Enemies Killed: {numKills}\nDamage Dealt: {damageDealt}\nHighest Damage: {highestDamage}\nMost used Element:\n   <color={elementColour}>{favouriteElement}</color>");

            for(int i = 0; i < 16; i++)
                chemicalBlockers[i].enabled = !ChemicalManager.IsUnlocked(ChemicalManager.GetIndexed(i));
        }



        private void PopulateResources()
        {
            string earnings = CoinManager.Instance.Earnings.ToString();
            string expenditure = CoinManager.Instance.Expenditure.ToString();
            string chestsOpened = PersistentStats.ChestsOpened.ToString();
            string puzzlesSolved = PersistentStats.PuzzlesSolved.ToString();
            resourcesContents.SetText($"Earnings: {earnings}\nExpenditure: {expenditure}\nChests Opened: {chestsOpened}\nPuzzles Solved: {puzzlesSolved}");

            miniPerkTree.sprite = DrawMiniTree(miniPerkTree.sprite);
        }

        //this is literally the dumbest way of doing it, but idk what the formula is :sob:
        private static readonly int[] minXs = new int[]{0, 9, 19, 30, 39, 49};
        private static readonly int[] maxXs = new int[]{8, 18, 27, 38, 48, 57};
        private static readonly int[] minYs = new int[]{46, 37, 29, 18, 9, 1};
        private static readonly int[] maxYs = new int[]{54, 45, 36, 26, 17, 8};

        [SerializeField] private Color[] perkTreeReplacements;

        /// <summary>
        /// Replaces a bunch of colours based on some dodgy coordinate stuff<br/>
        /// Won't work AT ALL if the texture is changed in ANY way
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Sprite DrawMiniTree(Sprite original)
        {
            int startX = (int) original.rect.x;
            int startY = (int) original.rect.y;
            int width = (int) original.rect.width;
            int height = (int) original.rect.height;

            Color[] pixels = original.texture.GetPixels(startX, startY, width, height);
            foreach(TreeNode treeNode in PerkTree.Instance.IterateNodes())
            {
                if(treeNode.State == PurchaseState.Available) continue;

                int minX = minXs[treeNode.Index.x];
                int maxX = maxXs[treeNode.Index.x];
                int minY = minYs[treeNode.Index.y];
                int maxY = maxYs[treeNode.Index.y];

                for(int x = minX; x <= maxX; x++)
                {
                    for(int y = minY; y <= maxY; y++)
                    {
                        Color colour = pixels[y * width + x];
                        for(int i = 0; i < 4; i++)
                        {
                            if(colour == perkTreeReplacements[i + 1])
                                colour = treeNode.State == PurchaseState.Purchased ? treeNode.Colours[i]
                                : treeNode.DarkColours[i];
                        }
                        pixels[y * width + x] = colour;
                    }
                }
            }

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(pixels[y * width + x] == perkTreeReplacements[0])
                        pixels[y * width + x] = perkTreeReplacements[1];
                }
            }

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(pixels);
            texture.Apply();

            Rect rect = new Rect(0, 0, width, height);
            return Sprite.Create(texture, rect, original.pivot / original.rect.size, original.pixelsPerUnit);
        }



        private void ChooseMessage()
        {
            IDamageSource killer = Player.Instance.LastDamager;
            conclusionContents.SetText(DeathMessages.ChooseDeathMessage(killer));
        }

        public void Retry() => SceneLoader.Instance.RestartGame();
        public void Quit() => SceneLoader.Instance.ReturnToMenu();
    }
}
