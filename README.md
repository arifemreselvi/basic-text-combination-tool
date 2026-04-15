# TextCombinatorWinForms

`TextCombinatorWinForms` is a Windows Forms desktop application for generating category-based text combinations and exporting them into organized `.txt` files.

---

# Developer Spotlight

## Arif Emre Selvi

### Second Year Management Information Systems Student  

---

## Features

- Create and manage multiple categories in a fixed order
- Add multiple text lines/items to each category
- Generate all possible combinations by selecting one item from each category in sequence
- Create one `.txt` file per item in the first category
- Keep output readable: each combination is on its own line, with a blank line between entries
- Use a simple desktop interface without requiring Visual Studio to run

---

## How It Works

1. Add categories (for example: `Animal`, `Color`, `Mood`)
2. Add items under each category
3. Choose an output folder
4. Generate files

The application creates separate files based on the first category’s items.

---

## Example

Given:

1. `Animal`: `Cat`, `Dog`  
2. `Color`: `Black`, `White`  
3. `Mood`: `Happy`, `Calm`

Output files:

- `Cat.txt`
- `Dog.txt`

Sample content inside `Cat.txt`:

```text
Cat Black Happy

Cat Black Calm

Cat White Happy

Cat White Calm
