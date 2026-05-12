namespace Syncrer.Sync;

public record FileInfoRecord(string RelativePath, long LastWriteTimeTicks, long SizeBytes);