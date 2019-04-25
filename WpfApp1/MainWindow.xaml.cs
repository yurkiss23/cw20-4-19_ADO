using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection _connect;
        private string conStr = ConfigurationManager.AppSettings["conStr"];
        private ObservableCollection<Users> users = new ObservableCollection<Users>();
        public MainWindow()
        {
            InitializeComponent();
            _connect = new SqlConnection(conStr);

            DG_Load();
        }
        public void DG_Load()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<Users> usersList = new List<Users>();
                    _connect.Open();
                    SqlCommand cmd = new SqlCommand("SELECT [Id],[Name]FROM[yurkissdb].[dbo].[testUsers]", _connect);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        usersList.Add(new Users
                        {
                            Id = (int)rdr["Id"],
                            Name = rdr["Name"].ToString()
                        });
                    }
                    users = new ObservableCollection<Users>(usersList);
                    _connect.Close();
                    scope.Complete();
                }
            }
            catch
            {
                MessageBox.Show("errors");
            }
            DG.ItemsSource = users;
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUser addUser = new AddUser();
            addUser.ShowDialog();
            users.Add(new Users() { Name = addUser.AddName });
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    _connect.Open();
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].Id != 0)
                        {
                            SqlCommand cmd = new SqlCommand($"UPDATE [dbo].[testUsers]SET[Name] = '{users[i].Name}'WHERE [Id] = {users[i].Id}", _connect);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand($"INSERT INTO [dbo].[testUsers]([Name])VALUES('{users[i].Name}')", _connect);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    _connect.Close();
                    scope.Complete();
                }
            }
            catch
            {
                MessageBox.Show("errors");
            }
            MessageBox.Show($"add user(s)");
            DG_Load();
        }

        private void btnChangeUser_Click(object sender, RoutedEventArgs e)
        {
            if (DG.SelectedItem != null)
            {
                MessageBox.Show((DG.SelectedItem as Users).Id.ToString());
                

                AddUser cngUser = new AddUser();
                MessageBox.Show((DG.SelectedItem as Users).Name.ToString());
                cngUser.txtAddName.Text = (DG.SelectedItem as Users).Name;
                cngUser.ShowDialog();
                //_context.Users.Where(u => u.Id == (DG.SelectedItem as Entities.User).Id).First().Name = cngUser.txtAddName.Text;
                //_context.SaveChanges();
                MessageBox.Show("ok");
                //(DG.SelectedItem as User).Name = "Random Name";
                //using (TransactionScope sc =new TransactionScope())
                //{
                //    _connect.Open();
                //    SqlCommand cmd = new SqlCommand($"UPDATE [dbo].[stor_Users]SET[FirstName] = '{DG.SelectedItems[1]}' WHERE [Id] = '{DG.SelectedItems[0]}'", _connect);
                //    cmd.ExecuteNonQuery();
                //    MessageBox.Show("update user");

                //    _connect.Close();
                //    sc.Complete();
                //}
            }
            DG_Load();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            int delId = (DG.SelectedItem as Users).Id;
            int deleted = 0;
            if (DG.SelectedItem != null)
            {
                users.Remove(DG.SelectedItem as Users);
            }
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    _connect.Open();
                    SqlCommand cmd = new SqlCommand($"DELETE FROM [dbo].[testUsers]WHERE[Id] = {delId}", _connect);
                    deleted = cmd.ExecuteNonQuery();
                    _connect.Close();
                    scope.Complete();
                }
            }
            catch
            {
                MessageBox.Show("error");
            }
            if (deleted > 0)
            {
                MessageBox.Show("delete user(s)");
            }
            else
            {
                MessageBox.Show("delete nothing");
            }
            DG_Load();
            btnChangeUser.IsEnabled = false;
            btnDeleteUser.IsEnabled = false;
        }

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnChangeUser.IsEnabled = true;
            btnDeleteUser.IsEnabled = true;
        }
    }
    public class Users : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
