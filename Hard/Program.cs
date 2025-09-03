namespace Module_5_Homework_1;

// Перечисление для типов предметов
enum ItemType
{
    Sword, Helm, Shield, Bracer, Back, Legs, Chest
}

// Класс предмета экипировки
class Item
{
    // Тип предмета
    public ItemType Type { get; private set; }
    // Требуемая сила для экипировки
    public int RequiredStrength { get; private set; }
    // Требуемая ловкость для экипировки
    public int RequiredAgility { get; private set; }
    
    // Конструктор класса Item
    public Item(ItemType type, int strength, int agility)
    {
        Type = type;
        RequiredStrength = strength;
        RequiredAgility = agility;
    }
}

// Базовый класс для персонажа (игрока или врага)
class Character
{
    // Имя персонажа
    public string Name { get; private set; }
    // Сила персонажа
    public int Strength { get; private set; }
    // Ловкость персонажа
    public int Agility { get; private set; }

    // Список доступных для экипировки предметов
    private readonly List<Item> _itemsAvailableToEquip = new List<Item>
    {
        new Item(ItemType.Sword, 10, 7),
        new Item(ItemType.Helm, 6, 4),
        new Item(ItemType.Shield, 7, 3),
        new Item(ItemType.Bracer, 3, 5),
        new Item(ItemType.Back, 2, 4),
        new Item(ItemType.Legs, 5, 5),
        new Item(ItemType.Chest, 7, 3)
    };
    // Список экипированных предметов
    private readonly List<Item> _equippedItems = new List<Item>();
    
    public IReadOnlyList<Item> ItemsAvailableToEquip => _itemsAvailableToEquip;
    public IReadOnlyList<Item> EquippedItems => _equippedItems;

    // Событие для проверки возможности экипировки предмета
    public event Predicate<Item> OnEquip;
    
    // Конструктор класса Character
    public Character(string name, int strength, int agility)
    {
        Name = name;
        Strength = strength;
        Agility = agility;
    }

    // Метод для попытки экипировать предмет
    public void TryEquip(Item item)
    {
        bool canEquip = OnEquip?.Invoke(item) ?? false;
        if (canEquip && !EquippedItems.Contains(item))
        {
            _equippedItems.Add(item);
            Console.WriteLine($"{item.Type} можно экипировать");
        }
        else
        {
            Console.WriteLine($"{item.Type} не может быть экипирован");
        }
    }

    // Метод для экипировки всех доступных предметов, соответствующих статам
    public void EquipAllItems()
    {
        if (ItemsAvailableToEquip.Count == 0)
        {
            Console.WriteLine("Нет доступных предметов для экипировки.");
            return;
        }
        
        foreach (Item item in ItemsAvailableToEquip.Where( item => !EquippedItems.Contains(item)))
        {
            TryEquip(item);
        }
        DisplayAllEquippedItems();
    }
    
    // Метод для проверки, достаточно ли статов для экипировки предмета
    public bool CanEquip(Item item)
    {
        return Strength >= item.RequiredStrength && Agility >= item.RequiredAgility;
    }
    
    // Метод для вывода списка экипированных предметов
    public void DisplayAllEquippedItems()
    {
        if (EquippedItems.Count == 0)
        {
            Console.WriteLine("\nНет экипированных предметов.");
            return;
        }
        Console.WriteLine($"\nЭкипировка персонажа {Name}: " + string.Join(", ", EquippedItems.Select(item => item.Type)));
    }

    // Метод для асинхронного увеличения статов на +1 каждые 2 секунды в течение 5 итераций
    public async Task IncreaseStats()
    {
        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(2000);
            Strength += 1;
            Agility += 1; 
            Console.WriteLine($"\nУвеличены статы! Сила: {Strength}, Ловкость: {Agility}");
            EquipAllItems();
        }
    }

    // Метод для олучения суммы текущих статов персонажа
    public int GetTotalStats()
    {
        return Strength + Agility;
    }

    // Метод для поиска предмета с максимальными статами
    public int GetMaxItemStats()
    {
        Item maxItem = ItemsAvailableToEquip.OrderByDescending(item => item.RequiredStrength + item.RequiredAgility).First();
        return maxItem.RequiredStrength +  maxItem.RequiredAgility;
    }

    // Метод для поединка с противником
    public void StartBattle(Character character)
    {
        int maxItemStats = GetMaxItemStats();
        int requiredStats = (int)(0.8 * maxItemStats);
        int totalStats = GetTotalStats();
        int totalEnemyStats = character.GetTotalStats();
        Console.WriteLine($"\nНачинается бой!" +
                          $"\nСтаты игрока: {totalStats}, Экипировано: {EquippedItems.Count} предметов" +
                          $"\nСтаты противника: {totalEnemyStats}, Экипировано: {character.EquippedItems.Count} предметов");
        
        if (totalStats >= requiredStats && EquippedItems.Count > character.EquippedItems.Count)
        {
            Console.WriteLine($"Игрок {Name} выиграл!");
        }
        else if (totalStats < requiredStats && EquippedItems.Count < character.EquippedItems.Count)
        {
            Console.WriteLine($"Противник {character.Name} выиграл!");
        }
        else if (totalStats < requiredStats && totalEnemyStats < requiredStats)
        {
            Console.WriteLine($"Ничья! У всех слишком слабые статы.\nНужно как минимум {requiredStats} статов");
        }
        else
        {
            Console.WriteLine("Ничья!");
        }
    }
}

// класс игрока
class Player(string name, int strength, int agility) : Character(name, strength, agility);

// класс противника
class Enemy(string name, int strength, int agility) : Character(name, strength, agility);


class Program
{
    static async Task Main(string[] args)
    {
        int strength, agility;
        // Ввод значения силы от игрока
        Console.WriteLine("Введите вашу силу:");
        while (!int.TryParse(Console.ReadLine(), out strength))
        {
            Console.WriteLine("Ошибка ввода! Введите целое положительное число:");
        }
        if (strength < 0)
        {
            throw new ArgumentOutOfRangeException("Статы не могут быть отрицательными");
        }
        // Ввод значения ловкости от игрока
        Console.WriteLine("Введите вашу ловкость:");
        while (!int.TryParse(Console.ReadLine(), out agility))
        {
            Console.WriteLine("Ошибка ввода! Введите целое положительное число:");
        }
        if (agility < 0)
        {
            throw new ArgumentOutOfRangeException("Статы не могут быть отрицательными");
        }

        Random random = new Random();
        Player player = new Player("Рыцарь", strength, agility);
        Enemy enemy = new Enemy("Демон", random.Next(3, 11), random.Next(3, 11));

        // Подписка на событие
        player.OnEquip += player.CanEquip;
        enemy.OnEquip += enemy.CanEquip;

        // Первичная экипировка предметов игроком
        player.EquipAllItems();
        
        // Запуск процесса увеличения статов игрока и повторной экипировки
        await player.IncreaseStats();
        
        // Экипировка предметов противником
        enemy.EquipAllItems();
        
        // Проведение сражения игрока с противником
        player.StartBattle(enemy);
        
        // Отписка от события
        player.OnEquip -= player.CanEquip;
        enemy.OnEquip -= enemy.CanEquip;
    }
}