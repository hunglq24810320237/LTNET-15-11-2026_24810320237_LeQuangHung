using ClinicManager.GUI;
namespace ClinicManager;
internal static class Program{[STAThread]static void Main(){ApplicationConfiguration.Initialize();Application.Run(new MainForm());}}
