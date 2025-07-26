# Development Container Setup

## Quick Start

1. **Copy the environment template:**
   ```bash
   cp .devcontainer/.env.template .devcontainer/.env
   ```

2. **Edit `.devcontainer/.env` with your personal details:**
   ```bash
   # Required: Your Git identity
   GIT_USER_NAME="John Doe"
   GIT_USER_EMAIL="john.doe@company.com"
   
   # Optional: Git preferences
   GIT_DEFAULT_BRANCH="main"
   GIT_EDITOR="code --wait"
   ```

3. **Rebuild the dev container** or restart if already running

## What Gets Configured

The setup script will automatically:
- ✅ Configure Git with your name and email from `.env`
- ✅ Set up useful aliases for development
- ✅ Configure Azure emulator connections
- ✅ Install development tools and utilities

## Manual Git Setup (Alternative)

If you prefer not to use the `.env` file:

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

## Security Note

The `.env` file is gitignored and contains only your personal development preferences - no sensitive production secrets.