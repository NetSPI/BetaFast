using BetaBank.Utilities;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace BetaBank.ViewProperties
{
    // Credit to http://blog.functionalfun.net/2008/06/wpf-passwordbox-and-data-binding.html

    public static class PasswordBoxProperty
    {
        public static readonly DependencyProperty BoundPassword =
          DependencyProperty.RegisterAttached("BoundPassword", typeof(SecureString), typeof(PasswordBoxProperty), new PropertyMetadata(new SecureString(), OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword", typeof(bool), typeof(PasswordBoxProperty), new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxProperty), new PropertyMetadata(false));

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            box.PasswordChanged -= HandlePasswordChanged;

            SecureString newPassword = (SecureString)e.NewValue;
            if (newPassword.Length == 0)
            {
                box.Clear();
            }

            if (!GetUpdatingPassword(box))
            {
                SecureStringUtility.FillSecureString(box.SecurePassword, newPassword);
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)(e.OldValue);
            bool needToBind = (bool)(e.NewValue);

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            SetUpdatingPassword(box, true);
            SetBoundPassword(box, box.SecurePassword);
            SetUpdatingPassword(box, false);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject dp, SecureString value)
        {
            dp.SetValue(BoundPassword, value);
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
    }
}