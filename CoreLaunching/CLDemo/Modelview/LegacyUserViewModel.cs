using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CLDemo.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace CLDemo.Modelview
{
    public class LegacyUserViewModel : VisibleViewModel
    {
        protected void ClearUserName()
        {
            _username = string.Empty;
            RaiseEvent("UserName"); 
            _uuid = Guid.NewGuid().ToString();
            RaiseEvent("Uuid");
        }

        #region commands

        public class AddRecordCommand : ICommand
        {


            private LegacyUserViewModel _legacyUserViewModel;
            public event EventHandler? CanExecuteChanged;

            public AddRecordCommand(LegacyUserViewModel legacyUserViewModel)
            {
                _legacyUserViewModel = legacyUserViewModel;
            }

            public bool CanExecute(object? parameter)
            {
                return _legacyUserViewModel.UserName.Length != 0;
            }

            public void Execute(object? parameter)
            {
                GeneralData.LegacyPlayerInfos.Add(new LegacyPlayerInfo(_legacyUserViewModel.UserName, _legacyUserViewModel.Uuid));
                _legacyUserViewModel.ClearUserName();
            }
        }

        [Obsolete("不要使用，会带来不幸。", true)]
        public class DeleteRecordedCommand : ICommand
        {
            private LegacyUserViewModel _vm;
            public event EventHandler? CanExecuteChanged;
            public DeleteRecordedCommand(LegacyUserViewModel vm)
            {
                _vm = vm;
            }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                int ind = 0;
                for (int i = 0; i < GeneralData.LegacyPlayerInfos.Count; i++)
                {
                    var item = parameter as LegacyUserViewModel;
                    if (item.UserName == GeneralData.LegacyPlayerInfos[i].Name && item.Uuid == GeneralData.LegacyPlayerInfos[i].Uuid)
                    {
                        ind = i;
                        break;
                    }
                }
                GeneralData.LegacyPlayerInfos.RemoveAt(ind);
            }
        }
        #endregion

        public string UserName
        {
            get => _username;
            set
            {
                _username = value;
                if (value == null) throw new Exception("用户名不能为 null。");
                if (value.Length == 0) throw new Exception("用户名不能为空。");
                RaiseEvent();
            }
        }
        private string _username;
        public string Uuid
        {
            get => _uuid;
            set
            {
                var a = Guid.TryParse(value, out var guid);
                if (a == true)
                {
                    _uuid = value;
                    RaiseEvent();
                }
                else
                {
                    throw new Exception($"UUID 不合法");
                }
            }
        }
        private string _uuid;
        public AddRecordCommand AddRecordCmd { get; set; }
        // public DeleteRecordedCommand DeleteRecordedCmd { get; set; }
      

        public LegacyUserViewModel(LaunchPageViewModel launchViewModel):base(launchViewModel)
        {
            _username = "fanbal";
            _uuid = Guid.NewGuid().ToString();
            AddRecordCmd = new AddRecordCommand(this);
            // DeleteRecordedCmd = new DeleteRecordedCommand(this);
          
        }
      
    }
}
