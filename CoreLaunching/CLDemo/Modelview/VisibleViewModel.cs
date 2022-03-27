using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CLDemo.Modelview
{
    public abstract class VisibleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        #region commands
        public class ShowCommand : ICommand
        {
            private VisibleViewModel _visibleViewModel;
            public event EventHandler? CanExecuteChanged;

            public ShowCommand(VisibleViewModel visibleViewModel)
            {
                _visibleViewModel = visibleViewModel;
            }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                ShowVisibility(_visibleViewModel);
                // VisibleViewModel.SetVisibility(_visibleViewModel, Visibility.Visible);
            }

        }

        public class HideCommand : ICommand
        {
            private VisibleViewModel _visibleViewModel;
            public event EventHandler? CanExecuteChanged;

            public HideCommand(VisibleViewModel visibleViewModel)
            {
                _visibleViewModel = visibleViewModel;
            }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                VisibleViewModel.SetVisibility(_visibleViewModel, Visibility.Hidden);
            }
        }
        #endregion

        #region properties
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                RaiseEvent();
            }
        }
        private Visibility _visibility = Visibility.Hidden;

        public ICommand ShowCmd { get; protected set; } // 可重写。
        public ICommand HideCmd { get; protected set; } // 可重写。

        #endregion


        // 上级视图。
        public VisibleViewModel ParentViewModel { get; private set; }

        public IEnumerable<VisibleViewModel> SubViewModels => _subViewModels;
        private List<VisibleViewModel> _subViewModels;

        public VisibleViewModel(VisibleViewModel parentViewModel)
        {
            ParentViewModel = parentViewModel;
            ShowCmd = new ShowCommand(this);
            HideCmd = new HideCommand(this);
            _subViewModels = new List<VisibleViewModel>();
        }

        /// <summary>
        /// 重新设定子功能。
        /// </summary>
        /// <param name="subViewModels"></param>
        protected void ReloadSubViewModel(IEnumerable<VisibleViewModel> subViewModels)
        {
            _subViewModels = new List<VisibleViewModel>(subViewModels);
        }



        protected void RaiseEvent([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 在显示的时候，保证每次显示只显示我自己，把同一级的给隐藏掉。
        protected static void ShowVisibility(VisibleViewModel visibleViewModel)
        {
            var parent = visibleViewModel.ParentViewModel;
            if (parent == null) return;
            foreach (var item in parent.SubViewModels)
            {
                if (item == visibleViewModel)
                    item.Visibility = Visibility.Visible;
                else
                    item.Visibility = Visibility.Hidden;
            }

        }


        protected static void SetVisibility(VisibleViewModel visibleViewModel, Visibility visibility)
        {
            visibleViewModel.Visibility = visibility;
            foreach (var item in visibleViewModel.SubViewModels)
            {
                SetVisibility(item, visibility);
            }
        }

    }
}
