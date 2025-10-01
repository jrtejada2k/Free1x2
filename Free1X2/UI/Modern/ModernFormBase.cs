using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Free1X2.UI.Modern
{
    /// <summary>
    /// Modern command interface for MVVM-like patterns in WinForms
    /// </summary>
    public interface ICommand
    {
        bool CanExecute(object parameter);
        void Execute(object parameter);
        event EventHandler CanExecuteChanged;
    }

    /// <summary>
    /// Relay command implementation for delegating command logic
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Base class for modern forms with command and data binding support
    /// </summary>
    public abstract class ModernFormBase : Form
    {
        private readonly Container _components = new Container();
        
        protected ModernFormBase()
        {
            InitializeModernForm();
            ConfigureServices();
            ConfigureDataBinding();
            ConfigureCommands();
            ApplyModernStyling();
        }

        private void InitializeModernForm()
        {
            // Modern form defaults
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = SystemFonts.DefaultFont;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(400, 300);
            
            // Enable visual styles
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Configure dependency injection or service provider
        /// </summary>
        protected virtual void ConfigureServices() { }

        /// <summary>
        /// Configure modern data binding patterns
        /// </summary>
        protected virtual void ConfigureDataBinding() { }

        /// <summary>
        /// Configure command bindings
        /// </summary>
        protected virtual void ConfigureCommands() { }

        /// <summary>
        /// Apply modern visual styling
        /// </summary>
        protected virtual void ApplyModernStyling()
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Base class for modern dialogs with proper result handling
    /// </summary>
    /// <typeparam name="TResult">Type of the dialog result</typeparam>
    public abstract class ModernDialogBase<TResult> : ModernFormBase
    {
        public TResult Result { get; protected set; }

        protected ModernDialogBase()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        protected virtual bool ValidateInput()
        {
            return true;
        }

        protected virtual void OnOk()
        {
            if (ValidateInput())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected virtual void OnCancel()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    /// <summary>
    /// Modern data-bound form base with binding source management
    /// </summary>
    public abstract class DataBoundFormBase : ModernFormBase
    {
        private BindingSource _mainBindingSource;

        protected BindingSource MainBindingSource
        {
            get
            {
                if (_mainBindingSource == null)
                {
                    _mainBindingSource = new BindingSource();
                    _mainBindingSource.CurrentChanged += OnCurrentItemChanged;
                }
                return _mainBindingSource;
            }
        }

        protected virtual void OnCurrentItemChanged(object sender, EventArgs e)
        {
            // Override in derived classes for current item change handling
        }

        protected override void ConfigureDataBinding()
        {
            base.ConfigureDataBinding();
            ConfigureBindings();
        }

        protected abstract void ConfigureBindings();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mainBindingSource?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}