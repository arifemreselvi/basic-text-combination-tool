using System.ComponentModel;
using System.Text;

namespace TextCombinatorWinForms;

public partial class Form1 : Form
{
    private readonly BindingList<TextCategory> _categories = [];
    private readonly ListBox _categoryList = new();
    private readonly ListBox _itemList = new();
    private readonly TextBox _categoryNameInput = new();
    private readonly TextBox _itemInput = new();
    private readonly TextBox _outputPathInput = new();
    private readonly Label _selectedCategoryLabel = new();
    private readonly Label _statusLabel = new();
    private readonly Label _combinationInfoLabel = new();

    public Form1()
    {
        InitializeComponent();
        BuildUi();
        BindEvents();
        _categoryList.DataSource = _categories;
        _categoryList.DisplayMember = nameof(TextCategory.Name);
        UpdateSelectedCategoryView();
    }

    private void BuildUi()
    {
        Text = "Text Combination Builder";
        MinimumSize = new Size(900, 600);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(10)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var categoryPanel = BuildCategoryPanel();
        var itemPanel = BuildItemPanel();
        var outputPanel = BuildOutputPanel();

        mainLayout.Controls.Add(categoryPanel, 0, 0);
        mainLayout.Controls.Add(itemPanel, 1, 0);
        mainLayout.Controls.Add(outputPanel, 0, 1);
        mainLayout.SetColumnSpan(outputPanel, 2);

        Controls.Add(mainLayout);
    }

