using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class RelayCommand : ICommand, IDisposable
    {
        private Action _execute = null;
        private Predicate<object> _canExecute = null;

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public RelayCommand(Action act, Predicate<object> canExe = null)
        {
            this._execute = act;
            this._canExecute = canExe;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Dispose()
        {
            _execute = null;
            _canExecute = null;
        }

        public void Execute(object parameter = null)
        {
            _execute();
        }
    }


    /// <summary>
    /// 任意の型の引数を1つ受け付けるRelayCommand
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// RaiseCanExecuteChanged が呼び出されたときに生成されます。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 常に実行可能な新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">実行ロジック。</param>
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// 新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">実行ロジック。</param>
        /// <param name="canExecute">実行ステータス ロジック。</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        /// <summary>
        /// 現在の状態でこの <see cref="RelayCommand"/> が実行できるかどうかを判定します。
        /// </summary>
        /// <param name="parameter">
        /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
        /// </param>
        /// <returns>このコマンドが実行可能な場合は true、それ以外の場合は false。</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        /// <summary>
        /// 現在のコマンド ターゲットに対して <see cref="RelayCommand"/> を実行します。
        /// </summary>
        /// <param name="parameter">
        /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
        /// </param>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        /// <summary>
        /// <see cref="CanExecuteChanged"/> イベントを発生させるために使用されるメソッド
        /// <see cref="CanExecute"/> の戻り値を表すために
        /// メソッドが変更されました。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    //public class DelegateCommand<T> : ICommand
    //{

    //    private readonly Action<T> _execute;
    //    private readonly Func<bool> _canExecute;

    //    public event EventHandler CanExecuteChanged;

    //    public DelegateCommand(Action<T> execute) : this(execute, () => true) { }

    //    public DelegateCommand(Action<T> execute, Func<bool> canExecute)
    //    {
    //        this._execute = execute;
    //        this._canExecute = canExecute;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        this._execute((T)parameter);
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return this._canExecute();
    //    }

    //}
}
