using BetaBank.Mediator;
using BetaBank.View;
using BetaBank.ViewModel;
using BetaBank.ViewModel.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

// Code credited to https://www.technical-recipes.com/2016/using-the-mediator-pattern-in-mvvm-wpf/

namespace BetaBank.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IViewModel _currentViewModel;
        private List<IViewModel> _viewModels;
        private readonly Stack<int> _history;

        public List<IViewModel> ViewModels
        {
            get
            {
                if (_viewModels == null)
                    _viewModels = new List<IViewModel>();

                return _viewModels;
            }
        }

        public IViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }

        private void ChangeViewModel(IViewModel viewModel)
        {
            if (!ViewModels.Contains(viewModel))
                ViewModels.Add(viewModel);

            // Check if new ViewModel is Login
            if (ViewModels.IndexOf(viewModel) == 0)
            {
                _history.Clear();
                Mediator.Mediator.Notify("ClearLogin", "");
                Mediator.Mediator.Notify("SessionID", new System.Guid("00000000-0000-0000-0000-000000000000"));
                CurrentViewModel = ViewModels.FirstOrDefault(vm => vm == viewModel);
                return;
            }

            // Check if new ViewModel is Home
            if (ViewModels.IndexOf(viewModel) == 2)
            {
                _history.Clear();
                Mediator.Mediator.Notify("SetHome", "Hidden");
            }
            else
            {
                Mediator.Mediator.Notify("SetHome", "Visible");
            }

            CurrentViewModel = ViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        private void ChangeViewModelDirect(IViewModel viewModel)
        {
            _history.Push(ViewModels.IndexOf(CurrentViewModel));
            ChangeViewModel(viewModel);
        }

        private void OnGoBack(object obj)
        {
            if (_history.Count > 0)
            {
                ChangeViewModel(ViewModels[_history.Pop()]);
            }
        }

        private void OnLogin(object obj)
        {
            ChangeViewModelDirect(ViewModels[0]);
        }

        private void OnRegister(object obj)
        {
            ChangeViewModelDirect(ViewModels[1]);
        }

        private void OnHome(object obj)
        {
            ChangeViewModelDirect(ViewModels[2]);
        }

        private void OnAdmin(object obj)
        {
            ChangeViewModelDirect(ViewModels[3]);
        }

        public MainViewModel()
        {
            _history = new Stack<int>();

            // Add available pages and set page
            ViewModels.Add(new LoginViewModel());
            ViewModels.Add(new RegisterViewModel());
            ViewModels.Add(new HomeViewModel());
            ViewModels.Add(new AdminViewModel());

            CurrentViewModel = ViewModels[0];

            Mediator.Mediator.Subscribe("GoToLogin", this.OnLogin);
            Mediator.Mediator.Subscribe("GoToRegister", this.OnRegister);
            Mediator.Mediator.Subscribe("GoToHome", this.OnHome);
            Mediator.Mediator.Subscribe("GoToAdmin", this.OnAdmin);

            Mediator.Mediator.Subscribe("GoBack", this.OnGoBack);
        }
    }
}