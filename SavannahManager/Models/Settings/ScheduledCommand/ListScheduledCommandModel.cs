using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models.Scheduled;
using _7dtd_svmanager_fix_mvvm.ViewModels.Settings.ScheduledCommand;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.Settings.ScheduledCommand
{
    public class ListScheduledCommandModel : ModelBase
    {
        private readonly ScheduledCommandLoader _commandLoader;

        private ObservableCollection<ScheduledCommandInfo> _scheduledCommands;

        public ObservableCollection<ScheduledCommandInfo> ScheduledCommands
        {
            get => _scheduledCommands;
            set => SetProperty(ref _scheduledCommands, value);
        }


        public ListScheduledCommandModel()
        {
            _scheduledCommands = new ObservableCollection<ScheduledCommandInfo>();

            var loader = new ScheduledCommandLoader();
            _ = loader.LoadFromFileAsync().ContinueWith(t =>
            {
                ScheduledCommands.AddRange(loader.Commands.Select(x => new ScheduledCommandInfo
                {
                    Type = ScheduledCommandInfo.ItemType.Item,
                    Command = x
                }));
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            _commandLoader = loader;
        }

        public void RemoveCommand(int index)
        {
            if (_commandLoader.RemoveCommand(index))
                ScheduledCommands.RemoveAt(index);
        }

        public void AddCommand(Scheduled.ScheduledCommand command)
        {
            _commandLoader.AddCommand(command);
            ScheduledCommands.Add(new ScheduledCommandInfo
            {
                Type = ScheduledCommandInfo.ItemType.Item,
                Command = command
            });
        }

        public void EditCommand(int index, Scheduled.ScheduledCommand command)
        {
            if (index < 0 || index >= ScheduledCommands.Count)
                return;

            ScheduledCommands[index] = new ScheduledCommandInfo
            {
                Type = ScheduledCommandInfo.ItemType.Item,
                Command = command
            };
            _commandLoader.EditCommand(index, command);
        }

        public async Task Save()
        {
            await _commandLoader.SaveToFileAsync();
        }
    }
}
