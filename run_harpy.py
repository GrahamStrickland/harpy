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
    args = parser.parse_args()

    num_files = 0
    num_successes = 0
    total_time = 0.0
    successful_time = 0.0
    directories = args.directories

    for directory in directories:
        for entry in os.listdir(directory):
            path = Path(directory, entry)
            if os.path.isfile(path) and path.suffix in (".ch", ".prg"):
                num_files += 1
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
            elif os.path.isdir(path):
                directories.append(str(path))

    avg_time_per_file = total_time / num_files if num_files > 0 else 0
    avg_time_per_success = successful_time / num_successes if num_successes > 0 else 0

    print(
        f"Number of files: {num_files}\n"
        f"Number of successes: {num_successes}\n"
        f"Success rate: {(num_successes / num_files) * 100:.2f}%\n"
        f"Total running time: {total_time:.2f}s\n"
        f"Average time per file: {avg_time_per_file:.3f}s\n"
        f"Average time per successful file: {avg_time_per_success:.3f}s"
    )


if __name__ == "__main__":
    main()
