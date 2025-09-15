using System.Collections.Generic;
using System.Collections;
using Sigmoid.Utilities;
using Sigmoid.Audio;
using Sigmoid.Game;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Sigmoid.UI
{
    /// <summary>
    /// An incredibly complex system for simply... loading some text<br/>
    /// (This was such a waste of time)
    /// </summary>
	public class CLISimulator : Singleton<CLISimulator>
	{
        [field: SerializeField] public ScriptableAudio TypingNoise { get; private set; }
        [field: SerializeField] public ScriptableAudio ErrorNoise { get; private set; }
        [field: SerializeField] public ScriptableAudio SuccessNoise { get; private set; }
        [SerializeField] private AudioPlayer audioPlayer;

        [SerializeField] private List<GameObject> selectionButtons;
        [SerializeField] private TextMeshProUGUI hintMessage;
        [SerializeField] private TextMeshProUGUI textMesh;
        private const int MAX_LINES = 10;

        private CLILine[] mainSequence;
        private CLILine[] interruptSequence;
        private CLILine[] selectionSequence;
        private CLILine[] StartSequence
        {
            get
            {
                CLIColour preferredColour = chosenKey == 1 ? CLIColour.Special : chosenKey == 2 ? CLIColour.Output : chosenKey == 3 ? CLIColour.Warning : CLIColour.Error;
                string difficultyString = chosenKey == 2 ? "Rookie" : chosenKey == 3 ? "Skilled" : "Veteran";
                string emoticonString = chosenKey == 1 ? ":D" : chosenKey == 2 ? ":)" : chosenKey == 3 ? ":O" : ">:)";

                CLILine[] result = new CLILine[chosenKey == 1 || chosenKey == 3 ? 6 : 10];
                result[0]  = new(CLIOperation.New,       0.00f, "", preferredColour);
                result[1]  = new(CLIOperation.New,       0.20f, $"> Entering {(chosenKey == 1 ? "tutorial" : ("game on " + difficultyString))}", preferredColour);
                result[2]  = new(CLIOperation.Append,    0.20f, ".", preferredColour, CLISound.Type);
                result[3]  = new(CLIOperation.Append,    0.20f, ".", preferredColour, CLISound.Type);
                result[4]  = new(CLIOperation.Append,    0.20f, ".", preferredColour, CLISound.Type);
                result[^1] = new(CLIOperation.New,       0.60f, $"Good Luck! {emoticonString}", preferredColour, CLISound.Success);

                if(chosenKey == 2)
                {
                    result[5] = new(CLIOperation.New, 0.20f, "Enemy health -30%", preferredColour, CLISound.Type);
                    result[6] = new(CLIOperation.New, 0.10f, "Enemy speed -25%", preferredColour, CLISound.Type);
                    result[7] = new(CLIOperation.New, 0.15f, "Coins earned +40%", preferredColour, CLISound.Type);
                    result[8] = new(CLIOperation.New, 0.50f, "Special room chance increased", preferredColour, CLISound.Type);
                }
                else if(chosenKey == 4)
                {
                    result[5] = new(CLIOperation.New, 0.20f, "Enemy health +33%", preferredColour, CLISound.Type);
                    result[6] = new(CLIOperation.New, 0.10f, "Enemy speed +33%", preferredColour, CLISound.Type);
                    result[7] = new(CLIOperation.New, 0.15f, "Coins earned -25%", preferredColour, CLISound.Type);
                    result[8] = new(CLIOperation.New, 0.50f, "Puzzle difficulty increased", preferredColour, CLISound.Type);
                }

                return result;
            }
        }

        private void Awake()
        {
            int numFiles = Random.Range(50000, 65000);
            mainSequence = new CLILine[]
            {
                new(CLIOperation.New,       0.64f, "> Initialising session as 'root'...", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.00f, "> git clone https://lab/chemical-", CLIColour.Input, CLISound.Type),
                new(CLIOperation.New,       0.27f, "combat/environment.git", CLIColour.Input, CLISound.Type),
                new(CLIOperation.New,       0.07f, $"> Unpacking objects: 0/{numFiles}", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.03f, $"> Unpacking objects: {Random.Range(1500, 10000)}/{numFiles}", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.05f, $"> Unpacking objects: {Random.Range(12000, 21000)}/{numFiles}", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.06f, $"> Unpacking objects: {Random.Range(24000, 33000)}/{numFiles}", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.12f, $"> Unpacking objects: {Random.Range(36000, 47000)}/{numFiles}", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.30f, $"> Unpacking objects: {numFiles}/{numFiles}", CLIColour.Output, CLISound.Success),
                new(CLIOperation.New,       0.08f, "> dotnet --version"),
                new(CLIOperation.New,       0.00f, "> 8.0.4"),
                new(CLIOperation.New,       0.35f, ""),

                new(CLIOperation.New,       0.00f, "> dotnet build ./ChemicalCombat", CLIColour.Input, CLISound.Type),
                new(CLIOperation.New,       0.42f, ".csproj --production --debug-off", CLIColour.Input, CLISound.Type),
                new(CLIOperation.New,       0.14f,  "> Installing dependencies...", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.16f, "> - unity-runtime@2022.3", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.10f, "> - reactions-core@1.6.3", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.18f, "> - player-controller@2.5.4", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.08f, "> - enemy-ai@5.9.1", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.11f, "> - audio-system@3.0.8", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.33f, "> - lab-security@0.1.6", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.41f, "> - lab-security@0.1.6 NOT FOUND!", CLIColour.Error, CLISound.Error),
                new(CLIOperation.New,       0.17f, "> Files Missing: 1/6", CLIColour.Warning),
                new(CLIOperation.New,       0.00f,  "> Proceeding with installation..."),
                new(CLIOperation.New,       0.42f, ""),

                new(CLIOperation.New,       0.23f,  "> Linking assets...", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Append,    0.04f, " [success]", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.28f,  "> Compiling shaders...", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Append,    0.06f, " [success]", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.19f,  "> Verifying resources...", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Append,    0.07f, " [success]", CLIColour.Output, CLISound.Type),
                new(CLIOperation.New,       0.24f,  "> Resolving memory leaks...", CLIColour.Output, CLISound.Type),
                new(CLIOperation.Overwrite, 0.0f,  "> Resolving memory leaks...", CLIColour.Error),
                new(CLIOperation.New,       0.12f, "[failed after 0.231s]", CLIColour.Error, CLISound.Error),
                new(CLIOperation.New,       0.00f, "> Build completed in 1.23s with", CLIColour.Warning),
                new(CLIOperation.New,       0.00f, "status 'Partial Success'", CLIColour.Warning),
                new(CLIOperation.New,       0.36f, ""),

                new(CLIOperation.New,       0.13f, $"> authorise user {SteamManager.Instance.Username}", CLIColour.Input),
                new(CLIOperation.New,       0.34f,  "> Authorising..."),
                new(CLIOperation.Append,    0.05f, " Success!", CLIColour.Output, CLISound.Success),
                new(CLIOperation.New,       0.10f, "> cd ./lab-reports/subjects", CLIColour.Input),
                new(CLIOperation.New,       0.18f, "> mkdir ./new-run.json --unsafe", CLIColour.Input),
                new(CLIOperation.New,       0.00f, "> run ./difficulty-selector.bat", CLIColour.Input),
                new(CLIOperation.New,       0.16f, "")
            };
            interruptSequence = new CLILine[]
            {
                new(CLIOperation.New,       0.24f, "> WARNING: Unexpected input!", CLIColour.Error, CLISound.Error),
                new(CLIOperation.New,       0.00f, "> Skipping boot sequence...", CLIColour.Warning),
                new(CLIOperation.New,       0.15f, ""),
            };
            selectionSequence = new CLILine[]
            {
                new(CLIOperation.New,       0.40f, "> Please select a difficulty level:", CLIColour.Input),
                new(CLIOperation.New,       0.00f, $"> [1] Tutorial{(DifficultyManager.HasCompletedTutorial ? "" : " (recommended)")}", CLIColour.Special),
                new(CLIOperation.New,       0.00f, "> [2] Rookie", CLIColour.Output),
                new(CLIOperation.New,       0.00f, "> [3] Skilled", CLIColour.Warning),
                new(CLIOperation.New,       0.00f, "> [4] Veteran", CLIColour.Error)
            };
        }

        private int chosenKey = -1;
        private bool wantsToSkip;
        private bool isChoosing;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Mouse0)
            || Input.GetKeyDown(KeyCode.Mouse1)) wantsToSkip = true;
            if(!isChoosing) return;

            if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) Choose(1);
            else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) Choose(2);
            else if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) Choose(3);
            else if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) Choose(4);
        }

        /// <summary>
        /// Starts a game on a certain index (1 being the tutorial; 2, 3, and 4 being the difficulties)
        /// </summary>
        /// <param name="index"></param>
        public void Choose(int index)
        {
            chosenKey = index;
            isChoosing = false;
        }

        private Coroutine activeSequence;
        public void BootTerminal()
        {
            KillTerminal();
            activeSequence = StartCoroutine(BootTerminalAsync());
        }
        public void KillTerminal()
        {
            DOTween.Kill("BootSequence");
            hintMessage.alpha = 0f;
            if(activeSequence != null) StopCoroutine(activeSequence);
        }

        /// <summary>
        /// Plays the boot sequence and then allows the player to choose a difficulty
        /// </summary>
        /// <returns></returns>
        private IEnumerator BootTerminalAsync()
        {
            chosenKey = -1;
            wantsToSkip = false;
            isChoosing = false;

            hintMessage.DOFade(0.5f, 0.5f * Options.Current.AnimationTimeMultiplier).SetDelay(1.5f * Options.Current.AnimationTimeMultiplier).SetId("BootSequence");

            LinkedList<string> lines = new LinkedList<string>();
            yield return PlaySequenceAsync(lines, mainSequence, () => wantsToSkip);
            DOTween.Kill("BootSequence");
            hintMessage.DOFade(0f, 0.5f * Options.Current.AnimationTimeMultiplier).SetId("BootSequence");

            if(wantsToSkip) yield return PlaySequenceAsync(lines, interruptSequence, () => false);
            yield return PlaySequenceAsync(lines, selectionSequence, () => false);

            RectTransform tutorialButton = (RectTransform) selectionButtons[0].transform;
            tutorialButton.sizeDelta = new Vector2(DifficultyManager.HasCompletedTutorial ? 70f : 154f, 10f);

            isChoosing = true;
            foreach(GameObject obj in selectionButtons)
                obj.SetActive(true);

            yield return new WaitWhile(() => isChoosing);
            foreach(GameObject obj in selectionButtons)
                obj.SetActive(false);

            yield return PlaySequenceAsync(lines, StartSequence, () => false);
            if(chosenKey == 1) DifficultySelector.Instance.StartTutorial();
            else if(chosenKey == 2) DifficultySelector.Instance.ChooseDifficulty(Difficulty.Rookie);
            else if(chosenKey == 3) DifficultySelector.Instance.ChooseDifficulty(Difficulty.Skilled);
            else if(chosenKey == 4) DifficultySelector.Instance.ChooseDifficulty(Difficulty.Veteran);
        }

        /// <summary>
        /// Loops through all lines of a sequence and adds them to the LinkedList
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="sequence"></param>
        /// <param name="interrupt"></param>
        /// <returns></returns>
        private IEnumerator PlaySequenceAsync(LinkedList<string> lines, CLILine[] sequence, System.Func<bool> interrupt)
        {
            foreach(CLILine line in sequence)
            {
                if(line.operation == CLIOperation.New)
                {
                    lines.AddLast(line.Line);
                    if(lines.Count > MAX_LINES)
                        lines.RemoveFirst();
                }
                else if(line.operation == CLIOperation.Overwrite) lines.Last.Value = line.Line;
                else lines.Last.Value += line.Line;

                if(line.sound != CLISound.None)
                    audioPlayer.Play(line.sound.GetSound(), AudioChannel.UI);

                if(interrupt())
                    yield break;

                textMesh.SetText(lines.BuildText());
                yield return new WaitForSeconds(line.duration * Options.Current.AnimationTimeMultiplier);
            }
        }

        /// <summary>
        /// Called when the menu is closed unexpectedly
        /// </summary>
        public void Cancel()
        {
            if(activeSequence != null) StopCoroutine(activeSequence);
            wantsToSkip = false;
            isChoosing = false;

            foreach(GameObject obj in selectionButtons)
                obj.SetActive(false);
        }
	}

    /// <summary>
    /// Represents a single line in a terminal animation
    /// </summary>
    public struct CLILine
    {
        public CLIOperation operation;
        public float duration;
        private readonly string line;
        public readonly string Line => $"<color={colour.GetHex()}>{line}</color>";
        public CLIColour colour;
        public CLISound sound;

        public CLILine(CLIOperation operation, float duration, string line, CLIColour colour = CLIColour.Output, CLISound sound = CLISound.None)
        {
            this.operation = operation;
            this.duration = duration;
            this.line = line;
            this.colour = colour;
            this.sound = sound;
        }
    }

    /// <summary>
    /// Controls what this line will do to the current text
    /// </summary>
    public enum CLIOperation
    {
        New,
        Overwrite,
        Append
    }

    /// <summary>
    /// A preset list of colours that can be used for displaying text in the CLI
    /// </summary>
    public enum CLIColour
    {
        Input,
        Output,
        Warning,
        Error,
        Special
    }

    /// <summary>
    /// A sound effect that will play when a line is started
    /// </summary>
    public enum CLISound
    {
        None,
        Type,
        Error,
        Success
    }

    public static class CLIHelper
    {
        /// <summary>
        /// Gets the desired hex code for a given message type
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        public static string GetHex(this CLIColour colour) => colour switch
        {
            CLIColour.Warning => "#F0CA4B",
            CLIColour.Error => "#FA4661",
            CLIColour.Special => "#76E8D7",
            CLIColour.Input => "#D5DEE0",
            _ => "#61ED6F"
        };

        public static ScriptableAudio GetSound(this CLISound sound) => sound switch
        {
            CLISound.Type => CLISimulator.Instance.TypingNoise,
            CLISound.Error => CLISimulator.Instance.ErrorNoise,
            CLISound.Success => CLISimulator.Instance.SuccessNoise,
            _ => null
        };
    }
}
