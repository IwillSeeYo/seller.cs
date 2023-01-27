using System.Diagnostics.SymbolStore;

class Program
{
    static void Main(string[] args)
    {
        Market market = new Market();

        market.Work();
    }
}

class Market
{
    private Seller _seller = new Seller(10000);
    private Player _player = new Player(5000);

    public void Work()
    {
        const string CommandToShowBag = "1";
        const string CommandToBuyProduct = "2";
        const string CommandToExit = "0";

        bool isOpen = true;

        while (isOpen)
        {
            Console.WriteLine($"Магазин. Ваши деньги {_player.Money} | Деньги продавца {_seller.Money}\n");
            Console.WriteLine($" {CommandToShowBag}- показать содержимое сумки");
            Console.WriteLine($" {CommandToBuyProduct}- Купить у продавца предмет");
            Console.WriteLine($" {CommandToExit}- Выйти из магазина");

            switch (Console.ReadLine())
            {
                case CommandToShowBag:
                    _player.ShowItems();
                    break;

                case CommandToBuyProduct:
                    SellProduct();
                    break;

                case CommandToExit:
                    isOpen = false;
                    break;
            }

            Console.ReadLine();
            Console.Clear();
        }
    }

    public void SellProduct()
    {
        _seller.ShowItems();

        Console.Write("Введите номер желаемого товара:");

        int index = ReadInt() - 1;
        Cell cell = _seller.TryGetItem(index);

        if (cell == null)
        {
            Console.WriteLine("Такого товара нет");

            return;
        }

        Console.Write("Введите количество товара: ");

        int amount = ReadInt();

        if (amount > cell.Amount)
        {
            Console.WriteLine("Такого количества нет");

            return;
        }

        int totalPrice = amount * cell.Product.Price;

        if (_player.IsCanPay(totalPrice) == false)
        {
            Console.WriteLine("Не хватает денег");

            return;
        }

        _player.AddItem(cell.Product, amount, totalPrice);
        _player.DecreaseMoney(cell.Product, amount);
        _seller.RemoveItem(cell);
        _seller.IncreaseMoney(cell.Product, amount);
        cell.DecreaseAmount(amount);

        Console.WriteLine($"Вы купили {cell.Product.Name}, по цене {cell.Product.Price} | {amount} шт.");
    }

    private int ReadInt()
    {
        int index;

        while (int.TryParse(Console.ReadLine(), out index) == false)
        {
            Console.WriteLine("Ошибка ввода");
        }

        return index;
    }
}

abstract class Person
{
    public Person(int money)
    {
        Money = money;
    }

    protected List<Cell> Items;

    public int Money { get; protected set; }

    public void DecreaseMoney(Product product, int amount)
    {
        Money -= product.Price * amount;
    }

    public void IncreaseMoney(Product product, int amount)
    {
        Money += product.Price * amount;
    }

    public void ShowItems()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            int index = i + 1;

            Console.WriteLine($"{index} {Items[i]};");
        }
    }

    public void RemoveItem(Cell cell)
    {
        if (cell.Amount == 0)
        {
            Items.Remove(cell);
        }
    }
}


class Seller : Person
{
    public Seller(int money) : base(money)
    {
        Items = new List<Cell>();

        Items.Add(new Cell(new Weapon("Пожиратель мух", 550, 550), 1));
        Items.Add(new Cell(new Weapon("Полюбитель дух", 1000, 750), 1));
        Items.Add(new Cell(new Clothes("Мантия Кадгара", 10, 150), 1));
        Items.Add(new Cell(new Clothes("Кольчужная роба", 30, 250), 7));
        Items.Add(new Cell(new Clothes("Шлем Короля Лича", 800, 10000), 1));
    }

    public Cell TryGetItem(int index)
    {
        Cell cell = null;

        if (index < 0 || index >= Items.Count)
        {
            return cell;
        }

        cell = Items[index];

        Console.WriteLine($"Вы выбрали {cell.Product.Name}");

        return cell;
    }
}

class Player : Person
{
    public Player(int money) : base(money)
    {
        Items = new List<Cell>();

        Items.Add(new Cell(new Weapon("Ржавый меч из задней кости дракона", 50, 150), 1));
        Items.Add(new Cell(new Clothes("Самодельный доспех из кожи дракона", 10, 550), 6));
        Items.Add(new Cell(new Clothes("Самодельное наплечье из кожи дракона", 10, 550), 6));
        Items.Add(new Cell(new Clothes("Самодельные поножи из кожи дракона", 10, 550), 6));
    }

    public bool IsCanPay(int cost)
    {
        return Money >= cost;
    }

    public void AddItem(Product product, int amount, int cost)
    {
        bool isFound = false;

        foreach (Cell item in Items)
        {
            if (item.IsContains(product))
            {
                item.IncreaseAmount(amount);
                isFound = true;
                return;
            }
        }

        if (isFound == false)
        {
            Items.Add(new Cell(product, amount));
        }
    }
}

abstract class Product
{
    public Product(string name, int price)
    {
        Name = name;
        Price = price;
    }

    public string Name { get; private set; }
    public int Price { get; private set; }
}

class Cell
{
    public Cell(Product product, int amount)
    {
        Product = product;
        Amount = amount;
    }

    public Product Product { get; }
    public int Amount { get; private set; }

    public bool IsContains(Product product)
    {
        return product.Name == Product.Name;
    }

    public override string ToString()
    {
        string info = $"{Product.Name} |\t {Product.Price} цена |\t {Amount} шт.";

        return info;
    }

    public void DecreaseAmount(int amount) => Amount -= amount;

    public void IncreaseAmount(int amount) => Amount += amount;
}

class Weapon : Product
{
    public Weapon(string name, int price, int damage) : base(name, price)
    {
        Damage = damage;
    }

    public int Damage { get; private set; }
}

class Clothes : Product
{
    public Clothes(string name, int price, int armor) : base(name, price)
    {
        Armor = armor;
    }

    public int Armor { get; private set; }
}