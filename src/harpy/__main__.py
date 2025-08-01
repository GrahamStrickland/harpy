import os
from argparse import ArgumentParser

from harpy.lexer import Lexer
from harpy.parser import HarbourParser


def main():
    argparse = ArgumentParser(prog="harpy", description="Harbour to Python transpiler")
    argparse.add_argument(
        "--src",
        dest="src",
        type=str,
        help="source file to transpile, ending in either '.prg' or '.ch'",
        required=True,
    )
    args = argparse.parse_args()

    base, ext = os.path.splitext(args.src)
    if ext not in (".prg", ".ch"):
        raise RuntimeError(f"Invalid source path '{args.src}' with extension '{ext}'.")
    elif not os.path.exists(args.src):
        raise FileNotFoundError(f"Source file '{args.src}' not found.")
    elif not os.path.isfile(args.src):
        raise FileNotFoundError(f"Invalid source file '{args.src}'.")

    output = False
    with open(args.src, "r", encoding="utf-8") as infile:
        text = infile.read()
        output = len(text) > 0

        lexer = Lexer(text=text)
        parser = HarbourParser(lexer=lexer)
        result = parser.parse()

    if output:
        with open(base + ".py", "w", encoding="utf-8") as outfile:
            outfile.writelines([line for line in result])
    else:
        raise ValueError(f"File with path '{args.src}' is empty.")


if __name__ == "__main__":
    main()
