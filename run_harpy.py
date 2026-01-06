#!/usr/bin/env python3
import argparse
import os
import subprocess
import time
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(
        description="Run Harpy parser on .ch and .prg files in specified directories"
    )
    parser.add_argument(
        "directories",
        nargs="*",
        help="Directories to search for .ch and .prg files",
    )
    parser.add_argument(
        "--stop-on-error",
        action="store_true",
        help="Stop execution when a file cannot be parsed and print the error",
    )
    args = parser.parse_args()

    num_files = 0
    num_successes = 0
    total_time = 0.0
    successful_time = 0.0
    total_lines = 0
    successfully_parsed_lines = 0
    directories = args.directories

    for directory in directories:
        for entry in os.listdir(directory):
            path = Path(directory, entry)
            if os.path.isfile(path) and path.suffix in (".ch", ".prg"):
                num_files += 1

                # Count lines in the file
                with open(path, "r", encoding="utf-8", errors="ignore") as f:
                    file_lines = sum(1 for _ in f)
                total_lines += file_lines

                start_time = time.time()
                result = subprocess.run(
                    [
                        "Harpy/bin/Debug/net9.0/Harpy",
                        "--src",
                        path,
                    ],
                    capture_output=True,
                )
                elapsed_time = time.time() - start_time
                total_time += elapsed_time

                if result.returncode == 0:
                    num_successes += 1
                    successful_time += elapsed_time
                    successfully_parsed_lines += file_lines
                else:
                    if args.stop_on_error:
                        print(f"Error parsing file: {path}")
                        if result.stderr:
                            print(result.stderr.decode("utf-8", errors="ignore"))
                        if result.stdout:
                            print(result.stdout.decode("utf-8", errors="ignore"))
                        return
            elif os.path.isdir(path):
                directories.append(str(path))

    avg_time_per_file = total_time / num_files if num_files > 0 else 0
    avg_time_per_success = successful_time / num_successes if num_successes > 0 else 0

    lines_success_rate = (
        (successfully_parsed_lines / total_lines) * 100 if total_lines > 0 else 0
    )

    print(
        f"Number of files: {num_files}\n"
        f"Number of successes: {num_successes}\n"
        f"Success rate: {(num_successes / num_files) * 100:.2f}%\n"
        f"Total lines: {total_lines}\n"
        f"Successfully parsed lines: {successfully_parsed_lines}\n"
        f"Lines success rate: {lines_success_rate:.2f}%\n"
        f"Total running time: {total_time:.2f}s\n"
        f"Average time per file: {avg_time_per_file:.3f}s\n"
        f"Average time per successful file: {avg_time_per_success:.3f}s"
    )


if __name__ == "__main__":
    main()
