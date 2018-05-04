using FScruiser.Models;
using System;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface IDialogService
    {
        Task<bool> AskCancelAsync(String message, String caption, bool defaultCancel);

        Task AskCruiserAsync(Tree tree);

        Task<int?> AskKPI(int min, int max);

        Task<bool> AskYesNoAsync(String message, String caption);

        Task<bool> AskYesNoAsync(string message, String caption, bool defaultNo);

        Task ShowMessageAsync(string message);

        Task ShowMessageAsync(string message, string caption);

        Task ShowEditTreeAsync(Tree tree);
    }
}