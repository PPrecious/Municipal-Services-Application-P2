
using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public partial class ReportIssueForm : Form
    {
        private TextBox txtLocation;
        private ComboBox cmbCategory;
        private RichTextBox txtDescription;
        private TextBox txtAttachment;
        private ProgressBar progress;
        private Label lblFeedback;

        public ReportIssueForm()
        {
            InitializeForm();
            CreateControls();
            UpdateProgress();
        }

        private void InitializeForm()
        {
            Text = "Report a Municipal Issue";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(650, 550);
            MinimumSize = new Size(650, 590);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void CreateControls()
        {
            Label title = new Label();
            title.Text = "Report an Issue";
            title.Font = new Font("Arial", 20, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(20, 15);
            Controls.Add(title);

            AddLabel("Location:", 25, 65);
            txtLocation = AddTextBox(25, 88, 590);

            AddLabel("Category:", 25, 125);

            cmbCategory = new ComboBox();
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Location = new Point(25, 148);
            cmbCategory.Size = new Size(590, 28);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Items.AddRange(new object[]
            {
                "Roads",
                "Water and Utilities",
                "Electricity",
                "Sanitation",
                "Waste Management",
                "Street Lighting",
                "Public Safety",
                "Other"
            });
            Controls.Add(cmbCategory);

            AddLabel("Description:", 25, 185);

            txtDescription = new RichTextBox();
            txtDescription.Name = "txtDescription";
            txtDescription.Location = new Point(25, 208);
            txtDescription.Size = new Size(590, 100);
            Controls.Add(txtDescription);

            AddLabel("Attachment (optional):", 25, 320);

            txtAttachment = AddTextBox(25, 343, 450);
            txtAttachment.ReadOnly = true;

            Button btnAttach = new Button();
            btnAttach.Text = "Choose File";
            btnAttach.Location = new Point(485, 342);
            btnAttach.Size = new Size(130, 30);
            btnAttach.Click += AttachFile;
            Controls.Add(btnAttach);

            AddLabel("Report Progress:", 25, 380);

            progress = new ProgressBar();
            progress.Location = new Point(25, 403);
            progress.Size = new Size(590, 22);
            progress.Minimum = 0;
            progress.Maximum = 4;
            Controls.Add(progress);

            lblFeedback = new Label();
            lblFeedback.Text =
                "Every report helps improve your community.";
            lblFeedback.Location = new Point(25, 433);
            lblFeedback.AutoSize = true;
            Controls.Add(lblFeedback);

            Button btnSubmit = new Button();
            btnSubmit.Text = "Submit Report";
            btnSubmit.Location = new Point(25, 470);
            btnSubmit.Size = new Size(180, 40);
            btnSubmit.BackColor = Color.FromArgb(46, 125, 50);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Click += SubmitReport;
            Controls.Add(btnSubmit);

            Button btnBack = new Button();
            btnBack.Text = "Back to Main Menu";
            btnBack.Location = new Point(225, 470);
            btnBack.Size = new Size(180, 40);
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);

            txtLocation.TextChanged += (s, e) => UpdateProgress();
            cmbCategory.SelectedIndexChanged +=
                (s, e) => UpdateProgress();
            txtDescription.TextChanged +=
                (s, e) => UpdateProgress();
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            Controls.Add(label);
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(width, 28);
            Controls.Add(box);
            return box;
        }

        private void AttachFile(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select an Image or Document";
                dialog.Filter =
                    "Images (*.jpg;*.jpeg;*.png;*.gif)|" +
                    "*.jpg;*.jpeg;*.png;*.gif|" +
                    "Documents (*.pdf;*.doc;*.docx;*.txt)|" +
                    "*.pdf;*.doc;*.docx;*.txt|" +
                    "All Files (*.*)|*.*";

                dialog.Multiselect = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtAttachment.Text = dialog.FileName;
                    UpdateProgress();
                    lblFeedback.Text = "Supporting file selected.";
                }
            }
        }

        private void UpdateProgress()
        {
            int value = 0;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text))
                value++;

            if (cmbCategory.SelectedIndex >= 0)
                value++;

            if (!string.IsNullOrWhiteSpace(txtDescription.Text))
                value++;

            if (!string.IsNullOrWhiteSpace(txtAttachment.Text))
                value++;

            progress.Value = value;

            if (value == 0)
                lblFeedback.Text =
                    "Every report helps improve your community.";
            else if (value == 1)
                lblFeedback.Text = "Great start! Keep going.";
            else if (value == 2)
                lblFeedback.Text = "You're halfway there!";
            else if (value == 3)
                lblFeedback.Text = "Almost done!";
            else
                lblFeedback.Text =
                    "Excellent! Your report is ready to submit.";
        }

        private void SubmitReport(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter the issue location.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            if (cmbCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an issue category.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please provide a description.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtDescription.Focus();
                return;
            }

            string reference = "MUN-" +
                DateTime.Now.ToString("yyyyMMddHHmmssfff");

            Issue newIssue = new Issue
            {
                ReferenceNumber = reference,
                Location = txtLocation.Text.Trim(),
                Category = cmbCategory.SelectedItem.ToString(),
                Description = txtDescription.Text.Trim(),
                AttachmentPath = txtAttachment.Text.Trim(),
                DateReported = DateTime.Now
            };

            IssueRepository.AddIssue(newIssue);

            MessageBox.Show(
                "Report submitted successfully!\n\n" +
                "Reference Number: " + reference + "\n\n" +
                (string.IsNullOrWhiteSpace(txtAttachment.Text)
                    ? "No attachment included."
                    : "Attachment selected: " +
                      System.IO.Path.GetFileName(txtAttachment.Text)) +
                "\n\nThank you for helping improve your community.",
                "Report Submitted",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearForm();
        }

        private void ClearForm()
        {
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            txtAttachment.Clear();
            UpdateProgress();
            txtLocation.Focus();
        }
    }
}
