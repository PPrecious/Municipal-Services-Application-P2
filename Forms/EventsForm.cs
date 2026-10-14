using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public partial class EventsForm : Form
    {
        private readonly EventRepository repository =
            new EventRepository();

        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private CheckBox chkDate;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private DataGridView dgvEvents;
        private ListBox lstRecommendations;
        private Label lblDetails;
        private Label lblMessage;

        private List<EventItem> currentResults =
            new List<EventItem>();

        public EventsForm()
        {
            InitializeForm();
            BuildInterface();
            LoadCategories();
            SearchEvents();
        }

        private void InitializeForm()
        {
            Text = "Local Events and Announcements";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1050, 720);
            MinimumSize = new Size(900, 650);
            BackColor = Color.White;
        }

        private void BuildInterface()
        {
            Label title = new Label();
            title.Text = "Local Events and Announcements";
            title.Font = new Font("Arial", 20, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(20, 15);
            Controls.Add(title);

            Label instructions = new Label();
            instructions.Text =
                "Search upcoming events, filter by category or date, and explore recommendations.";
            instructions.AutoSize = true;
            instructions.Location = new Point(22, 55);
            Controls.Add(instructions);

            Label searchLabel = new Label();
            searchLabel.Text = "Search:";
            searchLabel.Location = new Point(22, 95);
            searchLabel.AutoSize = true;
            Controls.Add(searchLabel);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(22, 118);
            txtSearch.Size = new Size(240, 28);
            Controls.Add(txtSearch);

            Label categoryLabel = new Label();
            categoryLabel.Text = "Category:";
            categoryLabel.Location = new Point(280, 95);
            categoryLabel.AutoSize = true;
            Controls.Add(categoryLabel);

            cmbCategory = new ComboBox();
            cmbCategory.Location = new Point(280, 118);
            cmbCategory.Size = new Size(180, 28);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbCategory);

            chkDate = new CheckBox();
            chkDate.Text = "Filter by date";
            chkDate.Location = new Point(480, 120);
            chkDate.AutoSize = true;
            Controls.Add(chkDate);

            dtpStart = new DateTimePicker();
            dtpStart.Location = new Point(600, 118);
            dtpStart.Size = new Size(135, 28);
            dtpStart.Format = DateTimePickerFormat.Short;
            dtpStart.Value = DateTime.Today;
            Controls.Add(dtpStart);

            dtpEnd = new DateTimePicker();
            dtpEnd.Location = new Point(745, 118);
            dtpEnd.Size = new Size(135, 28);
            dtpEnd.Format = DateTimePickerFormat.Short;
            dtpEnd.Value = DateTime.Today.AddDays(30);
            Controls.Add(dtpEnd);

            Button btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(895, 115);
            btnSearch.Size = new Size(120, 34);
            btnSearch.Click += (s, e) => SearchEvents();
            Controls.Add(btnSearch);

            Button btnClear = new Button();
            btnClear.Text = "Clear Filters";
            btnClear.Location = new Point(895, 155);
            btnClear.Size = new Size(120, 30);
            btnClear.Click += (s, e) => ClearFilters();
            Controls.Add(btnClear);

            Label eventsLabel = new Label();
            eventsLabel.Text = "Available Events";
            eventsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            eventsLabel.Location = new Point(22, 165);
            eventsLabel.AutoSize = true;
            Controls.Add(eventsLabel);

            dgvEvents = new DataGridView();
            dgvEvents.Location = new Point(22, 190);
            dgvEvents.Size = new Size(650, 270);
            dgvEvents.ReadOnly = true;
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.AllowUserToDeleteRows = false;
            dgvEvents.MultiSelect = false;
            dgvEvents.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvEvents.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvents.RowHeadersVisible = false;

            dgvEvents.Columns.Add("Id", "ID");
            dgvEvents.Columns.Add("Title", "Event");
            dgvEvents.Columns.Add("Category", "Category");
            dgvEvents.Columns.Add("Date", "Date");

            dgvEvents.Columns["Id"].FillWeight = 35;
            dgvEvents.Columns["Title"].FillWeight = 150;
            dgvEvents.Columns["Category"].FillWeight = 90;
            dgvEvents.Columns["Date"].FillWeight = 85;

            dgvEvents.SelectionChanged += DgvEvents_SelectionChanged;
            dgvEvents.CellDoubleClick += DgvEvents_CellDoubleClick;

            Controls.Add(dgvEvents);

            Label detailTitle = new Label();
            detailTitle.Text = "Event Details";
            detailTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            detailTitle.Location = new Point(695, 190);
            detailTitle.AutoSize = true;
            Controls.Add(detailTitle);

            lblDetails = new Label();
            lblDetails.Location = new Point(695, 220);
            lblDetails.Size = new Size(320, 240);
            lblDetails.BorderStyle = BorderStyle.FixedSingle;
            lblDetails.Padding = new Padding(8);
            lblDetails.Text = "Select an event to view its details.";
            Controls.Add(lblDetails);

            Label recommendationsTitle = new Label();
            recommendationsTitle.Text = "Recommended for You";
            recommendationsTitle.Font =
                new Font("Arial", 12, FontStyle.Bold);
            recommendationsTitle.Location = new Point(22, 475);
            recommendationsTitle.AutoSize = true;
            Controls.Add(recommendationsTitle);

            lstRecommendations = new ListBox();
            lstRecommendations.Location = new Point(22, 505);
            lstRecommendations.Size = new Size(650, 125);
            lstRecommendations.DoubleClick +=
                LstRecommendations_DoubleClick;
            Controls.Add(lstRecommendations);

            lblMessage = new Label();
            lblMessage.Location = new Point(695, 490);
            lblMessage.Size = new Size(320, 90);
            lblMessage.Text =
                "Tip: Search for a topic such as community, water, or education.";
            Controls.Add(lblMessage);

            Button btnBack = new Button();
            btnBack.Text = "Back to Main Menu";
            btnBack.Location = new Point(695, 600);
            btnBack.Size = new Size(200, 40);
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");

            foreach (string category in repository.GetCategories())
            {
                cmbCategory.Items.Add(category);
            }

            cmbCategory.SelectedIndex = 0;
        }

        private void SearchEvents()
        {
            if (chkDate.Checked &&
                dtpEnd.Value.Date < dtpStart.Value.Date)
            {
                MessageBox.Show(
                    "The end date cannot be earlier than the start date.",
                    "Invalid Date Range",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string category =
                cmbCategory.SelectedItem == null
                ? "All Categories"
                : cmbCategory.SelectedItem.ToString();

            currentResults = repository.SearchEvents(
                txtSearch.Text,
                category,
                chkDate.Checked,
                dtpStart.Value.Date,
                dtpEnd.Value.Date);

            dgvEvents.Rows.Clear();

            foreach (EventItem item in currentResults)
            {
                int rowIndex = dgvEvents.Rows.Add(
                    item.EventId,
                    item.Title,
                    item.Category,
                    item.EventDate.ToString("dd MMM yyyy"));

                dgvEvents.Rows[rowIndex].Tag = item;
            }

            lblMessage.Text = currentResults.Count +
                " event(s) found. Your searches help personalise recommendations.";

            UpdateRecommendations();

            if (dgvEvents.Rows.Count > 0)
            {
                dgvEvents.Rows[0].Selected = true;
                ShowEventDetails(currentResults[0]);
            }
            else
            {
                lblDetails.Text =
                    "No events match your search. Try changing your filters.";
            }
        }

        private void ClearFilters()
        {
            txtSearch.Clear();
            cmbCategory.SelectedIndex = 0;
            chkDate.Checked = false;
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today.AddDays(30);

            SearchEvents();
        }

        private void DgvEvents_SelectionChanged(
            object sender, EventArgs e)
        {
            if (dgvEvents.SelectedRows.Count == 0)
            {
                return;
            }

            EventItem item =
                dgvEvents.SelectedRows[0].Tag as EventItem;

            ShowEventDetails(item);
        }

        private void DgvEvents_CellDoubleClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            EventItem item =
                dgvEvents.Rows[e.RowIndex].Tag as EventItem;

            if (item != null)
            {
                repository.RecordViewed(item);
                UpdateRecommendations();

                MessageBox.Show(
                    item.Title + Environment.NewLine +
                    Environment.NewLine +
                    "Category: " + item.Category + Environment.NewLine +
                    "Date: " + item.EventDate.ToString("dd MMMM yyyy") +
                    Environment.NewLine +
                    "Location: " + item.Location +
                    Environment.NewLine +
                    Environment.NewLine +
                    item.Description,
                    "Event Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void ShowEventDetails(EventItem item)
        {
            if (item == null)
            {
                return;
            }

            lblDetails.Text =
                "Title: " + item.Title +
                Environment.NewLine + Environment.NewLine +
                "Category: " + item.Category +
                Environment.NewLine + Environment.NewLine +
                "Date: " + item.EventDate.ToString("dd MMMM yyyy") +
                Environment.NewLine + Environment.NewLine +
                "Location: " + item.Location +
                Environment.NewLine + Environment.NewLine +
                item.Description;
        }

        private void UpdateRecommendations()
        {
            lstRecommendations.Items.Clear();

            List<EventItem> recommendations =
                repository.GetRecommendations();

            foreach (EventItem item in recommendations)
            {
                lstRecommendations.Items.Add(item);
            }

            if (recommendations.Count == 0)
            {
                lstRecommendations.Items.Add(
                    "No recommendations available yet.");
            }
        }

        private void LstRecommendations_DoubleClick(
            object sender, EventArgs e)
        {
            EventItem item =
                lstRecommendations.SelectedItem as EventItem;

            if (item == null)
            {
                return;
            }

            repository.RecordViewed(item);
            ShowEventDetails(item);

            MessageBox.Show(
                item.Title + Environment.NewLine +
                Environment.NewLine +
                item.EventDate.ToString("dd MMMM yyyy") +
                Environment.NewLine +
                item.Location + Environment.NewLine +
                Environment.NewLine +
                item.Description,
                "Recommended Event",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            UpdateRecommendations();
        }
    }
}