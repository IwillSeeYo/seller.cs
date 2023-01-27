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
                    SellItem();
                    break;

                case CommandToExit:
                    isOpen = false;
                    break;
            }

            Console.ReadLine();
            Console.Clear();
        }
    }

    public void SellItem()
    {
        int userInputBuyItemIndex;
        int userInputAmount;

        _seller.ShowItems();

        Console.Write("Введите номер желаемого товара:");

        userInputBuyItemIndex = ReadInt() - 1;

        if (_seller.TryGetItem(userInputBuyItemIndex, out Cell cell /*Product product*/) == false)
        {
            Console.WriteLine("Такого товара нету");

            return;
        }

        Console.Write("Введите количество товара: ");

        userInputAmount = ReadInt();

        if (userInputAmount <= cell.Amount)
        {
            if (_player.TryToBuy(cell.Product, userInputAmount))
            {
                _player.AddItem(cell.Product, userInputAmount);
                cell.DecreaseAmount(userInputAmount);
                _seller.RemoveItem(cell);
                _seller.IncreaseMoney(cell.Product, userInputAmount);
                _player.DecreaseMoney(cell.Product, userInputAmount);

                Console.WriteLine($"Вы купили {cell.Product.Name}, по цене {cell.Product.Price} | {userInputAmount} шт.");
            }
        }
        else
        {
            Console.WriteLine("Такого количества нету");

            return;
        }
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
    protected List<Cell> Items;

    public int Money { get; private set; }

    public Person(int money)
    {
        Money = money;
    }

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
        if (cell.Amount <= 0)
        {
            Items.Remove(cell);
        }
    }

    public bool TryGetStorage(Product product, out Cell cell)
    {
        cell = null;

        foreach (Cell currentCell in Items)
        {
            if (currentCell.IsContains(product))
            {
                cell = currentCell;
                return true;
            }
        }

        return false;
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

    public bool TryGetItem(int userInputBuyItemIndex, out Cell cell /*out Product product*/)
    {
        cell = null;
        //product= null;

        if (userInputBuyItemIndex < 0 || userInputBuyItemIndex >= Items.Count)
        {
            return false;
        }

        cell = Items[userInputBuyItemIndex];
        //product = Items[userInputBuyItemIndex].Product;

        Console.WriteLine($"Вы выбрали {cell}");
        //Console.WriteLine($"Вы выбрали {product}");

        return true;
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

    public bool TryToBuy(Product product, int amount)
    {
        int productFullPrice = product.Price * amount;

        if (productFullPrice > Money)
        {
            Console.WriteLine("У вас не хватает денег");
            return false;
        }

        return true;
    }

    public bool AddItem(Product product, int userInpruAmount)
    {
        if (TryGetStorage(product, out Cell cell))
        {
            cell.IncreaseAmount(userInpruAmount);
            return false;
        }
        else
        {
            Items.Add(new Cell(product, userInpruAmount));
            return true;
        }
    }
}

abstract class Product
{
    public string Name { get; private set; }
    public int Price { get; private set; }

    public Product(string name, int price)
    {
        Name = name;
        Price = price;
    }
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
    public int Damage { get; private set; }

    public Weapon(string name, int price, int damage) : base(name, price)
    {
        Damage = damage;
    }
}

class Clothes : Product
{
    public int Armor { get; private set; }

    public Clothes(string name, int price, int armor) : base(name, price)
    {
        Armor = armor;
    }
}