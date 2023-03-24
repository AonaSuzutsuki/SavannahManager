using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using _7dtd_svmanager_fix_mvvm.Models.Settings.ScheduledCommand;
using _7dtd_svmanager_fix_mvvm.Views.Settings.ScheduledCommand;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Settings.ScheduledCommand
{
    public class ScheduledCommandInfo
    {
        public enum ItemType
        {
            Item,
            Dummy
        }

        public ItemType Type { get; set; }

        public Models.Scheduled.ScheduledCommand Command { get; set; }
    }

    public class ListScheduledCommandViewModel : ViewModelWindowStyleBase
    {
        private readonly ListScheduledCommandModel _model;

        public ReadOnlyReactiveCollection<ScheduledCommandInfo> ScheduledCommands { get; set; }
        public ReactiveProperty<int> SelectedCommandIndex { get; set; }

        public ICommand AddItemCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ListScheduledCommandViewModel(IWindowService windowService, ListScheduledCommandModel model) : base(windowService, model)
        {
            _model = model;

            ScheduledCommands = model.ScheduledCommands.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable);
            SelectedCommandIndex = new ReactiveProperty<int>();

            AddItemCommand = new DelegateCommand(AddCommand);
            RemoveItemCommand = new DelegateCommand(RemoveCommand);
            EditItemCommand = new DelegateCommand(EditCommand);
            SaveCommand = new DelegateCommand(Save);
        }

        public void AddCommand()
        {
            var model = new AddCommandModel();
            var vm = new AddCommandViewModel(new WindowService(), model, false);
            WindowManageService.ShowDialog<AddCommand>(vm);

            if (!vm.IsCancel)
                _model.AddCommand(model.Command);
        }

        public void RemoveCommand()
        {
            _model.RemoveCommand(SelectedCommandIndex.Value);
        }

        public void EditCommand()
        {
            var index = SelectedCommandIndex.Value;
            if (index < 0 || index >= ScheduledCommands.Count)
                return;

            var prev = ScheduledCommands[index].Command;

            var model = new AddCommandModel()
            {
                CommandText = prev.Command,
                WaitTimeText = prev.WaitTimeValue.ToString(),
                IntervalText = prev.IntervalValue.ToString(),
                WaitTimeMode = prev.WaitTimeMode,
                IntervalTimeMode = prev.IntervalTimeMode
            };
            var vm = new AddCommandViewModel(new WindowService(), model, true);
            WindowManageService.ShowDialog<AddCommand>(vm);

            if (!vm.IsCancel)
                _model.EditCommand(index, model.Command);
        }

        public void Save()
        {
            _ = _model.Save().ContinueWith(t =>
            {
                WindowManageService.Dispatch(WindowManageService.Close);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