    private Control BuildCategoryPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 6,
            ColumnCount = 1,
            Padding = new Padding(6)
        };
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var title = new Label
        {
            Text = "Categories",
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(3, 3, 3, 8)
        };

        _categoryNameInput.PlaceholderText = "Category name";
        _categoryNameInput.Dock = DockStyle.Top;

        var addCategoryButton = new Button
        {
            Text = "Add Category",
            AutoSize = true,
            Dock = DockStyle.Top
        };
        addCategoryButton.Click += (_, _) => AddCategory();

        _categoryList.Dock = DockStyle.Fill;

        var removeCategoryButton = new Button
        {
            Text = "Remove Selected Category",
            AutoSize = true,
            Dock = DockStyle.Top
        };
        removeCategoryButton.Click += (_, _) => RemoveSelectedCategory();

        _combinationInfoLabel.Text = "Combinations: 0";
        _combinationInfoLabel.AutoSize = true;
        _combinationInfoLabel.Margin = new Padding(3, 8, 3, 3);

        panel.Controls.Add(title, 0, 0);
        panel.Controls.Add(_categoryNameInput, 0, 1);
        panel.Controls.Add(addCategoryButton, 0, 2);
        panel.Controls.Add(_categoryList, 0, 3);
        panel.Controls.Add(removeCategoryButton, 0, 4);
        panel.Controls.Add(_combinationInfoLabel, 0, 5);

        return panel;
    }

    private Control BuildItemPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 6,
            ColumnCount = 1,
            Padding = new Padding(6)
        };
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _selectedCategoryLabel.Text = "Select a category to add items.";
        _selectedCategoryLabel.Font = new Font(Font, FontStyle.Bold);
        _selectedCategoryLabel.AutoSize = true;
        _selectedCategoryLabel.Margin = new Padding(3, 3, 3, 8);

        _itemInput.Multiline = true;
        _itemInput.Height = 90;
        _itemInput.Dock = DockStyle.Top;
        _itemInput.ScrollBars = ScrollBars.Vertical;
        _itemInput.PlaceholderText = "Add one or more lines (each line becomes an item)";

        var addItemButton = new Button
        {
            Text = "Add Lines to Selected Category",
            AutoSize = true,
            Dock = DockStyle.Top
        };
        addItemButton.Click += (_, _) => AddItemsToSelectedCategory();

        _itemList.Dock = DockStyle.Fill;

        var removeItemButton = new Button
        {
            Text = "Remove Selected Line",
            AutoSize = true,
            Dock = DockStyle.Top
        };
        removeItemButton.Click += (_, _) => RemoveSelectedItem();

        var hintLabel = new Label
        {
            Text = "Combination order follows category order from top to bottom.",
            AutoSize = true,
            ForeColor = Color.DimGray,
            Margin = new Padding(3, 8, 3, 3)
        };

        panel.Controls.Add(_selectedCategoryLabel, 0, 0);
        panel.Controls.Add(_itemInput, 0, 1);
        panel.Controls.Add(addItemButton, 0, 2);
        panel.Controls.Add(_itemList, 0, 3);
        panel.Controls.Add(removeItemButton, 0, 4);
        panel.Controls.Add(hintLabel, 0, 5);

        return panel;
    }

    private Control BuildOutputPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 2,
            Padding = new Padding(6)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var outputLabel = new Label
        {
            Text = "Output folder:",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 8, 6, 3)
        };

        _outputPathInput.Dock = DockStyle.Fill;
        _outputPathInput.Margin = new Padding(0, 5, 0, 3);

        var browseButton = new Button
        {
            Text = "Browse",
            AutoSize = true,
            Margin = new Padding(8, 3, 0, 3)
        };
        browseButton.Click += (_, _) => BrowseOutputFolder();

        var generateButton = new Button
        {
            Text = "Generate Files",
            AutoSize = true,
            Margin = new Padding(8, 3, 0, 3)
        };
        generateButton.Click += (_, _) => GenerateFiles();

        _statusLabel.Text = "Ready.";
        _statusLabel.AutoSize = true;
        _statusLabel.ForeColor = Color.DarkSlateGray;
        _statusLabel.Margin = new Padding(3, 8, 3, 3);

        panel.Controls.Add(outputLabel, 0, 0);
        panel.Controls.Add(_outputPathInput, 1, 0);
        panel.Controls.Add(browseButton, 2, 0);
        panel.Controls.Add(generateButton, 3, 0);
        panel.Controls.Add(_statusLabel, 0, 1);
        panel.SetColumnSpan(_statusLabel, 5);

        return panel;
    }

    private void BindEvents()
    {
        _categoryList.SelectedIndexChanged += (_, _) => UpdateSelectedCategoryView();
    }

    private void AddCategory()
    {
        var name = _categoryNameInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            SetStatus("Enter a category name.");
            return;
        }

        _categories.Add(new TextCategory(name));
        _categoryNameInput.Clear();
        _categoryList.SelectedIndex = _categories.Count - 1;
        UpdateCombinationInfo();
        SetStatus($"Added category '{name}'.");
    }

    private void RemoveSelectedCategory()
    {
        if (_categoryList.SelectedItem is not TextCategory selected)
        {
            SetStatus("Select a category to remove.");
            return;
        }

        _categories.Remove(selected);
        UpdateSelectedCategoryView();
        UpdateCombinationInfo();
        SetStatus($"Removed category '{selected.Name}'.");
    }

    private void AddItemsToSelectedCategory()
    {
        if (_categoryList.SelectedItem is not TextCategory selected)
        {
            SetStatus("Select a category first.");
            return;
        }

        var addedCount = 0;
        var lines = _itemInput.Text
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                continue;
            }

            selected.Items.Add(line);
            addedCount++;
        }

        _itemInput.Clear();
        UpdateCombinationInfo();

        if (addedCount == 0)
        {
            SetStatus("No lines were added.");
            return;
        }

        SetStatus($"Added {addedCount} line(s) to '{selected.Name}'.");
    }

    private void RemoveSelectedItem()
    {
        if (_categoryList.SelectedItem is not TextCategory selected)
        {
            SetStatus("Select a category first.");
            return;
        }

        if (_itemList.SelectedItem is not string item)
        {
            SetStatus("Select a line to remove.");
            return;
        }

        selected.Items.Remove(item);
        UpdateCombinationInfo();
        SetStatus($"Removed line from '{selected.Name}'.");
    }

    private void BrowseOutputFolder()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select folder where text files will be created"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _outputPathInput.Text = dialog.SelectedPath;
    }

    private void GenerateFiles()
    {
        if (_categories.Count == 0)
        {
            SetStatus("Add at least one category.");
            return;
        }

        if (_categories.Any(c => c.Items.Count == 0))
        {
            var emptyCategory = _categories.First(c => c.Items.Count == 0);
            SetStatus($"Category '{emptyCategory.Name}' has no lines.");
            return;
        }

        var outputPath = _outputPathInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(outputPath) || !Directory.Exists(outputPath))
        {
            SetStatus("Select a valid output folder.");
            return;
        }

        try
        {
            var firstCategory = _categories[0];
            var additionalCategories = _categories.Skip(1).ToList();
            var usedNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var filesCreated = 0;

            foreach (var firstItem in firstCategory.Items)
            {
                var combinations = BuildCombinations(firstItem, additionalCategories);
                var fileName = BuildUniqueFileName(firstItem, usedNames);
                var filePath = Path.Combine(outputPath, fileName);

                using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
                foreach (var combination in combinations)
                {
                    writer.WriteLine(combination);
                    writer.WriteLine();
                }
                filesCreated++;
            }

            SetStatus($"Generated {filesCreated} file(s) in '{outputPath}'.");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private List<string> BuildCombinations(string firstItem, List<TextCategory> additionalCategories)
    {
        var results = new List<string>();

        if (additionalCategories.Count == 0)
        {
            results.Add(firstItem);
            return results;
        }

        var parts = new List<string> { firstItem };
        BuildCombinationsRecursive(additionalCategories, 0, parts, results);
        return results;
    }

    private static void BuildCombinationsRecursive(
        List<TextCategory> categories,
        int depth,
        List<string> parts,
        List<string> results)
    {
        if (depth == categories.Count)
        {
            results.Add(string.Join(' ', parts));
            return;
        }

        foreach (var item in categories[depth].Items)
        {
            parts.Add(item);
            BuildCombinationsRecursive(categories, depth + 1, parts, results);
            parts.RemoveAt(parts.Count - 1);
        }
    }

    private static string BuildUniqueFileName(string seed, Dictionary<string, int> usedNames)
    {
        var sanitized = SanitizeFileName(seed);
        if (!usedNames.TryGetValue(sanitized, out var count))
        {
            usedNames[sanitized] = 1;
            return $"{sanitized}.txt";
        }

        count++;
        usedNames[sanitized] = count;
        return $"{sanitized}_{count}.txt";
    }

    private static string SanitizeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray()).Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            return "item";
        }

        return cleaned.Length > 80 ? cleaned[..80] : cleaned;
    }

    private void UpdateSelectedCategoryView()
    {
        if (_categoryList.SelectedItem is TextCategory selected)
        {
            _selectedCategoryLabel.Text = $"Lines in: {selected.Name}";
            _itemList.DataSource = selected.Items;
        }
        else
        {
            _selectedCategoryLabel.Text = "Select a category to add items.";
            _itemList.DataSource = null;
        }

        UpdateCombinationInfo();
    }

    private void UpdateCombinationInfo()
    {
        if (_categories.Count == 0)
        {
            _combinationInfoLabel.Text = "Combinations: 0";
            return;
        }

        if (_categories.Any(c => c.Items.Count == 0))
        {
            _combinationInfoLabel.Text = "Combinations: pending (some categories are empty)";
            return;
        }

        var total = _categories
            .Select(c => (decimal)c.Items.Count)
            .Aggregate(1m, (acc, next) => acc * next);

        _combinationInfoLabel.Text = $"Combinations: {total:N0}";
    }

    private void SetStatus(string message)
    {
        _statusLabel.Text = message;
    }

    private sealed class TextCategory(string name)
    {
        public string Name { get; set; } = name;
        public BindingList<string> Items { get; } = [];

        public override string ToString() => Name;
    }
}
