namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Services;
    using static System.Int32;

    /// <summary>
    ///     Нода в дереве шаблона для отчёта по тест кейсам
    /// </summary>
    /// <remarks>
    ///     Шаблона отчёта представляется как дерево нод
    ///     Всего в шаблоне 3 уровня: корень, категория, подкатегория
    /// </remarks>
    internal class TemplateNode
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        private TemplateNode() { }

        /// <summary>
        ///     Возвращает пустой шаблон отчёта
        /// </summary>
        public static TemplateNode Default()
        {
            return new TemplateNode { Name = "Тест-кейсы", Rank = 1, Order = 0 };
        }

        /// <summary>
        ///     Возвращает новую корневую ноду шаблона
        /// </summary>
        public static TemplateNode CreateRoot()
        {
            return new TemplateNode { Rank = 1, Order = 0 };
        }

        /// <summary>
        ///     Уровень ноды
        /// </summary>
        /// <remarks>
        ///     1 - корень
        ///     2 - категория
        ///     3 - подкатегория
        /// </remarks>
        private int Rank { get; set; }

        /// <summary>
        ///     Порядковый номер ноды 
        /// </summary>
        private int Order { get; set; }

        /// <summary>
        ///     Имя
        /// </summary>
        public string Name { get; private set; } = default!;

        /// <summary>
        ///     Описание
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        ///     Дочерние ноды 
        /// </summary>
        /// <remarks>
        ///     Для нод подкатегорий всегда пустой словарь
        /// </remarks>
        private readonly List<TemplateNode> _children = new List<TemplateNode>();

        /// <summary>
        ///    Родительскная нода
        /// </summary>
        /// <remarks>
        ///     Для корневой ноды всегда null
        /// </remarks>
        private TemplateNode? Parent { get; set; }

        /// <summary>
        ///     Возвращает корневую нода шаблона
        /// </summary>
        public TemplateNode Root => Parent is null ? this : Parent.Root;

        /// <summary>
        ///     Возвращает описание категории из шаблона
        /// </summary>
        public string? GetCategoryDescription(string? categoryName)
        {
            return GetChild(categoryName)?.Description;
        }

        /// <summary>
        ///     Возвращает описание подкатегории из шаблона
        /// </summary>
        public string? GetSubCategoryDescription(string? categoryName, string? subCategoryName)
        {
            return GetChild(categoryName)?.GetChild(subCategoryName)?.Description;
        }

        /// <summary>
        ///     Возвращает метку сортировки категории
        /// </summary>
        public int GetCategoryOrder(string? categoryName)
        {
            // если в тест кейсе не указана сортировка, то сдвигаем в самый низ сортировки категорий
            if (string.IsNullOrWhiteSpace(categoryName))
                return _children.Count + 1;

            // категория в шаблоне
            var category = GetChild(categoryName);

            // если категория не найдена в шаблоне, то сдвигаем ниже остальных категорий шаблона
            if (category is null)
                return _children.Count;

            // возвращаем порядковый номер категории из шаблона
            return category.Order;
        }

        /// <summary>
        ///     Возвращает метку сортировки подкатегории
        /// </summary>
        public int GetSubCategoryOrder(string? categoryName, string? subCategoryName)
        {
            // если в тест кейсе не указана категория, то сдвигаем в самый низ сортировки категорий
            if (string.IsNullOrWhiteSpace(categoryName))
                return 0;

            // категория в шаблоне
            var category = GetChild(categoryName);

            // если категория не найдена в шаблоне, то сдвигаем ниже остальных категорий шаблона
            if (category is null)
            {
                if (string.IsNullOrWhiteSpace(subCategoryName))
                    return MaxValue;
                else
                    return MaxValue - 1;
            }

            // если в тест кейсе не указана подкатегория, то сдвигаем в самый низ сортировки подкатегорий
            if (string.IsNullOrWhiteSpace(subCategoryName))
                return category._children.Count + 1;

            // подкатегория в шаблоне
            var subCategory = category.GetChild(subCategoryName);

            // если подкатегория не найдена в шаблоне, то сдвигаем ниже остальных подкатегорий
            if (subCategory is null)
                return category._children.Count;

            // возвращаем порядковый номер подкатегории из шаблона
            return subCategory.Order;
        }

        /// <summary>
        ///     Добавляет ноду в дерево шаблона
        /// </summary>
        /// <param name="line">Строка из шаблона</param>
        /// <returns>
        ///     Возвращает текущую ноду в построении дерева шаблна
        ///     Или ошибку, если шаблон оказался невалидным
        /// </returns>
        public (TemplateNode? node, string? error) AddNode(string line)
        {
            // добавляем братскую ноду текущего уровня?
            if (ShouldAddSibling(line))
            {
                // имя ноды
                var name = line.Remove(0, Rank).Trim();
                if (string.IsNullOrWhiteSpace(name))
                    return (null, "Шаблон включает пустые заголовки");

                // присваиваем имя текущей, если у него ещё нет имени
                if (string.IsNullOrWhiteSpace(Name))
                {
                    Name = name;
                    return (this, null);
                }

                // может быть только один заголовок первого уровня
                if (IsRootNode)
                    return (null, "Шаблон включает более одного заголовка первого уровня");

                // проверяем дублирование заголовков
                if (Parent!.HasChildWithName(name))
                    return (null, $"В шаблоне задублирован заголовок {name}");

                // добавляем в родительский набор новую ноду того же уровня
                var siblingNode = Parent.AddChildNode(name);

                return (siblingNode, null);
            }

            // добавляем потомка?
            if (ShouldAddChild(line))
            {
                // имя ноды
                var name = line.Remove(0, ChildRank).Trim();
                if (string.IsNullOrWhiteSpace(name))
                    return (null, "Шаблон включает пустые заголовки");

                // проверяем дублирование заголовков
                if (HasChildWithName(name))
                    return (null, $"В шаблоне задублирован заголовок {name}");

                // добавляем потомка в текущую ноду
                var childNode = AddChildNode(name);

                return (childNode, null);
            }

            // добавляем ноду в корень?
            if (ShouldAddElder(line))
            {
                // имя ноды
                var name = line.Remove(0, ParentRank).Trim();
                if (string.IsNullOrWhiteSpace(name))
                    return (null, "Шаблон включает пустые заголовки");

                // проверяем дублирование заголовков
                if (Root.HasChildWithName(name))
                    return (null, $"В шаблоне задублирован заголовок {name}");

                // добавляем в корневую ноду нового потомка второго уровня
                var elderNode = Root.AddChildNode(name);

                return (elderNode, null);
            }

            // для первого уровня игнорируем лидирующие пустые строки шаблона
            if (string.IsNullOrWhiteSpace(line) && IsRootNode && string.IsNullOrWhiteSpace(Name))
                return (this, null);

            // добавляем строку к описанию текущей ноды
            Description = string.Join(Environment.NewLine, Description, line);
            return (this, null);
        }

        /// <summary>
        ///     Возвращает true, если в среди дочерних потомкой есть нода с таким именем
        /// </summary>
        private bool HasChildWithName(string name)
        {
            return _children.Any(s => s.Name.EqualsTrimmedIgnoreCase(name));
        }

        /// <summary>
        ///     Добавляет дочернюю ноду
        /// </summary>
        /// <param name="name">Имя ноды</param>
        private TemplateNode AddChildNode(string name)
        {
            var node = new TemplateNode { Parent = this, Name = name, Rank = Rank + 1, Order = _children.Count };
            _children.Add(node);
            return node;
        }

        /// <summary>
        ///     True, если надо добавить братскую ноду того же уровня в родителя
        /// </summary>
        /// <param name="line">Строка из шаблона</param>
        private bool ShouldAddSibling(string line) => line.StartsWith(Label);

        /// <summary>
        ///     True, если надо добавить дочернуюю ноду в текущую
        /// </summary>
        /// <param name="line">Строка из шаблона</param>
        private bool ShouldAddChild(string line) => line.StartsWith(ChildLabel) && IsNotSubCategoryNode;

        /// <summary>
        ///     True, если надо добавить новую ноду категории в корень
        /// </summary>
        /// <param name="line">Строка из шаблона</param>
        private bool ShouldAddElder(string line) => line.StartsWith(ParentLabel) && IsSubCategoryNode;

        /// <summary>
        ///     True, если это не нода подкатегории
        /// </summary>
        private bool IsNotSubCategoryNode => Rank < 3;

        /// <summary>
        ///     True, если это нода подкатегории
        /// </summary>
        private bool IsSubCategoryNode => Rank == 3;

        /// <summary>
        ///     True, если это корневая нода
        /// </summary>
        private bool IsRootNode => Rank == 1;

        /// <summary>
        ///     Уровень дочерней ноды
        /// </summary>
        private int ChildRank => Rank + 1;

        /// <summary>
        ///     Уровень родительской ноды
        /// </summary>
        private int ParentRank => Rank - 1;

        /// <summary>
        ///     Метка заголовка ноды в шаблоне
        /// </summary>
        private string Label => new string('#', Rank) + " ";

        /// <summary>
        ///     Метка заголовка дочерних ноды
        /// </summary>
        private string ChildLabel => new string('#', ChildRank) + " ";

        /// <summary>
        ///     Метка заголовка родительской ноды
        /// </summary>
        private string ParentLabel => new string('#', ParentRank) + " ";

        /// <summary>
        ///     Возвращает дочернюю ноду по имени
        /// </summary>
        private TemplateNode? GetChild(string? name)
        {
            return _children.FirstOrDefault(t => name.EqualsTrimmedIgnoreCase(t.Name.Trim()));
        }
    }
}