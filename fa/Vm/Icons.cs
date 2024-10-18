using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fa.Data;

namespace fa.Vm;

public class Icons
{
    public Icons()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var zipPath = Path.Combine(home, "Downloads", "fontawesome-free-6.5.2-desktop.zip");
        using var zip = new ZipArchive(File.Open(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read), ZipArchiveMode.Read);
        var f = zip.Entries.FirstOrDefault(x => x.FullName.EndsWith("/metadata/icons.json"));
        if (f is null) throw new Exception($"Couldn't find icons.json in {zipPath}");
        using var json = f.Open();
        List = new IconsSerde().Deserialize(json);
    }

    public JsonIcon[] List { get; }
}
