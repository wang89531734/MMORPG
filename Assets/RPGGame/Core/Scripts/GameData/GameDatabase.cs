using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameDatabase : ScriptableObject
{
    [Header("Player database")]
    [Range(1, 1000)]
    public int playerMaxLevel;
    [Tooltip("Requires Exp to levelup for each level")]
    public Int32Attribute playerExpTable;
    [Tooltip("`Soft Currency`, `Start Amount` is start amount when create new player")]
    public Currency softCurrency = new Currency() { id = "GOLD", startAmount = 0 };
    [Tooltip("`Hard Currency`, `Start Amount` is start amount when create new player")]
    public Currency hardCurrency = new Currency() { id = "GEM", startAmount = 0 };
    public Stamina stageStamina = new Stamina() { id = "STAGE_STAMINA", maxAmountTable = new Int32Attribute() };
    public List<Formation> stageFormations = new List<Formation>() {
        new Formation() { id = "STAGE_FORMATION_A" },
        new Formation() { id = "STAGE_FORMATION_B" },
        new Formation() { id = "STAGE_FORMATION_C" },
    };

    [Header("Item database")]
    [Tooltip("List of game items, place all items here (includes character, equipment)")]
    public List<BaseItem> items;

    [Header("Stage database")]
    [Tooltip("List of game stages, place all stages here")]
    public List<BaseStage> stages;

    [Header("Loot Box database")]
    [Tooltip("List of game loot boxes, place all loot boxes here")]
    public List<LootBox> lootBoxes;

    [Header("In-App Purchasing Package database")]
    [Tooltip("List of game IAP packages, place all IAP packages here")]
    public List<IapPackage> iapPackages;

    [Header("Game beginning")]
    [Tooltip("List of start items, place items that you want to give to players when begin the game")]
    public List<ItemAmount> startItems;
    [Tooltip("List of start characters, characters in this list will joined team formation when begin the game")]
    public List<CharacterItem> startCharacters;
    [Tooltip("List of stages that will be unlocked when begin the game")]
    public List<BaseStage> unlockStages;

    [Header("Fake data")]
    [Tooltip("List of fake players that will be shown in helper selection before start battle scene")]
    public List<FakePlayer> fakePlayers;

    [Header("Gameplay")]
    [Tooltip("Base attributes for all characters while battle")]
    public CalculationAttributes characterBaseAttributes;
    [Tooltip("Price to revive all characters when all characters dies, this use hard currency")]
    public int revivePrice = 5;
    [Tooltip("This will caculate with sum Atk to random Atk as: Atk = Mathf.Random(Atk * minAtkVaryRate, Atk * maxAtkVaryRate)")]
    public float minAtkVaryRate;
    [Tooltip("This will caculate with sum Atk to random Atk as: Atk = Mathf.Random(Atk * minAtkVaryRate, Atk * maxAtkVaryRate)")]
    public float maxAtkVaryRate;
    [Tooltip("If this is true, system will reset item level to 1 when evolved")]
    public bool resetItemLevelAfterEvolve;

    public readonly Dictionary<string, BaseItem> Items = new Dictionary<string, BaseItem>();
    public readonly Dictionary<string, Currency> Currencies = new Dictionary<string, Currency>();
    public readonly Dictionary<string, Stamina> Staminas = new Dictionary<string, Stamina>();
    public readonly Dictionary<string, Formation> Formations = new Dictionary<string, Formation>();
    public readonly Dictionary<string, BaseStage> Stages = new Dictionary<string, BaseStage>();
    public readonly Dictionary<string, LootBox> LootBoxes = new Dictionary<string, LootBox>();
    public readonly Dictionary<string, IapPackage> IapPackages = new Dictionary<string, IapPackage>();

    public void Setup()
    {
        Items.Clear();
        Currencies.Clear();
        Staminas.Clear();
        Formations.Clear();
        Stages.Clear();
        LootBoxes.Clear();
        IapPackages.Clear();

        AddItemsToDatabase(items);

        var startItemList = new List<BaseItem>();
        foreach (var startItem in startItems)
        {
            startItemList.Add(startItem.item);
        }
        AddItemsToDatabase(startItemList);

        var startCharacterList = new List<BaseItem>();
        foreach (var startCharacter in startCharacters)
        {
            startCharacterList.Add(startCharacter);
        }
        AddItemsToDatabase(startCharacterList);

        Currencies[softCurrency.id] = softCurrency;
        Currencies[hardCurrency.id] = hardCurrency;
        Staminas[stageStamina.id] = stageStamina;
        AddStageFormationsToDatabase(stageFormations);
        AddStagesToDatabase(stages);
        AddStagesToDatabase(unlockStages);
        AddLootBoxesToDatabase(lootBoxes);
        AddIapPackagesToDatabase(iapPackages);
    }

    private void AddItemsToDatabase(IEnumerable<BaseItem> items)
    {
        foreach (var item in items)
        {
            if (item == null)
                continue;
            var dataId = item.Id;
            if (!string.IsNullOrEmpty(dataId) && !Items.ContainsKey(dataId))
            {
                Items[dataId] = item;
            }
        }
    }

    private void AddStageFormationsToDatabase(IEnumerable<Formation> formations)
    {
        foreach (var formation in formations)
        {
            if (formation == null)
                continue;
            var dataId = formation.id;
            if (!string.IsNullOrEmpty(dataId) && !Formations.ContainsKey(dataId))
            {
                Formations[dataId] = formation;
            }
        }
    }

    private void AddStagesToDatabase(IEnumerable<BaseStage> stages)
    {
        foreach (var stage in stages)
        {
            if (stage == null)
                continue;
            var dataId = stage.Id;
            if (!string.IsNullOrEmpty(dataId) && !Stages.ContainsKey(dataId))
            {
                Stages[dataId] = stage;
            }
        }
    }

    private void AddLootBoxesToDatabase(IEnumerable<LootBox> lootBoxes)
    {
        foreach (var lootBox in lootBoxes)
        {
            if (lootBox == null)
                continue;
            var dataId = lootBox.Id;
            if (!string.IsNullOrEmpty(dataId) && !LootBoxes.ContainsKey(dataId))
            {
                LootBoxes[dataId] = lootBox;
            }
        }
    }

    private void AddIapPackagesToDatabase(IEnumerable<IapPackage> iapPackages)
    {
        foreach (var iapPackage in iapPackages)
        {
            if (iapPackage == null)
                continue;
            var dataId = iapPackage.Id;
            if (!string.IsNullOrEmpty(dataId) && !IapPackages.ContainsKey(dataId))
            {
                IapPackages[dataId] = iapPackage;
            }
        }
    }
}
