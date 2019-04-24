﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public string AddName { get; set; }
        public AddUser()
        {
            InitializeComponent();
            txtAddName.Focusable = true;
            //txtAddName.Text = AddName;
        }

        private void BtnAddName_Click(object sender, RoutedEventArgs e)
        {
            AddName = string.IsNullOrEmpty(txtAddName.Text) ? "new user" : txtAddName.Text;
            this.Close();
        }
    }
}
