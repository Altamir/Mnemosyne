# Codecov Setup Guide

## Installing Codecov GitHub App

1. Go to https://github.com/apps/codecov
2. Click "Install"
3. Select your account/organization
4. Choose "Only select repositories" and select `Altamir/Mnemosyne`
5. Click "Install"

## After Installation

Once installed, the Codecov GitHub App will:
- Automatically create a `CODECOV_TOKEN` secret in your repository
- Add PR comments with coverage reports
- Track coverage history

## Verify Installation

1. Go to your repository Settings > Secrets
2. Confirm `CODECOV_TOKEN` secret exists (created automatically by app)

## Manual Token (if needed)

If you need to add the token manually:
1. Go to https://codecov.io
2. Sign in and select your repository
3. Copy the upload token
4. Go to GitHub > Repository > Settings > Secrets > New secret
5. Name: `CODECOV_TOKEN`, Value: paste token