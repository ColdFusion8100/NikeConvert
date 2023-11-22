using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: CommandLineApp.exe <inputFilePath> <outputPath> [-s]");
            return;
        }

        string inputFilePath = args[0];
        string outputPath = args[1];
        bool saveAsSingleDocument = Array.Exists(args, element => element.Equals("-s", StringComparison.OrdinalIgnoreCase));

        // Generate a unique identifier for this set of data
        Guid uniqueId = Guid.NewGuid();

        List<string[]> lapData = new List<string[]>();
        Dictionary<string, string> workoutDetails = new Dictionary<string, string>();
        List<string[]> sampleData = new List<string[]>();

        // Read the input file and process the data
        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            string section = "";

            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.Contains("Lap Number"))
                    {
                        section = "Lap Data";
                        continue;
                    }
                    else if (line.Contains("Workout Name"))
                    {
                        section = "Workout Details";
                        continue;
                    }
                    else if (line.Contains("Sample Number"))
                    {
                        section = "Sample Data";
                        continue;
                    }

                    switch (section)
                    {
                        case "Lap Data":
                            lapData.Add(line.Split('\t'));
                            break;
                        case "Workout Details":
                            string[] workoutInfo = line.Split('\t');
                            // Check if workoutInfo has at least two elements before accessing them
                            if (workoutInfo.Length >= 2)
                            {
                                workoutDetails.Add(workoutInfo[0], workoutInfo[1]);
                            }
                            break;
                        case "Sample Data":
                            sampleData.Add(line.Split('\t'));
                            break;
                    }
                }
            }
        }

        string dateSuffix = DateTime.Now.ToString("MMddyyyy");

        if (saveAsSingleDocument)
        {
            // Write the processed data to a single CSV file
            WriteCSVFile(Path.Combine(outputPath, $"CombinedData_{dateSuffix}.csv"), "UniqueID,Lap Number,Lap Cumulative Hours,Minutes,Seconds,Hundredths", lapData, workoutDetails, sampleData, uniqueId);
        }
        else
        {
            // Write the processed data to individual CSV files
            WriteCSVFile(Path.Combine(outputPath, $"LapData_{dateSuffix}.csv"), "UniqueID,Lap Number,Lap Cumulative Hours,Minutes,Seconds,Hundredths", lapData, uniqueId);
            WriteCSVFile(Path.Combine(outputPath, $"WorkoutDetails_{dateSuffix}.csv"), "UniqueID,Workout Name,Workout Steps", workoutDetails, uniqueId);
            WriteCSVFile(Path.Combine(outputPath, $"SampleData_{dateSuffix}.csv"), "UniqueID,Sample Number,Heart Rate (BPM),Cumulative Meters", sampleData, uniqueId);
        }

        Console.WriteLine($"Formatted data has been saved to {outputPath}");
    }

    static void WriteCSVFile(string filePath, string header, IEnumerable<string[]> data, Guid uniqueId)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header
            writer.WriteLine(header);

            // Write the data
            foreach (var record in data)
            {
                writer.WriteLine($"{uniqueId},{string.Join(",", record)}");
            }
        }
    }

    static void WriteCSVFile(string filePath, string header, Dictionary<string, string> data, Guid uniqueId)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header
            writer.WriteLine(header);

            // Write the data
            foreach (var entry in data)
            {
                writer.WriteLine($"{uniqueId},{entry.Key},{entry.Value}");
            }
        }
    }

    static void WriteCSVFile(string filePath, string header, List<string[]> data, Guid uniqueId)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header
            writer.WriteLine(header);

            // Write the data
            foreach (var record in data)
            {
                writer.WriteLine($"{uniqueId},{string.Join(",", record)}");
            }
        }
    }

    static void WriteCSVFile(string filePath, string header, List<string[]> lapData, Dictionary<string, string> workoutDetails, List<string[]> sampleData, Guid uniqueId)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write lap data
            writer.WriteLine("UniqueID,Lap Number,Lap Cumulative Hours,Minutes,Seconds,Hundredths");
            foreach (var lap in lapData)
            {
                writer.WriteLine($"{uniqueId},{string.Join(",", lap)}");
            }
            writer.WriteLine();  // Add an empty line

            // Write workout details
            writer.WriteLine("UniqueID,Workout Name,Workout Steps");
            foreach (var entry in workoutDetails)
            {
                writer.WriteLine($"{uniqueId},{entry.Key},{entry.Value}");
            }
            writer.WriteLine();  // Add an empty line

            // Write sample data
            writer.WriteLine("UniqueID,Sample Number,Heart Rate (BPM),Cumulative Meters");
            foreach (var sample in sampleData)
            {
                writer.WriteLine($"{uniqueId},{string.Join(",", sample)}");
            }
        }
    }
}
