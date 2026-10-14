
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            Text = "Municipal Services Application";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(650, 430);
            MinimumSize = new Size(550, 400);
            BackColor = Color.White;

            Label title = new Label();
            title.Text = "Municipal Services";
            title.Font = new Font("Arial", 24, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(175, 40);
            Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "South African Municipal Services Portal";
            subtitle.AutoSize = true;
            subtitle.Location = new Point(185, 90);
            Controls.Add(subtitle);

            // Report Issues
            Button reportButton = CreateButton(
                "Report Issues", 170);

            reportButton.Click += (s, e) =>
            {
                using (ReportIssueForm form =
                    new ReportIssueForm())
                {
                    form.ShowDialog(this);
                }
            };

            // Local Events and Announcements
            Button eventsButton = CreateButton(
                "Local Events and Announcements", 240);

            eventsButton.Click += (s, e) =>
            {
                using (EventsForm form = new EventsForm())
                {
                    form.ShowDialog(this);
                }
            };

            // Reserved for Task 3
            Button statusButton = CreateButton(
                "Service Request Status (Task 3)", 310);

            statusButton.Enabled = false;
        }

        private Button CreateButton(string text, int y)
        {
            Button button = new Button();

            button.Text = text;
            button.Font = new Font(
                "Arial", 11, FontStyle.Bold);

            button.Size = new Size(340, 48);
            button.Location = new Point(155, y);

            button.BackColor = Color.FromArgb(46, 125, 50);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;

            Controls.Add(button);

            return button;
        }
    }
}