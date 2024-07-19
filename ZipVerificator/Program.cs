using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using log4net;
using log4net.Config;

class Program
{
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static void Main(string[] args)
    {
        // Configurar log4net
        XmlConfigurator.Configure(new FileInfo("log4net.config"));

        if (args.Length < 1)
        {
            Console.WriteLine("Usage: VerifyFileHashes <directory>");
            log.Error("No directory specified.");
            return;
        }

        string directoryPath = args[0];

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"Directory does not exist: {directoryPath}");
            log.Error($"Directory does not exist: {directoryPath}");
            return;
        }

        Console.WriteLine($"Processing directory: {directoryPath}");
        log.Info($"Processing directory: {directoryPath}");

        var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);
        var hashSet = new HashSet<string>();

        foreach (var file in files)
        {
            // Verificar si el archivo es .wav o .mp3
            if (Path.GetExtension(file).ToLower() == ".wav" || Path.GetExtension(file).ToLower() == ".mp3")
            {
                Console.WriteLine($"Processing file: {file}");
                log.Info($"Processing file: {file}");

                if (!File.Exists(file))
                {
                    log.Warn($"File not found: {file}");
                    continue;
                }

                try
                {
                    string fileHash = ComputeFileHash(file);

                    if (hashSet.Contains(fileHash))
                    {
                        log.Warn($"Duplicate file detected: {file}");
                        Console.WriteLine($"Duplicate file detected: {file}");
                    }
                    else
                    {
                        hashSet.Add(fileHash);
                        log.Info($"File is unique: {file}");
                        Console.WriteLine($"File is unique: {file}");
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Error processing file {file}: {ex.Message}");
                }
            }
            else
            {
                log.Warn($"Skipping unsupported file type: {file}");
            }
        }
    }

    private static string ComputeFileHash(string filePath)
    {
        using (var md5 = MD5.Create())
        using (var stream = File.OpenRead(filePath))
        {
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }
    }
}
