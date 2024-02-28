using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestAdapter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable table = new DataTable("NhanVien");
        MySqlConnection connection;
        MySqlDataAdapter adapter;
        DataSet dataSet = new DataSet();
        public MainWindow()
        {
            InitAdapter();
            InitializeComponent();
        }

        private void InitAdapter()
        {
            var sqlStringBuilder = new MySqlConnectionStringBuilder
            {
                ["Server"] = "127.0.0.1",
                ["Database"] = "tyanlab",

                // UserID
                ["UID"] = "root",
                ["PWD"] = "abc123",

                // Port ảo của MySQL trong Docker
                ["Port"] = "3307"
            };

            var sqlStringConnection = sqlStringBuilder.ToString();

            // using giải phóng `MySqlConnection`
            connection = new MySqlConnection(sqlStringConnection);

            connection.Open();

            adapter = new MySqlDataAdapter();
            adapter.TableMappings.Add("Table", "NhanVien");

            // SelectCommand
            adapter.SelectCommand = new MySqlCommand("SELECT NhanviennID, Ten, Ho, NgaySinh FROM Nhanvien", connection);

            // InsertCommand
            adapter.InsertCommand = new MySqlCommand("INSERT INTO Nhanvien (Ho, Ten, NgaySinh) values (@Ho, @Ten, @NgaySinh)", connection);
            adapter.InsertCommand.Parameters.Add(new MySqlParameter("@Ho", MySqlDbType.VarChar, 255, "Ho"));
            adapter.InsertCommand.Parameters.Add(new MySqlParameter("@Ten", MySqlDbType.VarChar, 255, "Ten"));
            adapter.InsertCommand.Parameters.Add(new MySqlParameter("@NgaySinh", MySqlDbType.VarChar, 255, "NgaySinh"));

            // DeleteCommand
            adapter.DeleteCommand = new MySqlCommand("DELETE FROM Nhanvien WHERE NhanviennID = @id", connection);
            adapter.DeleteCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 4, "NhanviennID"));

            // UpdateCommand
            adapter.UpdateCommand = new MySqlCommand("UPDATE Nhanvien Set Ho = @Ho, Ten = @Ten, NgaySinh = @NgaySinh WHERE NhanviennID = @id", connection);
            adapter.UpdateCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 4, "NhanviennID"));
            adapter.UpdateCommand.Parameters.Add(new MySqlParameter("@Ho", MySqlDbType.VarChar, 255, "Ho"));
            adapter.UpdateCommand.Parameters.Add(new MySqlParameter("@Ten", MySqlDbType.VarChar, 255, "Ten"));
            adapter.UpdateCommand.Parameters.Add(new MySqlParameter("@NgaySinh", MySqlDbType.VarChar, 255, "NgaySinh"));

            dataSet.Tables.Add(table);
        }

        void LoadDataTable()
        {
            table.Rows.Clear();
            adapter.Fill(dataSet);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataTable();

            // Load từ CSDL lên Dataset
            datagrid.DataContext = table.DefaultView;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadDataTable();

            // Load từ CSDL lên Dataset
            datagrid.DataContext = table.DefaultView;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            adapter.Update(dataSet);
            table.Rows.Clear();
            adapter.Fill(dataSet);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedItem = (DataRowView)datagrid.SelectedItem;

            if (selectedItem != null)
            {
                selectedItem.Delete();
            }
        }
    }
}
