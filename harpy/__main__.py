from argparse import ArgumentParser
from os import path

from .harbour_parser import HarbourParser
from .lexer import Lexer


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

    lexer = Lexer(text=args.src)
    parser = HarbourParser(lexer=lexer)

    result = parser.parse()
    base, ext = path.splitext(args.src)
    if ext == "prg" or "ch":
        with open(path.join(base, "py"), "w", encoding="utf-8") as outfile:
            outfile.write(result)
    else:
        raise RuntimeError(f"Invalid source path '{args.src}' with extension '{ext}'")


if __name__ == "__main__":
    main()
