using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Free1X2.UI.Modern.Theming;

namespace Free1X2.UI.Modern
{
    public interface ICommand
    {
        bool CanExecute(object parameter);
        void Execute(object parameter);
        event EventHandler CanExecuteChanged;
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute    = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter)     => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Base for all modernized forms. Inherit instead of Form.
    /// Designer-safe: do NOT set UserPaint on a Form.
    /// </summary>
    public abstract class ModernFormBase : Form
    {
        private readonly Container _components = new Container();

        protected ModernFormBase()
        {
            // DPI-aware scaling
            AutoScaleMode = AutoScaleMode.Dpi;

            // Set modern font before InitializeComponent runs in derived class
            Font = ModernTheme.Fonts.Default;

            // Smooth rendering without UserPaint (UserPaint on Form breaks background)
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint,
                true);

            StartPosition   = FormStartPosition.CenterParent;
            MinimumSize     = ModernTheme.Sizes.MinimumFormSize;
            BackColor       = ModernTheme.Colors.Background;
            ForeColor       = ModernTheme.Colors.Text;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyTheme();
        }

        /// <summary>
        /// Override to customize theming after base applies ModernTheme to all controls.
        /// Always call base.ApplyTheme() first.
        /// </summary>
        protected virtual void ApplyTheme()
        {
            ModernTheme.ApplyToForm(this);
        }

        // Hooks for derived classes
        protected virtual void ConfigureServices()    { }
        protected virtual void ConfigureDataBinding() { }
        protected virtual void ConfigureCommands()    { }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _components?.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Base for fixed-size dialogs (no maximize/minimize).
    /// </summary>
    public abstract class ModernDialogBase<TResult> : ModernFormBase
    {
        public TResult Result { get; protected set; }

        protected ModernDialogBase()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            StartPosition   = FormStartPosition.CenterParent;
        }

        protected virtual bool ValidateInput() => true;

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
    /// Form base with BindingSource management.
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

        protected virtual void OnCurrentItemChanged(object sender, EventArgs e) { }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _mainBindingSource?.Dispose();
            base.Dispose(disposing);
        }

        protected abstract void ConfigureBindings();

        protected override void ConfigureDataBinding()
        {
            base.ConfigureDataBinding();
            ConfigureBindings();
        }
    }
}
