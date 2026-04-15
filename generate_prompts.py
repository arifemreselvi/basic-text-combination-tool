import argparse
import math
from itertools import islice, product
from pathlib import Path
from typing import Iterable, List, Sequence, Tuple


def split_top_level_commas(text: str) -> List[str]:
    items: List[str] = []
    current: List[str] = []
    depth = 0

    for ch in text:
        if ch == "(":
            depth += 1
            current.append(ch)
            continue
        if ch == ")":
            depth = max(0, depth - 1)
            current.append(ch)
            continue
        if ch == "," and depth == 0:
            item = "".join(current).strip()
            if item:
                items.append(item)
            current = []
            continue
        current.append(ch)

    tail = "".join(current).strip()
    if tail:
        items.append(tail)

    return items


def parse_categories(raw_text: str) -> List[Tuple[str, List[str]]]:
    categories: List[Tuple[str, List[str]]] = []
    current_name: str | None = None
    current_lines: List[str] = []

    def flush_current() -> None:
        nonlocal current_name, current_lines
        if not current_name:
            return

        merged = " ".join(line.strip() for line in current_lines if line.strip())
        parsed_items = split_top_level_commas(merged)

        seen = set()
        deduped: List[str] = []
        for item in parsed_items:
            key = item.lower()
            if key in seen:
                continue
            seen.add(key)
            deduped.append(item)

        categories.append((current_name, deduped))
        current_name = None
        current_lines = []

    for original_line in raw_text.splitlines():
        line = original_line.strip()
        if not line:
            continue

        if line.upper().endswith("CATEGORY:"):
            flush_current()
            current_name = line[:-1].strip()
            continue

        if current_name is None:
            continue

        current_lines.append(line)

    flush_current()
    return categories


def count_combinations(categories: Sequence[Tuple[str, Sequence[str]]]) -> int:
    total = 1
    for _, items in categories:
        if not items:
            return 0
        total *= len(items)
    return total


def chunk_ranges(total: int, parts: int) -> List[Tuple[int, int]]:
    if total <= 0:
        return []

    ranges: List[Tuple[int, int]] = []
    for i in range(parts):
        start = math.floor(i * total / parts)
        end = math.floor((i + 1) * total / parts)
        ranges.append((start, end))
    return ranges


def write_chunk(
    categories: Sequence[Tuple[str, Sequence[str]]],
    start: int,
    end: int,
    out_file: Path,
    append: bool,
) -> int:
    item_lists: List[Sequence[str]] = [items for _, items in categories]
    combo_iter: Iterable[Tuple[str, ...]] = islice(product(*item_lists), start, end)

    mode = "a" if append else "w"
    written = 0
    with out_file.open(mode, encoding="utf-8", newline="\n") as f:
        for combo in combo_iter:
            f.write(", ".join(combo))
            f.write("\n")
            written += 1
    return written


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Generate prompt combinations in 10 chunks.")
    parser.add_argument("--input", required=True, help="Path to category input text file")
    parser.add_argument(
        "--output",
        default=r"C:\Projects\prompt\prompts.txt",
        help=r"Output txt path (default: C:\Projects\prompt\prompts.txt)",
    )
    parser.add_argument("--parts", type=int, default=10, help="Number of chunks (default: 10)")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    input_path = Path(args.input)
    output_path = Path(args.output)

    if not input_path.exists():
        print(f"Input file not found: {input_path}")
        return 1

    raw_text = input_path.read_text(encoding="utf-8")
    categories = parse_categories(raw_text)

    if not categories:
        print("No categories detected. Ensure lines end with 'CATEGORY:'.")
        return 1

    empty = [name for name, items in categories if len(items) == 0]
    if empty:
        print("Some categories are empty:")
        for name in empty:
            print(f"- {name}")
        return 1

    total = count_combinations(categories)
    if total == 0:
        print("No combinations can be generated.")
        return 1

    output_path.parent.mkdir(parents=True, exist_ok=True)

    print("Detected categories:")
    for name, items in categories:
        print(f"- {name}: {len(items)} items")
    print(f"Total combinations: {total}")
    print(f"Output: {output_path}")

    ranges = chunk_ranges(total, args.parts)
    total_written = 0
    first_write = True

    for idx, (start, end) in enumerate(ranges, start=1):
        if start == end:
            print(f"Part {idx}/{args.parts}: skipped (no rows in this slice).")
            continue

        answer = input(f"Continue with part {idx}/{args.parts} ({end - start} rows)? [y/N]: ").strip().lower()
        if answer not in {"y", "yes"}:
            print("Stopped by user.")
            print(f"Rows written so far: {total_written}")
            return 0

        written = write_chunk(
            categories=categories,
            start=start,
            end=end,
            out_file=output_path,
            append=not first_write,
        )
        first_write = False
        total_written += written
        print(f"Part {idx}/{args.parts}: wrote {written} rows.")

    print(f"Done. Total rows written: {total_written}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
