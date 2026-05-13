# Syncrer

Syncrer is a small .NET console application for one-way folder synchronization.
It copies files from a source folder to a target folder, then keeps checking for
changes on a fixed schedule.

## Status

This project is early-stage and experimental. The current implementation focuses
on scheduled one-way synchronization and basic change detection.

## Requirements

- .NET 10 SDK

## Usage

Run Syncrer with a source folder, target folder, and sync interval in seconds:

```bash
dotnet run --project Syncrer -- \
  --source-folder /path/to/source \
  --target-folder /path/to/target \
  --sync-interval 30
```

The sync interval must be at least 10 seconds.

## Notes

- Synchronization is one-way: source to target.
- The app runs until stopped with `Ctrl-C`.
- The target folder should be treated as managed by Syncrer while the app is running.

## License

MIT
