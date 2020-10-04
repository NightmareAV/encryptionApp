using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace encryptionWPF
{
    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            attempts = 3;
            enable = true;
        }
        public int attempts;
        public bool enable;

        public string firstFile;
        public string secondFile;
        public string thirdFile;
        private string _decryption;
        private string _firstFileText;
        private string _secondFileText;
        private string _thirdFileText;
        private string _encryption;
        private ICommand _decryptionButton;
        private ICommand _encryptionButton;
        private ICommand _loadFile;
        private ICommand _loadSecondFile;
        private ICommand _loadThirdFile;

        public OpenFileDialog openFileDialog = new OpenFileDialog();

        public string Password { get; set; }
        public string Encryption
        {
            get { return _encryption; }
            set { _encryption = value; OnPropertyChanged(nameof(Encryption)); }
        }
        public string Decryption
        {
            get { return _decryption; }
            set { _decryption = value; OnPropertyChanged(nameof(Decryption)); }
        }
        public string FirstFileText
        {
            get { return _firstFileText; }
            set { _firstFileText = value; OnPropertyChanged(nameof(FirstFileText)); }
        }
        public string SecondFileText
        {
            get { return _secondFileText; }
            set { _secondFileText = value; OnPropertyChanged(nameof(SecondFileText)); }
        }
        public string ThirdFileText
        {
            get { return _thirdFileText; }
            set { _thirdFileText = value; OnPropertyChanged(nameof(ThirdFileText)); }
        }
        public ICommand LoadFile
        {
            get { return _loadFile ?? (_loadFile = new RelayCommand(LoadFileExecute, LoadFileCanExecute)); }
        }
        private void LoadFileExecute()
        {
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                firstFile = openFileDialog.FileName;
                FirstFileText = File.ReadAllText(openFileDialog.FileName);
            }

        }
        private bool LoadFileCanExecute()
        {
            return true;
        }
        public ICommand LoadSecondFile
        {
            get { return _loadSecondFile ?? (_loadSecondFile = new RelayCommand(LoadSecondFileExecute, LoadSecondFileCanExecute)); }
        }
        private void LoadSecondFileExecute()
        {
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                secondFile = openFileDialog.FileName;
                SecondFileText = File.ReadAllText(openFileDialog.FileName);
            }

        }
        private bool LoadSecondFileCanExecute()
        {
            return true;
        }
        public ICommand LoadThirdFile
        {
            get { return _loadThirdFile ?? (_loadThirdFile = new RelayCommand(LoadThirdFileExecute, LoadThirdFileCanExecute)); }
        }
        private void LoadThirdFileExecute()
        {
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                thirdFile = openFileDialog.FileName;
            }

        }
        private bool LoadThirdFileCanExecute()
        {
            return true;
        }

        public ICommand EncryptionButton
        {
            get { return _encryptionButton ?? (_encryptionButton = new RelayCommand<object>(EncryptionTextExecute, EncryptionTextCanExecute)); }
        }
        private void EncryptionTextExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string key = passwordBox.Password;
            string str = FirstFileText;
            str = ecryption_decryptionXOR(str, key);
            str = ecryption_decryptionNegation(str, key);
            str = ecryptionPlus(str, key);
            SecondFileText = str;
            File.WriteAllText(secondFile, str);
        }
        private bool EncryptionTextCanExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (string.IsNullOrEmpty(passwordBox.Password) || string.IsNullOrEmpty(FirstFileText)
                || string.IsNullOrEmpty(secondFile))
            {
                return false;
            }
            else
                return true;
        }

        public ICommand DecryptionButton
        {
            get { return _decryptionButton ?? (_decryptionButton = new RelayCommand<object>(DecryptionTextExecute, DecryptionTextCanExecute)); }
        }
        private void DecryptionTextExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string key = passwordBox.Password;
            string str2 = File.ReadAllText(secondFile);
            str2 = decryptionMinus(str2, key);
            str2 = ecryption_decryptionNegation(str2, key);
            str2 = ecryption_decryptionXOR(str2, key);
            ThirdFileText = str2;
            if (checkKey())
            {
                File.WriteAllText(thirdFile, str2);
            }
            else
            {
                attempts--;
                if (attempts == 0)
                {
                    enable = false;
                    File.WriteAllText(firstFile, "");
                    FirstFileText = "";
                    PropertyChanged(this, new PropertyChangedEventArgs("DecryptionButton"));
                }
                MessageBox.Show($"Вы ввели неправильный ключ! Попыток осталось: {attempts}");
            }
        }
        private bool DecryptionTextCanExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (string.IsNullOrEmpty(passwordBox.Password) || string.IsNullOrEmpty(FirstFileText)
                || enable == false || string.IsNullOrEmpty(thirdFile) || string.IsNullOrEmpty(secondFile))
            {
                return false;
            }
            else
                return true;
        }
        public bool checkKey()
        {
            var FFT = FirstFileText.ToCharArray();
            var TFT = ThirdFileText.ToCharArray();
            if (FFT.Length != TFT.Length)
                return false;
            for (int i = 0; i < FFT.Length; i++)
            {
                if (FFT[i] != TFT[i])
                {
                    return false;
                }
            }
            return true;
        }
        // ФУНКЦИИ ШИФРОВАНИЯ - РАСШИФРОВАНИЯ
        // xor
        public string ecryption_decryptionXOR(string str, string key)
        {
            var ch = str.ToCharArray();
            var k = key.ToCharArray();
            string newStr = "";
            int i = 0;
            foreach (var c in ch)
            {
                newStr += xor(c, k[i]);
                if (i == k.Length - 1)
                {
                    i = 0;
                    continue;
                }
                i++;
            }
            return newStr;
        }
        // negation
        public string ecryption_decryptionNegation(string str, string key)
        {
            var ch = str.ToCharArray();
            var k = key.ToCharArray();
            string newStr = "";
            int i = 0;
            foreach (var c in ch)
            {
                newStr += negation(c, key[i]);
                if (i == k.Length - 1)
                {
                    i = 0;
                    continue;
                }
                i++;
            }
            return newStr;
        }
        // plus and minus
        public string ecryptionPlus(string str, string key)
        {
            var ch = str.ToCharArray();
            var k = key.ToCharArray();
            string newStr = "";
            int i = 0;
            foreach (var c in ch)
            {
                newStr += plus(c, key[i]);
                if (i == k.Length - 1)
                {
                    i = 0;
                    continue;
                }
                i++;
            }
            return newStr;
        }
        public string decryptionMinus(string str, string key)
        {
            var ch = str.ToCharArray();
            var k = key.ToCharArray();
            string newStr = "";
            int i = 0;
            foreach (var c in ch)
            {
                newStr += minus(c, key[i]);
                if (i == k.Length - 1)
                {
                    i = 0;
                    continue;
                }
                i++;
            }
            return newStr;
        }
        // xor
        public char xor(char character, int key)
        {
            character = (char)(character ^ key);
            return character;
        }
        // negation
        public char negation(char character, int key)
        {
            character = (char)~(character + key);
            return character;
        }
        // plus and minus
        public char plus(char character, int key)
        {
            character = (char)(character + key);
            return character;
        }
        public char minus(char character, int key)
        {
            character = (char)(character - key);
            return character;
        }

        //--------------------------------------------------------------------
        private void OnPropertyChanged(string PropertyName)
        {
            if (PropertyName == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            if (parameter is T arg)
            {
                _execute.Invoke(arg);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is T arg)
            {
                return _canExecute?.Invoke(arg) ?? true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
