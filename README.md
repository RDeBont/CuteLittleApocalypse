# Cute Little Apocalypse

2D pixelart-platformer in Unity (C#). Schoolproject Software Developer niveau 4, Curio.
Een laatste overlevende soldaat vlucht over de daken van een ingenomen stad naar zijn evacuatiepunt.

## Team
- Rowan de Bont — Art, UI en documentatie
- Tycho — Gameplay-programmeur
- Thomas — Leveldesign, vijanden, audio en testen

## Setup
1. Unity <versie> via Unity Hub installeren
2. Git LFS installeren: `git lfs install`
3. Repo clonen en de map in Unity Hub openen via **Add project from disk**

## Branches
- `main` — altijd speelbaar, alleen via pull request
- `develop` — integratie
- `feature/<naam>` — losse features
- `art/<naam>` — art en assets

## Afspraken
- Maximaal één persoon tegelijk in dezelfde scene
- Alles wat kan wordt een prefab
- Commit messages: `type: korte beschrijving` (feat, fix, chore, art, docs)