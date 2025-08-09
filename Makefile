.PHONY: help
help:	## Show this help message
	@echo "  help       Show this help message"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf " \033[36m%-15s\033[0m %s\n", $$1, $$2}' 

.DEFAULT_GOAL := help

.PHONY: install
install:	## Install dependencies with uv
		uv python install 3.13
		uv sync

.PHONY: sync
sync:		## Sync dependencies with uv
		uv sync

.PHONY: clean
clean:		## Clean up temporary files
		find . -type f -name "*.pyc" -delete
		find . -type d -name "__pycache__" -delete
		find . ! -name . -type d -name ".ruff_cache" -exec rm -r {} + -depth
		rm -rf .venv/ .pytest_cache/ .coverage htmlcov/

.PHONY: format
format:		## Format the codebase with ruff and isort
		uv run ruff format
		uv run ruff check --fix
		uv run isort .

.PHONY: lint
lint:		## Lint the codebase with ruff and isort
		uv run ruff check
		uv run isort --check-only .

.PHONY: test
test:		## Run tests with pytest
		uv run -m pytest --verbose

.PHONY: test-cov
test-cov:	## Run pytest tests with coverage report
		uv run -m pytest tests/ --verbose --cov=src --cov-report=html --cov-report=term

.PHONY: check
check: lint test-cov  ## Run linting and tests with coverage