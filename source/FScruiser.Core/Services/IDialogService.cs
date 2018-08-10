using FScruiser.Models;
using System;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface IDialogService
    {
        Task<bool> AskCancelAsync(String message, String caption, bool defaultCancel);

        Task AskCruiserAsync(TallyEntry tree);

        Task<string> AskValueAsync(string prompt, params string[] values);

        Task<int?> AskKPIAsync(int max, int min = 1);

        Task<bool> AskYesNoAsync(string message, String caption, bool defaultNo = false);

        Task ShowMessageAsync(string message, string caption = null);

        Task ShowEditTreeAsync(string tree_guid);
    }
}