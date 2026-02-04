# Git Workflow

## Worktrees for Feature Development

All new features MUST be developed using git worktrees. This keeps the main worktree clean and allows parallel development of multiple features.

### Why Worktrees?

- **Isolation**: Each feature has its own working directory
- **Parallel work**: Switch between features without stashing or committing WIP
- **Clean main**: The main worktree stays on the main branch for reference
- **Easy cleanup**: Delete the worktree directory when done

### Creating a New Feature

Use the `/new-feature` slash command to create a new worktree:

```
/new-feature
```

This will:
1. Prompt for a feature branch name
2. Create a new branch from main
3. Set up a worktree in `.worktrees/<branch-name>`
4. Change to the new worktree directory

### Manual Worktree Commands

If needed, you can create worktrees manually:

```bash
# Create a new branch and worktree
git worktree add ../.worktrees/feature-name -b feature/feature-name

# List all worktrees
git worktree list

# Remove a worktree (after merging)
git worktree remove ../.worktrees/feature-name
```

### Directory Structure

```
Questline/                    # Main repository (bare or main worktree)
├── .worktrees/
│   ├── main/                 # Main branch worktree
│   ├── feature-auth/         # Feature branch worktree
│   └── feature-inventory/    # Another feature worktree
```

### Workflow

1. **Start feature**: Use `/new-feature` to create worktree
2. **Develop**: Work in the feature worktree
3. **Commit**: Make commits on the feature branch
4. **Push**: Push feature branch to remote
5. **PR**: Create pull request to main
6. **Merge**: Merge PR on GitHub
7. **Cleanup**: Remove the worktree after merge

### Best Practices

- Keep feature branches short-lived
- Regularly pull main into your feature branch to avoid conflicts
- Delete worktrees promptly after merging to avoid clutter
- Use descriptive branch names: `feature/add-inventory`, `fix/parser-bug`
