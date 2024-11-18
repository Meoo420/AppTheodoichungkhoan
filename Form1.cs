using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace StockTrackerApp
{
    public partial class MainForm : Form
    {
        private List<string> selectedStocks = new List<string>();
        private ChatBot chatBot;

        public MainForm()
        {
            InitializeComponent();
            chatBot = new ChatBot();
            InitializeUI();
        }
        private void InitializeUI()
        {
            this.Text = "Tỷ giá chứng khoán";
            this.Size = new Size(900, 600);
            this.BackColor = Color.Black;

            this.BackgroundImage = Image.FromFile("stonk.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Label stockLabel = new Label();
            stockLabel.Text = "Chọn Mã Chứng Khoán :";
            stockLabel.ForeColor = Color.Red;
            stockLabel.Location = new Point(20, 20);
            stockLabel.Font = new Font("Arial", 14);
            this.Controls.Add(stockLabel);

            ComboBox stockComboBox = new ComboBox();
            stockComboBox.Location = new Point(200, 20);
            stockComboBox.Size = new Size(200, 30);
            stockComboBox.Font = new Font("Arial", 12);
            stockComboBox.Items.Add("Vui lòng chọn mã CK");
            stockComboBox.SelectedIndex = 0;
            stockComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            stockComboBox.Items.AddRange(new string[] { "AAPL", "GOOG", "AMZN", "TSLA", "MSFT", "NVDA", "META", "NFLX", "SPY", "BA", "DIS", "V", "JPM", "PYPL", "AMD", "INTC", "BABA", "XOM", "CVX", "WMT", "MCD", "PEP", "KO" });
            this.Controls.Add(stockComboBox);

            DataGridView stockDataGridView = new DataGridView();
            stockDataGridView.Location = new Point(20, 60);
            stockDataGridView.Size = new Size(402, 200);
            stockDataGridView.ColumnCount = 4;
            stockDataGridView.Columns[0].Name = "Mã CK";
            stockDataGridView.Columns[1].Name = "Giá Mua (USD)";
            stockDataGridView.Columns[2].Name = "Giá Bán (USD)";
            stockDataGridView.Columns[3].Name = "Ngày";
            stockDataGridView.Font = new Font("Arial", 12);
            stockDataGridView.BackgroundColor = Color.White;
            stockDataGridView.ForeColor = Color.Red;
            stockDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stockDataGridView.ReadOnly = true;

            
            stockDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            stockDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            stockDataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            stockDataGridView.RowHeadersVisible = false;  

            stockDataGridView.DefaultCellStyle.SelectionBackColor = Color.LightGray;  
            stockDataGridView.DefaultCellStyle.SelectionForeColor = Color.Red;        
            stockDataGridView.DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Regular);

            this.Controls.Add(stockDataGridView);

            Button fetchButton = new Button();
            fetchButton.Text = "Lấy Dữ Liệu";
            fetchButton.Location = new Point(200, 380);
            fetchButton.Size = new Size(200, 40);
            fetchButton.Font = new Font("Arial", 14);
            fetchButton.BackColor = Color.Black;
            fetchButton.ForeColor = Color.Red;
            fetchButton.Click += (sender, e) => FetchStockData(stockComboBox, stockDataGridView);
            this.Controls.Add(fetchButton);

            Button viewChartButton = new Button();
            viewChartButton.Text = "Xem Sơ Đồ";
            viewChartButton.Location = new Point(200, 440);
            viewChartButton.Size = new Size(200, 40);
            viewChartButton.Font = new Font("Arial", 14);
            viewChartButton.BackColor = Color.Black;
            viewChartButton.ForeColor = Color.Red;
            viewChartButton.Click += (sender, e) => OpenChartFromTable(stockDataGridView);
            this.Controls.Add(viewChartButton);

            Button resetButton = new Button();
            resetButton.Text = "Reset";
            resetButton.Location = new Point(420, 380);
            resetButton.Size = new Size(200, 40);
            resetButton.Font = new Font("Arial", 14);
            resetButton.BackColor = Color.Black;
            resetButton.ForeColor = Color.Red;
            resetButton.Click += (sender, e) => ResetData(stockDataGridView);
            this.Controls.Add(resetButton);

            TextBox chatTextBox = new TextBox();
            chatTextBox.Location = new Point(440, 60);  
            chatTextBox.Size = new Size(420, 100);  
            chatTextBox.Font = new Font("Arial", 12);  
            chatTextBox.Multiline = true;  
            chatTextBox.ReadOnly = true;  
            this.Controls.Add(chatTextBox);

            TextBox userInputTextBox = new TextBox();
            userInputTextBox.Location = new Point(440, 180);  
            userInputTextBox.Size = new Size(420, 30);  
            userInputTextBox.Font = new Font("Arial", 12);
            this.Controls.Add(userInputTextBox);

            Button askButton = new Button();
            askButton.Text = "Gửi";
            askButton.Location = new Point(440, 220);  
            askButton.Size = new Size(100, 40);  
            askButton.Font = new Font("Arial", 14);
            askButton.BackColor = Color.Black;
            askButton.ForeColor = Color.Red;
            askButton.Click += async (sender, e) => await AskChatBot(userInputTextBox.Text, chatTextBox);
            this.Controls.Add(askButton);

        }

        private async void FetchStockData(ComboBox stockComboBox, DataGridView stockDataGridView)
        {
            string selectedStock = stockComboBox.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedStock))
            {
                MessageBox.Show("Vui lòng chọn mã chứng khoán.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedStocks.Contains(selectedStock))
            {
                MessageBox.Show("Bạn đã chọn mã chứng khoán này trước đó.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            selectedStocks.Add(selectedStock);

            try
            {
                var stockData = await GetStockDataFromAPI(selectedStock);

                if (stockData == null || stockData.Length == 0)
                {
                    MessageBox.Show("Không có dữ liệu cho mã chứng khoán này.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var item in stockData)
                {
                    if (item != null)
                    {
                        stockDataGridView.Rows.Add(selectedStock, item.buyPrice ?? 0, item.sellPrice ?? 0, item.date ?? "Không xác định");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi lấy dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<dynamic[]> GetStockDataFromAPI(string stockSymbol)
        {
            string apiKey = "QiALPWZhwZvxYNYE677fdlKGJN7igxZ2";
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string url = $"https://api.polygon.io/v2/aggs/ticker/{stockSymbol}/prev?apiKey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(url);

                    if (string.IsNullOrEmpty(response))
                    {
                        throw new Exception("Dữ liệu trả về từ API rỗng.");
                    }

                    var data = JsonConvert.DeserializeObject<dynamic>(response);

                    if (data?.results == null || data?.results.Count == 0)
                    {
                        throw new Exception("Không có dữ liệu cho mã chứng khoán này.");
                    }

                    var stockData = new dynamic[1];
                    stockData[0] = new
                    {
                        buyPrice = data?.results?[0]?.o ?? 0,  
                        sellPrice = data?.results?[0]?.c ?? 0, 
                        date = today
                    };

                    return stockData;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi truy vấn dữ liệu: {ex.Message}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new dynamic[0];
                }
            }
        }

        private void OpenChartFromTable(DataGridView stockDataGridView)
        {
            if (stockDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một mã chứng khoán từ bảng để xem sơ đồ.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedStock = stockDataGridView.SelectedRows[0].Cells[0].Value?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedStock))
            {
                MessageBox.Show("Vui lòng chọn mã chứng khoán để xem sơ đồ.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string chartUrl = $"https://www.tradingview.com/symbols/{selectedStock}/";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = chartUrl,
                UseShellExecute = true
            });
        }

        private void ResetData(DataGridView stockDataGridView)
        {
            stockDataGridView.Rows.Clear();
            selectedStocks.Clear();
        }

        private async Task AskChatBot(string userInput, TextBox chatTextBox)
        {
             if (string.IsNullOrWhiteSpace(userInput))
    {
        chatTextBox.AppendText("Vui lòng nhập câu hỏi.\r\n");
        return;
    }

        chatTextBox.AppendText($"Bạn: {userInput}{Environment.NewLine}");

        string response = await chatBot.GetPrediction(userInput);

        chatTextBox.AppendText($"Bot: {response}{Environment.NewLine}{Environment.NewLine}");
        chatTextBox.SelectionStart = chatTextBox.Text.Length;
        chatTextBox.ScrollToCaret();
        }
    }
}
