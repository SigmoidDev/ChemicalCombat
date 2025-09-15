using System.Linq;
using System;
using Sigmoid.Generation;
using Sigmoid.Upgrading;
using Sigmoid.Reactions;
using Sigmoid.Players;
using Sigmoid.Enemies;
using Sigmoid.Weapons;
using Sigmoid.Rooms;
using UnityEngine;

namespace Sigmoid.Game
{
    public static class DeathMessages
    {
        private static DeathMessage[] AllDeathMessages => new DeathMessage[]
        {
            new(//If you somehow kill yourself in the home area
                source => SceneLoader.Instance.CurrentScene == GameScene.Home,
                () => 200,
                "How did you even manage to die in the home area?",
                "That must've been intentional. Surely...",
                "Nope - I haven't implemented any achivements yet. There's no need to blow yourself up."
            ),
            new(//If you die in the first room/in less than a minute on Veteran
                source => DifficultyManager.Difficulty == Difficulty.Veteran
                && (PersistentStats.RoomsCleared <= 1
                || TimeTracker.Instance.SecondsSurvived < 30f),
                () => 120,
                "Maybe veteran wasn't quite the right choice...",
                "Perhaps try an easier difficulty next time...",
                "It's ok - not everyone can be a professional!"
            ),
            new(//If you die similarly on Rookie
                source => DifficultyManager.Difficulty == Difficulty.Rookie
                && PersistentStats.RoomsCleared <= 1
                && TimeTracker.Instance.SecondsSurvived < 30f,
                () => 120,
                "\"Guys, that one was just a warm-up...\"",
                $"I'm not sure what to say... you died in the first {Mathf.Floor(TimeTracker.Instance.SecondsSurvived)} seconds?",
                "Perhaps you should revisit the tutorial..."
            ),
            new(//If you die with loads of spare money/haven't spent anything
                source => FloorManager.Instance.FloorNumber > 1
                && CoinManager.Instance.Earnings > 300
                && CoinManager.Instance.Expenditure == 0,
                () => 100,
                "You do know that you can spend your money, right?",
                "Try purchasing some upgrades next time. They're surprisingly helpful."
            ),
            new(//If you die with (nearly) every upgrade
                source => Perks.NumUnlockedPerks > 30,
                () => 140,
                $"Could {Perks.NumUnlockedPerks} upgrades still not save you?"
            ),
            new(//If you die at least 10 floors in
                source => FloorManager.Instance.FloorNumber >= 10,
                () => 80,
                $"Floor {FloorManager.Instance.FloorNumber}? That's pretty impressive!",
                "Unlucky - you were doing so well!"
            ),
            new(//If you accidentally explode yourself
                source => source is DestructibleObject,
                () => 110,
                "\"Oops! I didn't know those things dealt damage!\"",
                "\"Who put those things there anyway?\""
            ),
            new(//If you were standing still for ages
                source => MovementTracker.TimeStoodStill > 10f,
                () => 180,
                $"Perhaps you should've paused before going AFK for {Mathf.FloorToInt(MovementTracker.TimeStoodStill)}s.",
                $"Standing still for {Mathf.FloorToInt(MovementTracker.TimeStoodStill)}s? Did your keyboard disconnect or what?",
                "BREAKING NEWS:\nResearch shows that standing still leads to a 100% increased risk of death!"
            ),
            new(//If you were tabbed out of the game
                source => !Application.isFocused
                && FloorManager.Instance.FloorNumber > 1,
                () => 170,
                "Maybe this wasn't the best time to tab out...",
                "Is the game really so boring that you had to tab out? :("
            ),
            new(//If you die with over 90% dodge chance
                source => PlayerStats.DodgeChance > 90f,
                () => 180,
                $"You know you had a {PlayerStats.DodgeChance}% chance of dodging that attack? That is impressively unlucky!",
                $"How unlucky do you have to be to die with a {PlayerStats.DodgeChance}% dodge chance?"
            ),
            new(//If you die on the final room of a floor (not of floor 1)
                source => FloorManager.Instance.FloorNumber > 1
                && RoomGetter.TryGetRoom(out PhysicalRoom room)
                && room.Room == MapRenderer.Instance.Dungeon.LastRoom,
                () => 80,
                $"On the final room? If only you could've avoided that stupid {Player.Instance.LastDamager.DisplayName}!"
            ),
            new(//If there are no enemies alive (i.e. you died to a stray projectile after clearing the room)
                source => source is not DestructibleObject
                && RoomGetter.TryGetByPosition(Player.Instance.transform.position, out PhysicalRoom room)
                && room is EnemyRoom enemyRoom && enemyRoom.Count == 0,
                () => 170,
                "So close... there wasn't even anything left.",
                "If only you could've avoided that attack, you'd have survived."
            ),
            new(//If you skipped every single puzzle so far
                source => PersistentStats.PuzzlesEncountered > 5
                && PersistentStats.PuzzlesSolved == 0,
                () => 140,
                $"Was it laziness or stupidity that made you avoid all {PersistentStats.PuzzlesEncountered} of those puzzles?",
                $"Congratulations! You successfully solved 0/{PersistentStats.PuzzlesEncountered} of the puzzles you encountered!"
            ),
            new(//If you forgot to bring any elements
                source => WeaponManager.Instance.Weapons[0].Magazine.Count == 0
                && WeaponManager.Instance.Weapons[1].Magazine.Count == 0,
                () => 200,
                "I think you might've forgot something...",
                "Pro Tip: Weapons are known to be quite helpful.",
                "Sadly, pacifism doesn't appear to work on monsters.",
                "You used nothing! It wasn't very effective."
            ),
            new(//If you only ever triggered one reaction type
                source => PersistentStats.RoomsCleared > 2
                && ReactionTracker.UniqueReactions == 1,
                () => 130,
                "Only one reaction? Maybe try a new loadout next time.",
                "Whatever strategy you're trying, there's definitely a better one."
            ),
            new(//If you never triggered a reaction
                source => PersistentStats.RoomsCleared > 1
                && ReactionTracker.UniqueReactions == 0,
                () => 170,
                "You do know you're supposed to trigger reactions, right?",
                "The target dummies are there for a reason, you know?"
            ),
            new(//If you manage to die to a zombie
                source => source.DisplayName == "Zombie",
                () => 50,
                "A zombie? But they literally do like... nothing.",
                "Surely you didn't just die to the most basic enemy in the game..."
            ),
            new(//If you die to a skeleton projectile
                source => source.DisplayName == "Skeleton",
                () => 50,
                "Who would've thought a bone could do so much damage!",
                "You might want to work on your dodging. Just saying..."
            ),
            new(//If you die with 12+ terrified stacks
                source => PlayerBuffs.Instance.TerrifiedStacks >= 12 && !Perks.Has(Perk.Illuminating),
                () => 80,
                "Try turning up your brightness if you can't see.",
                "That's not fair! You couldn't even see it coming!"
            ),
            new(//If you get backstabbed by a goblin
                source =>
                {
                    if(source.DisplayName != "Goblin") return false;

                    Enemy goblin = (Enemy) source;
                    bool playerFacingLeft = Player.Instance.Sprite.flipX;
                    bool meFacingLeft = goblin.Sprite.flipX;

                    bool onRight = goblin.transform.position.x >= Player.Instance.transform.position.x;
                    return (meFacingLeft && onRight && playerFacingLeft)
                        || (!meFacingLeft && !onRight && !playerFacingLeft);
                },
                () => 100,
                "Double damage on backstabs? That's a bit unfair!",
                "\"Best watch your back\" - the goblin that just killed you"
            ),
            new(//If you die to a wizard who just teleported to you
                source => source.DisplayName == "Wizard",
                () => 70,
                "\"Can this stupid ***ing wizard stay still and stop teleporting!\"",
                "\"Why do these damn projectiles follow me around!?\""
            ),
            new(//If you die to an ooze or toxic puddle
                source => source.DisplayName == "Ooze",
                () => 60,
                "You seem to be in a sticky situation..."
            ),
            new(//If you get killed by a glasshopper
                source => source.DisplayName == "Glasshopper",
                () => 70,
                "Turns out glass is rather sharp.",
                "Hopefully that death didn't shatter your confidence..."
            ),
            new(//If you die to a bomb dropped by a drone
                source => source.DisplayName == "Drone",
                () => 50,
                "Guess what? The bomb wasn't just for decoration!",
                "\"What's that ticking noise? Hmmm, must be nothing.\"",
                "You know you had an entire second to react to that?"
            ),
            new(//If you die to a laser from a sentry
                source => source.DisplayName == "Sentry",
                () => 60,
                "You quite literally walked right into that one.",
                "How can you possibly be killed by something that can't even move?"
            ),
            new(//If a spider-bot self-detonates near you
                source => source.DisplayName == "Spider",
                () => 60,
                "\"See! Told you spiders were dangerous!\"",
                "\"What is this place!? Bloody Australia or something?\""
            ),
            new(//If you get killed by a demon
                source => source.DisplayName == "Demon",
                () => 50,
                "Go to hell! Wait no, we're already there...",
                "\"That's not fair! I didn't even have time to react!\""
            ),
            new(//If you run into a magmacrawler
                source => source.DisplayName == "Magmacrawler",
                () => 60,
                "Did it run into you or did you run into it? That is the question.",
                "Did you really just get killed by the DVD screensaver in worm form?",
                "That worm wasn't even targeting you - that's kind of embarassing..."
            ),
            new(//If you die to a hellhound
                source => source.DisplayName == "Hellhound",
                () => 50,
                "This breed of dog doesn't appear to be particularly friendly. Approach with caution.",
                "Watch out! Ah, maybe I should've given that warning a few seconds ago..."
            ),
            new(//If you die while frozen by a wraith
                source => FloorManager.Instance.IsIndex(FloorManager.Instance.Floor, 3)
                && PlayerStats.MoveSpeed < 10f,
                () => 80,
                "Why didn't you just run away? Oh right, you couldn't.",
                "Scary, aren't they? Petrifying, you could even say."
            )
        };

        /// <summary>
        /// Chooses a random death message with the highest priority (same priority get sorted randomly)
        /// </summary>
        /// <param name="killer"></param>
        /// <returns></returns>
        public static string ChooseDeathMessage(IDamageSource killer) => (AllDeathMessages
            .Where(m => m.condition(killer))
            .OrderByDescending(m => m.priority() * UnityEngine.Random.Range(0.7f, 1.3f))
            .ThenBy(m => UnityEngine.Random.value)
            .FirstOrDefault() is {} message)
            ? message.Message : "Skill issue";
    }

    /// <summary>
    /// Represents a group of messages that can occur as a result of some condition
    /// </summary>
	public readonly struct DeathMessage
    {
        public readonly string[] messages;
        public readonly string Message => (messages == null || messages.Length == 0) ? "This death message should never appear. Congrats on breaking the game!" : messages[UnityEngine.Random.Range(0, messages.Length)];
        public readonly Func<IDamageSource, bool> condition;
        public readonly Func<int> priority;

        public DeathMessage(Func<IDamageSource, bool> condition, Func<int> priority, params string[] messages)
        {
            this.messages = messages;
            this.condition = condition;
            this.priority = priority;
        }
    }
}
