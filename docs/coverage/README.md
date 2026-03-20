# Test Coverage with Codecov

## Overview

This project uses Codecov to track test coverage evolution over time.

## Dashboard Access

1. Go to https://codecov.io
2. Sign in with GitHub
3. Select repository `Altamir/Mnemosyne`

## What Gets Tracked

- **Coverage percentage** per PR and overall
- **Coverage history** over time
- **Flag breakdown** between unit and integration tests
- **Impact analysis** of code changes

## Workflow

The workflow runs automatically on every PR:
1. Runs all tests with Coverlet code coverage
2. Uploads coverage report to Codecov
3. Posts PR comment with coverage summary

## Coverage Goals

| Metric | Target |
|--------|--------|
| Project coverage | > 80% |
| Patch coverage | > 70% |