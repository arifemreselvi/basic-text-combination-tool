# TextCombinatorWinForms

`TextCombinatorWinForms` is a Windows Forms desktop app that builds text combinations from user-defined categories.

The Second Year Management Information Systems student named Arif Emre Selvi developed this app.

## What the app does

- Create multiple categories (in order)
- Add text lines/items to each category
- Generate all possible combinations by taking one item from each category in sequence
- Create one `.txt` file per item in the first category
- Write outputs with clean spacing: each combination is on its own line, with a blank line between combinations

## Example behavior

If the categories are:

1. `Animal`: `Cat`, `Dog`
2. `Color`: `Black`, `White`
3. `Mood`: `Happy`, `Calm`

The app creates:

- `Cat.txt` containing combinations starting with `Cat`
- `Dog.txt` containing combinations starting with `Dog`

And each file includes lines like:

```text
Cat Black Happy

Cat Black Calm

Cat White Happy

Cat White Calm
```

## Run from source

Requirements:

- Windows
- .NET SDK 9.0+

Command:

```bash
dotnet run --project TextCombinatorWinForms.csproj
```

## Build a shareable EXE

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Published executable location:

- `bin/Release/net9.0-windows/win-x64/publish/TextCombinatorWinForms.exe`

If the publish folder is locked by a running app instance, publish to a custom output folder:

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o "dist/win-x64"
```

## Project files

- `Form1.cs`: main UI and generation logic
- `Program.cs`: app startup entry point
- `TextCombinatorWinForms.csproj`: .NET project settings

## Screenshots

Add your images to `assets/` with the names below so they render automatically on GitHub.

### Main window

![Main Window](assets/main-window.png)

### Category editing

![Category Editing](assets/category-editing.png)

### Generated output example

![Generated Output](assets/generated-output.png)

### Optional GIF demo

![App Demo](assets/demo.gif)

If images are not visible, make sure the files exist in `assets/` and use exact matching names.

## License

This project is licensed under the MIT License. See `LICENSE`.

## Release notes template

Use `.github/RELEASE_TEMPLATE.md` when creating a new GitHub Release.
