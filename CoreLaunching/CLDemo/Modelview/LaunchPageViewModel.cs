using CLDemo.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CLDemo.Modelview
{
    /// <summary>
    /// 最顶层的视图模型。
    /// </summary>
    public class LaunchPageViewModel : VisibleViewModel
    {
        public LegacyUserViewModel LegacyUserViewModel { get; private set; }

        public ObservableCollection<LegacyPlayerInfo> LegacyPlayerInfos { get; set; }

        public LaunchGameViewModel LaunchGameViewModel { get; private set; }

        public GameDirMgrViewModel GameDirMgrViewModel { get; private set; }
        #region command

        public class RemoveCommand : ICommand
        {

            public event EventHandler? CanExecuteChanged;

            private LaunchPageViewModel _launchViewModel;

            public RemoveCommand(LaunchPageViewModel launchViewModel)
            {
                _launchViewModel = launchViewModel;
            }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                if (parameter is Selector == false) return; 

                var selector = parameter as Selector;
                var targetObject = selector.SelectedItem;
                
                
                if (targetObject == null || targetObject is LegacyPlayerInfo == false)
                {
                    MessageBox.Show("请选择一个项目进行删除");
                }
                else
                {
                    _launchViewModel.LegacyPlayerInfos.Remove(targetObject as LegacyPlayerInfo);
                }

            }

        }

        #endregion

        public ICommand RemoveCmd { get; private set; }
        public LaunchPageViewModel() : base(null)
        {
            LegacyUserViewModel = new LegacyUserViewModel(this);
            LaunchGameViewModel = new LaunchGameViewModel(this);
            RemoveCmd = new RemoveCommand(this);
            GameDirMgrViewModel = new GameDirMgrViewModel(this);
            ReloadSubViewModel(new VisibleViewModel[] { LegacyUserViewModel, GameDirMgrViewModel});
            LegacyPlayerInfos = Data.GeneralData.LegacyPlayerInfos;
        }
    }
}
