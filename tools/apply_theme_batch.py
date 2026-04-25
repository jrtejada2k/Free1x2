#!/usr/bin/env python3
"""
Batch-adds ModernTheme.ApplyToForm(this) via OnLoad override to all WinForms
form files that don't already have it.

Safe rules:
- Skips Designer.cs files
- Skips files already containing "ModernTheme"
- Skips files without ": Form" class inheritance
- Skips if OnLoad already exists
- Only touches files it can parse safely
"""

import os
import re
import sys

USING_LINE       = "using Free1X2.UI.Modern.Theming;"
ONLOAD_FORM      = (
    "\n"
    "        protected override void OnLoad(System.EventArgs e)\n"
    "        {\n"
    "            base.OnLoad(e);\n"
    "            ModernTheme.ApplyToForm(this);\n"
    "        }\n"
)
ONLOAD_USERCTRL  = (
    "\n"
    "        protected override void OnLoad(System.EventArgs e)\n"
    "        {\n"
    "            base.OnLoad(e);\n"
    "            ModernTheme.ApplyToControl(this);\n"
    "        }\n"
)

SKIP_PATTERNS = [
    ".Designer.cs",
    "MainForm.cs",          # already done manually
    "ModernFormBase.cs",
    "ThemeTestForm.cs",
    "ModernMainForm.cs",
    "ModernBancoPruebasForm.cs",
    "FormulariosHelper.cs",
]

def should_skip(path):
    fname = os.path.basename(path)
    return any(fname.endswith(p) or fname == p for p in SKIP_PATTERNS)

def process(path):
    with open(path, encoding="utf-8", errors="replace") as f:
        content = f.read()

    # Already themed
    if "ModernTheme" in content:
        return "skip:already_themed"

    # Detect inheritance type
    is_form       = bool(re.search(r':\s*(System\.Windows\.Forms\.)?Form\b', content))
    is_userctrl   = bool(re.search(r':\s*(System\.Windows\.Forms\.)?UserControl\b', content))

    if not is_form and not is_userctrl:
        return "skip:not_a_form"

    onload_body = ONLOAD_FORM if is_form else ONLOAD_USERCTRL

    # Must have InitializeComponent (proper WinForms control)
    if "InitializeComponent" not in content:
        return "skip:no_init_component"

    modified = content

    # --- Add using statement ---
    if USING_LINE not in modified:
        # Insert after the last consecutive "using ..." line block
        last_using_end = 0
        for m in re.finditer(r'^using\s+[^;]+;\s*$', modified, re.MULTILINE):
            last_using_end = m.end()
        if last_using_end:
            modified = modified[:last_using_end] + "\n" + USING_LINE + modified[last_using_end:]
        else:
            return "skip:no_using_block"

    # --- Add OnLoad override ---
    if "override void OnLoad" in modified or "override OnLoad" in modified:
        return "skip:has_onload"

    # Find class name (handles public/internal/no-modifier + optional partial)
    cls_match = re.search(r'(?:public\s+|internal\s+)?(?:partial\s+)?class\s+(\w+)', modified)
    if not cls_match:
        return "skip:no_class"
    class_name = cls_match.group(1)

    # Find constructor: "public ClassName(" — possibly with spaces
    ctor_pat = rf'public\s+{re.escape(class_name)}\s*\('
    ctor_match = re.search(ctor_pat, modified)
    if not ctor_match:
        return "skip:no_constructor"

    # Walk braces from the opening { of constructor body
    search_from = ctor_match.start()
    brace_open = modified.find('{', search_from)
    if brace_open < 0:
        return "skip:no_open_brace"

    depth = 0
    i = brace_open
    insert_at = -1
    while i < len(modified):
        ch = modified[i]
        if ch == '{':
            depth += 1
        elif ch == '}':
            depth -= 1
            if depth == 0:
                insert_at = i + 1
                break
        i += 1

    if insert_at < 0:
        return "skip:brace_mismatch"

    modified = modified[:insert_at] + onload_body + modified[insert_at:]

    with open(path, "w", encoding="utf-8") as f:
        f.write(modified)

    return "ok"


def main():
    search_dirs = [
        "Free1X2/UI",
        "Free1X2/UI/Estadisticas",
        "Free1X2/UI/Controls",
        "Free1X2/UI/Controls/Analisis",
        "Free1X2/UI/Controls/barraIconos",
    ]

    results = {"ok": [], "skip": {}}
    processed = set()

    for search_dir in search_dirs:
        for root, dirs, files in os.walk(search_dir):
            # Don't recurse into subdirectories from the base call — handled by search_dirs
            for fname in files:
                if not fname.endswith(".cs"):
                    continue
                path = os.path.join(root, fname)
                if path in processed:
                    continue
                processed.add(path)

                if should_skip(path):
                    results["skip"].setdefault("explicit_skip", []).append(path)
                    continue

                reason = process(path)
                if reason == "ok":
                    results["ok"].append(path)
                else:
                    results["skip"].setdefault(reason, []).append(path)

    print(f"\n=== RESULTS ===")
    print(f"Modified: {len(results['ok'])} files")
    for f in sorted(results["ok"]):
        print(f"  + {f}")

    print(f"\nSkipped:")
    for reason, files in sorted(results["skip"].items()):
        print(f"  [{reason}] {len(files)} files")
        if reason not in ("explicit_skip", "skip:not_a_form", "skip:no_init_component"):
            for f in sorted(files):
                print(f"    - {f}")


if __name__ == "__main__":
    main()
