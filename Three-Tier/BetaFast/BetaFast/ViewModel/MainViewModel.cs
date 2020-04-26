using BetaFast.ViewModel.Interfaces;
using System.Collections.Generic;
using System.Linq;

// Code credited to https://www.technical-recipes.com/2016/using-the-mediator-pattern-in-mvvm-wpf/

namespace BetaFast.ViewModel
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
                CurrentViewModel = ViewModels.FirstOrDefault(vm => vm == viewModel);
                return;
            }

            // Check if new ViewModel is Home
            if (ViewModels.IndexOf(viewModel) == 1)
            {
                _history.Clear();
                Mediator.Mediator.Notify("SetHome", "Hidden");
                Mediator.Mediator.Notify("RefreshHome", "");
            }
            else
            {
                Mediator.Mediator.Notify("SetHome", "Visible");
            }

            if (ViewModels.IndexOf(viewModel) == 6)
            {
                Mediator.Mediator.Notify("RefreshUserManagement", "");
            }
            else if (ViewModels.IndexOf(viewModel) == 7)
            {
                Mediator.Mediator.Notify("RefreshMovieManagement", "");
            }
            else if (ViewModels.IndexOf(viewModel) == 9)
            {
                Mediator.Mediator.Notify("RefreshShoppingCart", "");
            }
            else if (ViewModels.IndexOf(viewModel) == 10)
            {
                _history.Clear();
                Mediator.Mediator.Notify("SetHome", "Hidden");
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
            ChangeViewModelDirect(ViewModels[2]);
        }

        private void OnGoToMovie(object obj)
        {
            ChangeViewModelDirect(ViewModels[3]);
        }

        private void OnGoToUserManagement(object obj)
        {
            ChangeViewModelDirect(ViewModels[6]);
        }

        private void OnGoToMovieManagement(object obj)
        {
            ChangeViewModelDirect(ViewModels[7]);
        }

        private void OnGoToPayment(object obj)
        {
            ChangeViewModelDirect(ViewModels[4]);
        }

        private void OnGoToAboutUs(object obj)
        {
            ChangeViewModelDirect(ViewModels[5]);
        }

        private void OnGoToHome(object obj)
        {
            ChangeViewModelDirect(ViewModels[1]);
        }

        private void OnGoToSettings(object obj)
        {
            ChangeViewModelDirect(ViewModels[8]);
        }

        private void OnGoToShoppingCart(object obj)
        {
            ChangeViewModelDirect(ViewModels[9]);
        }

        private void OnGoToPaymentConfirmation(object obj)
        {
            ChangeViewModelDirect(ViewModels[10]);
        }

        public MainViewModel()
        {
            _history = new Stack<int>();

            // Add available pages and set page
            ViewModels.Add(new LoginViewModel());
            ViewModels.Add(new HomeViewModel());
            ViewModels.Add(new RegisterViewModel());
            ViewModels.Add(new MovieViewModel());
            ViewModels.Add(new PaymentViewModel());
            ViewModels.Add(new AboutUsViewModel());
            ViewModels.Add(new UserManagementViewModel());
            ViewModels.Add(new MovieManagementViewModel());
            ViewModels.Add(new SettingsViewModel());
            ViewModels.Add(new ShoppingCartViewModel());
            ViewModels.Add(new PaymentConfirmationViewModel());

            CurrentViewModel = ViewModels[0];

            Mediator.Mediator.Subscribe("GoToHome", this.OnGoToHome);
            Mediator.Mediator.Subscribe("GoToLogin", this.OnLogin);
            Mediator.Mediator.Subscribe("GoToRegister", this.OnRegister);
            Mediator.Mediator.Subscribe("GoToMovie", this.OnGoToMovie);
            Mediator.Mediator.Subscribe("GoToPayment", this.OnGoToPayment);
            Mediator.Mediator.Subscribe("GoToAboutUs", this.OnGoToAboutUs);
            Mediator.Mediator.Subscribe("GoToMovieManagement", this.OnGoToMovieManagement);
            Mediator.Mediator.Subscribe("GoToUserManagement", this.OnGoToUserManagement);
            Mediator.Mediator.Subscribe("GoToSettings", this.OnGoToSettings);
            Mediator.Mediator.Subscribe("GoToShoppingCart", this.OnGoToShoppingCart);
            Mediator.Mediator.Subscribe("GoToPaymentConfirmation", this.OnGoToPaymentConfirmation);

            Mediator.Mediator.Subscribe("GoBack", this.OnGoBack);
        }
    }
}
