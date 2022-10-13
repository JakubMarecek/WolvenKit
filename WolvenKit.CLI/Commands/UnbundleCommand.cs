#define IS_ASYNC
#undef IS_ASYNC

using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using CP77Tools.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WolvenKit.Core.Interfaces;

namespace CP77Tools.Commands;

internal class UnbundleCommand : CommandBase
{
    private const string s_description = "Target an archive to extract files from.";
    private const string s_name = "unbundle";

    public UnbundleCommand() : base(s_name, s_description)
    {
        AddAlias("extract");

        AddArgument(new Argument<FileSystemInfo[]>("path", "Input paths to .archive files or folders."));

        // deprecated. keep for backwards compatibility
        AddOption(new Option<FileSystemInfo[]>(new[] { "--path", "-p" }, "[Deprecated] Input paths to .archive files or folders."));

        AddOption(new Option<DirectoryInfo>(new[] { "--outpath", "-o" }, "Output directory for all input archives."));

        AddOption(new Option<string>(new[] { "--pattern", "-w" }, "Use optional search pattern (e.g. *.ink), if both regex and pattern is defined, pattern will be prioritized."));
        AddOption(new Option<string>(new[] { "--regex", "-r" }, "Use optional regex pattern."));
        AddOption(new Option<string>(new[] { "--hash" }, "Extract single file with a given hash. If a path is supplied, all hashes will be extracted."));
        AddOption(new Option<bool>(new[] { "--DEBUG_decompress" }, "Decompresses all buffers in the unbundled files."));

        SetInternalHandler(CommandHandler.Create<FileSystemInfo[], DirectoryInfo, string, string, string, bool, IHost>(Action));
    }

    private int Action(FileSystemInfo[] path, DirectoryInfo outpath, string pattern, string regex, string hash, bool DEBUG_decompress, IHost host)
    {
        var serviceProvider = host.Services;
        var logger = serviceProvider.GetRequiredService<ILoggerService>();

        if (path == null || path.Length < 1)
        {
            logger.Error("Please fill in an input path.");
            return ConsoleFunctions.ERROR_BAD_ARGUMENTS;
        }

        var consoleFunctions = serviceProvider.GetRequiredService<ConsoleFunctions>();

#if IS_ASYNC
        //Task.WhenAll(consoleFunctions.UnbundleTaskAsync(path, outpath, hash, pattern, regex, DEBUG_decompress)).Wait();#
        throw new NotImplementedException();
#else
        return consoleFunctions.UnbundleTask(path, outpath, hash, pattern, regex, DEBUG_decompress);
#endif
    }
}
